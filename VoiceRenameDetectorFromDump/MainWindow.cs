namespace VoiceRenameDetectorFromDump
{
  public partial class MainWindow : Form
  {
    /// <summary>
    /// Ctor
    /// </summary>
    public MainWindow()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Handles clicking the Start button, which begins the process of reading
    /// the input data files for this program.
    /// </summary>
    private void startButton_Click(object sender, EventArgs e)
    {
      openFileDialog.Title = "Select first file";

      if (openFileDialog.ShowDialog(this) == DialogResult.OK)
      {
        var firstPath = openFileDialog.FileName;
        openFileDialog.Title = "Select second file";

        if (openFileDialog.ShowDialog(this) == DialogResult.OK)
        {
          var secondPath = openFileDialog.FileName;

          try
          {
            var firstFile = DialDumpParser.Parse(firstPath);
            var secondFile = DialDumpParser.Parse(secondPath);

            var mergedFile = MergedFile.CreateFromDumpFiles(firstFile, secondFile);

            if (mergedFile.MaybeEqualResponses.Count > 0)
            {
              var resolveMaybesAnswer = MessageBox.Show(
                "Do you want to try to resolve responses that may be equal?",
                "Resolve maybes?",
                MessageBoxButtons.YesNo);

              if (resolveMaybesAnswer == DialogResult.Yes)
              {
                using (MaybeEqualResponseListWindow modalWindow = new MaybeEqualResponseListWindow(mergedFile))
                {
                  modalWindow.ShowDialog(this);
                }
                /*
                foreach (var (firstResponse, secondResponse, distance) in mergedFile.MaybeEqualResponses)
                {
                  var text1 = firstResponse.ResponseText;
                  var text2 = secondResponse.ResponseText;

                  var answer = MessageBox.Show(
                    $"Are these responses the same?\n\n" +
                    $"INFO form ID: {firstResponse.InfoFormId}\n" +
                    $"DIAL editor ID: {firstResponse.TopicEditorId}\n" +
                    $"QUST editor ID: {firstResponse.QuestEditorId}\n\n" +
                    $"{text1}\n\n" +
                    $"INFO form ID: {secondResponse.InfoFormId}\n" +
                    $"DIAL editor ID: {secondResponse.TopicEditorId}\n" +
                    $"QUST editor ID: {secondResponse.QuestEditorId}\n\n" +
                    $"{text2}\n\n" +
                    $"Levenshtein distance: {distance}",
                    "Same responses?",
                    MessageBoxButtons.YesNo);

                  if (answer == DialogResult.Yes)
                  {
                    mergedFile.EqualResponses.Add((firstResponse, secondResponse));
                    mergedFile.UnmatchedFromFirst.Remove(firstResponse);
                    mergedFile.UnmatchedFromSecond.Remove(secondResponse);
                  }
                }
                */
              }
            }

            var equalResponsesListViewItems = mergedFile.EqualResponses
              .Select(responsePair => MakeEqualResponseListViewItem(
                responsePair.Item1,
                responsePair.Item2));

            var unmatchedFromFirstListViewItems = mergedFile.UnmatchedFromFirst
              .Select(MakeUnmatchedFromFirstListViewItem);

            var unmatchedFromSecondListViewItems = mergedFile.UnmatchedFromSecond
              .Select(MakeUnmatchedFromSecondListViewItem);

            var allListViewItems = equalResponsesListViewItems
              .Concat(unmatchedFromFirstListViewItems)
              .Concat(unmatchedFromSecondListViewItems)
              .OrderBy(GetFirstFileQuestEditorId)
              .ThenBy(GetSecondFileQuestEditorId);

            listView1.BeginUpdate();
            listView1.Items.AddRange([..allListViewItems]);
            listView1.EndUpdate();
          }
          catch (Exception ex)
          {
            MessageBox.Show(ex.ToString());
          }
        }
      }
    }

    /// <summary>
    /// Makes a <see cref="ListViewItem"/> to represent a pair of
    /// <see cref="Response"/> objects that have been determined to refer to the
    /// same voice line.
    /// </summary>
    /// 
    /// <param name="first">
    /// The <see cref="Response"/> from the first file. Must not be null.
    /// </param>
    /// 
    /// <param name="second">
    /// The <see cref="Response"/> from the second file that matches the first.
    /// Must not be null.
    /// </param>
    /// 
    /// <returns>
    /// A <see cref="ListViewItem"/> with the data from the inputs formatted for
    /// display in a <see cref="ListView"/>. Never returns null.
    /// </returns>
    /// 
    /// <remarks>
    /// The relationship between voice lines from the first file and voice lines
    /// from the second file is 1:many, so there can be multiple entries with
    /// the same value for <paramref name="first"/>. There should only be one
    /// entry for each value of <paramref name="second"/>.
    /// </remarks>
    private static ListViewItem MakeEqualResponseListViewItem(
      Response first, Response second)
    {
      ArgumentNullException.ThrowIfNull(first);
      ArgumentNullException.ThrowIfNull(second);

      var listViewItem = new ListViewItem(
      [
        first.QuestEditorId,
        first.TopicEditorId,
        first.InfoFormId,
        first.ResponseNumber.ToString(),
        first.ResponseText,
        second.QuestEditorId,
        second.TopicEditorId,
        second.InfoFormId,
        second.ResponseNumber.ToString(),
        second.ResponseText
      ])
      { Tag = new Tuple<Response?, Response?>(first, second) };

      return listViewItem;
    }

    /// <summary>
    /// Makes a <see cref="ListViewItem"/> to represent a <see cref="Response"/>
    /// object from the first file that has no match determined in the second
    /// file.
    /// </summary>
    /// 
    /// <param name="response">
    /// The <see cref="Response"/> from the first file. Must not be null.
    /// </param>
    /// 
    /// <returns>
    /// A <see cref="ListViewItem"/> with the data from the inputs formatted for
    /// display in a <see cref="ListView"/>. The columns corresponding to data
    /// from the second file will receive an empty string. Never returns null.
    /// </returns>
    /// 
    /// <remarks>
    /// The relationship between voice lines from the first file and voice lines
    /// from the second file is 1:many. Since these are intended for use with
    /// Skyblivion, an unmatched value from the first file means a voice line in
    /// Oblivion that doesn't have an equivalent line in Skyblivion determined.
    /// It could be simply that the correct match has not been determined yet,
    /// or it could mean that this voice line will go completely unused in
    /// Skyblivion.
    /// </remarks>
    private ListViewItem MakeUnmatchedFromFirstListViewItem(
      Response response)
    {
      ArgumentNullException.ThrowIfNull(response);

      var listViewItem = new ListViewItem(
      [
        response.QuestEditorId,
        response.TopicEditorId,
        response.InfoFormId,
        response.ResponseNumber.ToString(),
        response.ResponseText,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty
      ])
      { Tag = new Tuple<Response?, Response?>(response, null) };

      return listViewItem;
    }

    /// <summary>
    /// Makes a <see cref="ListViewItem"/> to represent a <see cref="Response"/>
    /// object from the second file that has no match determined in the first
    /// file.
    /// </summary>
    /// 
    /// <param name="response">
    /// The <see cref="Response"/> from the second file. Must not be null.
    /// </param>
    /// 
    /// <returns>
    /// A <see cref="ListViewItem"/> with the data from the inputs formatted for
    /// display in a <see cref="ListView"/>. The columns corresponding to data
    /// from the first file will receive an empty string. Never returns null.
    /// </returns>
    /// 
    /// <remarks>
    /// The relationship between voice lines from the first file and voice lines
    /// from the second file is 1:many. Since these are intended for use with
    /// Skyblivion, an unmatched value from the second file means that there is
    /// a voice line in Skyblivion whose corresponding voice line (and
    /// consequently the correct audio for it) has not yet been determined. An
    /// unmatched line on the second file means the line will not have audio.
    /// This *may* be OK for blank lines containing only a space, where the line
    /// was added to compensate for differences in the dialogue system.
    /// </remarks>
    private ListViewItem MakeUnmatchedFromSecondListViewItem(
      Response response)
    {
      ArgumentNullException.ThrowIfNull(response);

      var listViewItem = new ListViewItem(
      [
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        response.QuestEditorId,
        response.TopicEditorId,
        response.InfoFormId,
        response.ResponseNumber.ToString(),
        response.ResponseText
      ])
      { Tag = new Tuple<Response?, Response?>(null, response) };

      return listViewItem;
    }

    /// <summary>
    /// Given a <see cref="ListViewItem"/>, returns the quest editor ID from
    /// that item that came from the first input file.
    /// </summary>
    /// 
    /// <param name="listViewItem">
    /// The <see cref="ListViewItem"/>. Must not be null.
    /// </param>
    /// 
    /// <returns>
    /// The quest editor ID from the first file. Since unmatched voice lines
    /// from the second file should receive an empty string for this value,
    /// this should never return null but may return an empty string.
    /// </returns>
    private string GetFirstFileQuestEditorId(ListViewItem listViewItem)
    {
      ArgumentNullException.ThrowIfNull(listViewItem);
      return listViewItem.Text;
    }

    /// <summary>
    /// Given a <see cref="ListViewItem"/>, returns the quest editor ID from
    /// that item that came from the second input file.
    /// </summary>
    /// 
    /// <param name="listViewItem">
    /// The <see cref="ListViewItem"/>. Must not be null.
    /// </param>
    /// 
    /// <returns>
    /// The quest editor ID from the second file. Since unmatched voice lines
    /// from the first file should receive an empty string for this value,
    /// this should never return null but may return an empty string.
    /// </returns>
    private string GetSecondFileQuestEditorId(ListViewItem listViewItem)
    {
      ArgumentNullException.ThrowIfNull(listViewItem);
      return listViewItem.SubItems[4].Text;
    }

    private static string LimitTo(string str, int length)
    {
      return str.Substring(0, Math.Min(str.Length, length));
    }

    private static string GetFileName(Response response, bool limit)
    {
      //Old file names do not seem to be limited, like nqdcheydinhal_cheydinhalnqdquestionresponses_0003e95c_1.
      string questEditorID = response.QuestEditorId;
      if (limit) { questEditorID = LimitTo(questEditorID, 10); }
      string questEditorIDAndTopicEditorID= questEditorID + "_" + response.TopicEditorId;
      if (limit) { questEditorIDAndTopicEditorID = LimitTo(questEditorIDAndTopicEditorID, 26); }
      questEditorIDAndTopicEditorID = questEditorIDAndTopicEditorID.ToLower();
      return questEditorIDAndTopicEditorID + "_" + response.InfoFormId.ToLower() + "_" + response.ResponseNumber;
    }

    private void ExportButton_Click(object sender, EventArgs e)
    {
      var responseTuples = listView1.Items.Cast<ListViewItem>()
        .Select(lvi => (Tuple<Response?, Response?>)lvi.Tag!)
        .Where(t => t.Item1 != null && t.Item2 != null)
        .Select(t => new Tuple<Response, Response, string, string>(t.Item1!, t.Item2!, GetFileName(t.Item1!, false), GetFileName(t.Item2!, true)))
        .ToArray();
      /*var nonDistinctOld = responseTuples.GroupBy(t => t.Item3).Where(g => g.Count() > 1).ToArray();
      var nonDistinctNew = responseTuples.GroupBy(t => t.Item4).Where(g => g.Count() > 1).ToArray();
      if (nonDistinctOld.Length > 0)
      {
        MessageBox.Show("Some old paths were non-distinct:\r\n" + string.Join("\r\n", nonDistinctOld.Select(g => g.Key + ": " + string.Join(", ", g.Select(t => t.Item4)))));
      }
      if (nonDistinctNew.Length > 0)
      {
        MessageBox.Show("Some new paths were non-distinct:\r\n" + string.Join("\r\n", nonDistinctNew.Select(g => g.Key + ": " + string.Join(", ", g.Select(t => t.Item3)))));
      }*/
      var distinctNew = responseTuples.DistinctBy(t=>t.Item4).ToArray();
      string output = string.Join("\r\n\r\n", distinctNew.Select(t =>
      {
        return t.Item3 + "\r\n" + t.Item4;
      }));
      File.WriteAllText("VoiceRenameDetectorOutput.txt", output);
    }
  }
}
