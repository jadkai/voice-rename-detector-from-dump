using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceRenameDetectorFromDump
{
  public class Response
  {
    public string QuestEditorId { get; set; }
    public string TopicEditorId { get; set; }
    public int ResponseNumber { get; set; }
    public string InfoFormId { get; set; }
    public string ResponseText { get; set; }

    public Response()
    {
      QuestEditorId = string.Empty;
      TopicEditorId = string.Empty;
      ResponseNumber = 0;
      InfoFormId = string.Empty;
      ResponseText = string.Empty;
    }
  }
}
