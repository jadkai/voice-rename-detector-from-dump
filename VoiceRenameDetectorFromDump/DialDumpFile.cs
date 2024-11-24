using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceRenameDetectorFromDump
{
  /// <summary>
  /// Represents the file containing data about the DIAL and INFO records for
  /// an ESM or ESP.
  /// </summary>
  /// 
  /// <remarks>
  /// This was named because the input files were originally created using
  /// the dump utility that comes with xEdit, but now it's a custom format
  /// produced by xEdit itself. The meaning is still close enough, even if not
  /// technically correct.
  /// </remarks>
  public class DialDumpFile
  {
    /// <summary>
    /// All of the <see cref="Response"/> objects in a flat list.
    /// </summary>
    public List<Response> Responses { get; set; }

    /// <summary>
    /// All of the <see cref="Response"/> objects, keyed by the form ID of the
    /// INFO that they correspond to and the response number of the response.
    /// </summary>
    /// 
    /// <remarks>
    /// The format of the key is "<INFO form ID>_<response number>"; e.g.,
    /// "00123456_1" for the response with number 1 from INFO 00123456.
    /// </remarks>
    public Dictionary<string, Response> ResponsesByInfoId { get; set; }

    /// <summary>
    /// All of the <see cref="Response"/> objects, keyed by their text. Note
    /// that the same text can appear in multiple responses, so the value in the
    /// key-value pair is a <see cref="List{T}"/>.
    /// </summary>
    public Dictionary<string, List<Response>> ResponsesByText { get; set; }

    /// <summary>
    /// Ctor
    /// </summary>
    public DialDumpFile()
    {
      Responses = [];
      ResponsesByInfoId = [];
      ResponsesByText = [];
    }

    /// <summary>
    /// Adds a single <see cref="Response"/> object to this
    /// <see cref="DialDumpFile"/>.
    /// </summary>
    /// 
    /// <param name="response">
    /// The <see cref="Response"/> to add. Must not be null.
    /// </param>
    public void AddResponse(Response response)
    {
      ArgumentNullException.ThrowIfNull(response);

      Responses.Add(response);
      ResponsesByInfoId[$"{response.InfoFormId}_{response.ResponseNumber}"] = response;

      List<Response>? byTextResponses;

      if (!ResponsesByText.TryGetValue(response.ResponseText, out byTextResponses))
      {
        byTextResponses = [];
      }

      byTextResponses.Add(response);

      ResponsesByText[response.ResponseText] = byTextResponses;
    }

    /// <summary>
    /// Adds multiple <see cref="Response"/> objects to this
    /// <see cref="DialDumpFile"/>.
    /// </summary>
    /// 
    /// <param name="responses">
    /// A sequence of <see cref="Response"/> objects to add. Must not be null,
    /// and must not contain any nulls.
    /// </param>
    public void AddResponses(IEnumerable<Response> responses)
    {
      ArgumentNullException.ThrowIfNull(responses);
	
      foreach (Response response in responses)
      {
        AddResponse(response);
      }
    }
  }
}
