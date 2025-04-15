using System.Data;

namespace VoiceRenameDetectorFromDump
{
    public partial class MaybeEqualResponseListWindow : Form
    {
        private readonly MergedFile mergedFile;
        public MaybeEqualResponseListWindow(MergedFile mergedFile)
        {
            InitializeComponent();
            this.mergedFile = mergedFile;
            var maybeEqualResponses = mergedFile.MaybeEqualResponses;
            int index = 0;
            foreach (var (firstResponse, secondResponse, distance) in maybeEqualResponses.OrderBy(r => r.Item3).ThenBy(r => r.Item1.InfoFormId))
            {
                AddResponse(distance, firstResponse, index);
                AddResponse(distance, secondResponse, index);
                index++;
            }
            Load += (_, _) =>
            {//Add this listener after Window Load so it's not called repeatedly while loading data.
                EqualResponseList.ItemChecked += (s, e) =>
                {
                    var checkedItem = e.Item;
                    string tag = (string)checkedItem.Tag!;
                    string relatedTag = maybeEqualResponses.Select(r =>
                    {
                        string tag1 = GetTag(r.Item3, r.Item1);
                        string tag2 = GetTag(r.Item3, r.Item2);
                        return tag1 == tag ? tag2 :
                           tag2 == tag ? tag1 :
                            null;
                    }).Where(x => x != null).Single()!;
                    var relatedItem = GetListViewItems([relatedTag]).Single();
                    if (relatedItem.Checked != checkedItem.Checked)
                    {
                        relatedItem.Checked = checkedItem.Checked;
                    }
                };
            };
        }

        private static string[] GetRowCells(int distance, Response response)
        {
            return ["", distance.ToString(), response.InfoFormId, response.TopicEditorId, response.QuestEditorId, response.ResponseText];
        }

        private static string GetTag(IEnumerable<string> rowCells)
        {
            return string.Join(",", rowCells);
        }
        private static string GetTag(int distance, Response response)
        {
            return GetTag(GetRowCells(distance, response));
        }

        private void AddResponse(int distance, Response response, int index)
        {
            string[] rowCells = GetRowCells(distance, response);
            var item = new ListViewItem(rowCells)
            {
                Tag = GetTag(rowCells)
            };
            if (index % 2 == 1) { item.BackColor = Color.WhiteSmoke; }
            EqualResponseList.Items.Add(item);
        }

        private IEnumerable<ListViewItem> GetListViewItems(IReadOnlyCollection<string> tags)
        {
            return EqualResponseList.Items.Cast<ListViewItem>().Where(liv => tags.Contains((string)liv.Tag!));
        }

        private static readonly string[] prefixes = ["TES4", "SKYB"];
        private void AutoCheck_Click(object sender, EventArgs e)
        {
            var likelyEqualResponses = mergedFile.MaybeEqualResponses.Where(r => r.Item3 == 1 && (r.Item1.TopicEditorId == "GREETING" || r.Item2.TopicEditorId == "" || prefixes.Any(p => p + r.Item1.TopicEditorId == r.Item2.TopicEditorId.Split('_')[0])) && prefixes.Any(p => p + r.Item1.QuestEditorId == r.Item2.QuestEditorId));
            var count = likelyEqualResponses.Count();
            foreach (var (firstResponse, secondResponse, distance) in likelyEqualResponses)
            {
                string[] tags = [GetTag(distance, firstResponse), GetTag(distance, secondResponse)];
                var itemsToCheck = GetListViewItems(tags);
                foreach (var item in itemsToCheck)
                {
                    item.Checked = true;
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            string[] tags = EqualResponseList.Items.Cast<ListViewItem>().Where(liv => liv.Checked).Select(liv => (string)liv.Tag!).ToArray();
            foreach (var (firstResponse, secondResponse, _) in mergedFile.MaybeEqualResponses.Where(r => tags.Contains(GetTag(r.Item3, r.Item1)) || tags.Contains(GetTag(r.Item3, r.Item2))))
            {
                mergedFile.EqualResponses.Add((firstResponse, secondResponse));
                mergedFile.UnmatchedFromFirst.Remove(firstResponse);
                mergedFile.UnmatchedFromSecond.Remove(secondResponse);
            }
            Close();
        }
    }
}
