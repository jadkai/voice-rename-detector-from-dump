using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceRenameDetectorFromDump
{
  /// <summary>
  /// Represents a single voice line.
  /// </summary>
  /// 
  /// <remarks>
  /// This is named "response" because that's how they are named in the ESM/ESP
  /// file format. A DIAL (topic) contains multiple INFOs, each of which
  /// contains the set of dialogue lines that an Actor says in response to a
  /// dialogue prompt. Each individual dialogue line is called a "response",
  /// and each of these corresponds to an audio file containing the audio for
  /// that response. A response can have multiple audio files if there are
  /// multiple voice types able to say the line, but each of those files will
  /// be named the same in the appropriate directory for each voice type.
  /// </remarks>
  public class Response
  {
    /// <summary>
    /// The editor ID of the quest that owns the INFO that this voice line
    /// appears in. This can be an empty string.
    /// </summary>
    public string QuestEditorId { get; set; }

    /// <summary>
    /// The editor ID of the topic (DIAL) that owns the INFO that this voice
    /// line appears in. This can be an empty string.
    /// </summary>
    public string TopicEditorId { get; set; }

    /// <summary>
    /// The response number of the voice line. Each line of dialogue in an INFO
    /// gets a response number that is something like the order in which the
    /// line of dialogue was added to the INFO.
    /// </summary>
    /// 
    /// <remarks>
    /// The response number is NOT the index of the response in the INFO, so it
    /// can actually be completely out of order with respect to the order in
    /// which an Actor will actually *say* each response.
    /// </remarks>
    public int ResponseNumber { get; set; }

    /// <summary>
    /// The form ID of the INFO that owns this voice line.
    /// </summary>
    public string InfoFormId { get; set; }

    /// <summary>
    /// The text of the dialogue for this voice line. In some cases, this
    /// string can be entirely whitespace.
    /// </summary>
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
