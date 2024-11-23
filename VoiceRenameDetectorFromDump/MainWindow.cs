namespace VoiceRenameDetectorFromDump
{
  public partial class MainWindow : Form
  {
    public MainWindow()
    {
      InitializeComponent();
    }

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

            var lvis = new List<ListViewItem>();

            foreach (var responsePair in mergedFile.EqualResponses)
            {
              var first = responsePair.Item1;
              var second = responsePair.Item2;

              var lvi = new ListViewItem(first.QuestEditorId);
              lvi.SubItems.Add(first.TopicEditorId);
              lvi.SubItems.Add(first.ResponseNumber.ToString());
              lvi.SubItems.Add(first.InfoFormId);
              lvi.SubItems.Add(first.ResponseText);
              lvi.SubItems.Add(second.QuestEditorId);
              lvi.SubItems.Add(second.TopicEditorId);
              lvi.SubItems.Add(second.ResponseNumber.ToString());
              lvi.SubItems.Add(second.InfoFormId);
              lvi.SubItems.Add(second.ResponseText);

              lvis.Add(lvi);
            }

            foreach (var response in mergedFile.UnmatchedFromFirst)
            {
              var lvi = new ListViewItem(response.QuestEditorId);
              lvi.SubItems.Add(response.TopicEditorId);
              lvi.SubItems.Add(response.ResponseNumber.ToString());
              lvi.SubItems.Add(response.InfoFormId);
              lvi.SubItems.Add(response.ResponseText);
              lvi.SubItems.Add(string.Empty);
              lvi.SubItems.Add(string.Empty);
              lvi.SubItems.Add(string.Empty);
              lvi.SubItems.Add(string.Empty);
              lvi.SubItems.Add(string.Empty);

              lvis.Add(lvi);
            }

            foreach (var response in mergedFile.UnmatchedFromSecond)
            {
              var lvi = new ListViewItem(string.Empty);
              lvi.SubItems.Add(string.Empty);
              lvi.SubItems.Add(string.Empty);
              lvi.SubItems.Add(string.Empty);
              lvi.SubItems.Add(string.Empty);
              lvi.SubItems.Add(response.QuestEditorId);
              lvi.SubItems.Add(response.TopicEditorId);
              lvi.SubItems.Add(response.ResponseNumber.ToString());
              lvi.SubItems.Add(response.InfoFormId);
              lvi.SubItems.Add(response.ResponseText);

              lvis.Add(lvi);
            }

            var ordered = lvis
              .OrderBy(lvi => lvi.Text)
              .ThenBy(lvi => lvi.SubItems[4].Text);

            listView1.BeginUpdate();

            foreach (var lvi in ordered)
            {
              listView1.Items.Add(lvi);
            }

            listView1.EndUpdate();
          }
          catch (Exception ex)
          {
            MessageBox.Show(ex.ToString());
          }

          //try
          //{
          //  var dialDumpFile = DialDumpParser.Parse(openFileDialog.FileName);

          //  foreach (var response in dialDumpFile.Responses)
          //  {
          //    var lvi = new ListViewItem(response.QuestEditorId);
          //    lvi.SubItems.Add(response.ResponseNumber.ToString());
          //    lvi.SubItems.Add(response.InfoFormId);
          //    lvi.SubItems.Add(response.ResponseText);

          //    listView1.Items.Add(lvi);
          //  }
          //}
          //catch (Exception ex)
          //{
          //  MessageBox.Show(ex.ToString());
          //}
        }
      }
    }
  }
}
