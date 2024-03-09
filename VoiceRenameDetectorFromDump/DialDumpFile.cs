using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceRenameDetectorFromDump
{
  public class DialDumpFile
  {
    public List<Response> Responses { get; set; }
    public Dictionary<string, Response> ResponsesByInfoId { get; set; }
    public Dictionary<string, List<Response>> ResponsesByText { get; set; }

    public DialDumpFile()
    {
      Responses = [];
      ResponsesByInfoId = [];
      ResponsesByText = [];
    }

    public void AddResponse(Response response)
    {
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
  }
}
