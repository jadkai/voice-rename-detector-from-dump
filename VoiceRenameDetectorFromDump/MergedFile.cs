using Quickenshtein;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceRenameDetectorFromDump
{
  public class MergedFile
  {
    public List<(Response, Response)> EqualResponses { get; set; }
    public List<(Response, Response, int)> MaybeEqualResponses { get; set; }
    public List<(Response, List<Response>)> AmbiguousMatches { get; set; }
    public List<Response> UnmatchedFromFirst { get; set; }
    public List<Response> UnmatchedFromSecond { get; set; }

    public MergedFile()
    {
      EqualResponses = [];
      MaybeEqualResponses = [];
      AmbiguousMatches = [];
      UnmatchedFromFirst = [];
      UnmatchedFromSecond = [];
    }

    public static MergedFile CreateFromDumpFiles(DialDumpFile firstFile, DialDumpFile secondFile)
    {
      var mergedFile = new MergedFile();
      var matchedFromFirst = new HashSet<string>();
      var matchedFromSecond = new HashSet<string>();

      foreach (var response in firstFile.Responses)
      {
        bool isMatched = false;
        var infoIdAndNum = $"{response.InfoFormId}_{response.ResponseNumber}";

        if (secondFile.ResponsesByInfoId.TryGetValue(infoIdAndNum, out var secondResponse))
        {
          // There's an exact form ID match. See if the response number and
          // text are the same. If they are, this is a definite match.

          if (response.ResponseNumber == secondResponse.ResponseNumber)
          {
            if (response.ResponseText == secondResponse.ResponseText)
            {
              isMatched = true;
              mergedFile.EqualResponses.Add((response, secondResponse));
              matchedFromFirst.Add(infoIdAndNum);
              matchedFromSecond.Add(infoIdAndNum);
            }
          }
        }
        else if (secondFile.ResponsesByText.TryGetValue(response.ResponseText, out var secondResponses))
        {
          // There's at least one response from the second file that has the
          // same text as the response from the first file.

          if (secondResponses.Count == 1)
          {
            // There is exactly one response from the second file that has the
            // same text as the response from the first file. That's a match.

            isMatched = true;
            mergedFile.EqualResponses.Add((response, secondResponses[0]));
            matchedFromFirst.Add(infoIdAndNum);
            matchedFromSecond.Add($"{secondResponses[0].InfoFormId}_{secondResponses[0].ResponseNumber}");
          }
        }

        if (!isMatched)
        {
          mergedFile.UnmatchedFromFirst.Add(response);
        }
      }

      foreach (var response in secondFile.Responses)
      {
        var secondInfoIdAndNum = $"{response.InfoFormId}_{response.ResponseNumber}";

        if (!matchedFromSecond.Contains(secondInfoIdAndNum))
        {
          bool isMatched = false;

          if (firstFile.ResponsesByText.TryGetValue(response.ResponseText, out var firstResponses))
          {
            if (firstResponses.Count == 1)
            {
              // The first pass looking for the other side to have 1 match
              // catches 1:1 matches. This pass catches 1:many matches, so
              // the source should be definitely the same. It just got matched
              // to multiple things on the right (this happens if you copy voice
              // lines instead of just moving them).

              isMatched = true;

              var firstResponse = firstResponses[0];
              var firstInfoIdAndNum = $"{firstResponse.InfoFormId}_{firstResponse.ResponseNumber}";

              mergedFile.UnmatchedFromFirst.Remove(firstResponse);
              mergedFile.EqualResponses.Add((firstResponse, response));
              matchedFromFirst.Add(firstInfoIdAndNum);
              matchedFromSecond.Add(secondInfoIdAndNum);
            }
          }

          if (!isMatched)
          {
            // For any as-yet unmatched responses from the second file, go
            // through all the reponses from the first file and calculate the
            // Levenshtein distance between them. For any that are within a
            // given threshold, find the minimum Levenshtein distance. If
            // there is only one response matching that minimum Levenshtein
            // distance, then it is a probable but not definite match.

            var bestResponse = null as Response;
            var bestDistance = int.MaxValue;
            var isAmbiguous = false;

            foreach (var firstResponse in firstFile.Responses)
            {
              var responseText = firstResponse.ResponseText;
              var distance = Levenshtein.GetDistance(responseText, response.ResponseText);

              if (distance < 20)
              {
                if (distance < bestDistance)
                {
                  isAmbiguous = false;
                  bestResponse = firstResponse;
                  bestDistance = distance;
                }
                else if (distance == bestDistance)
                {
                  isAmbiguous = true;
                }
              }
            }

            if (bestResponse != null && !isAmbiguous)
            {
              mergedFile.MaybeEqualResponses.Add((bestResponse, response, bestDistance));
            }
          }

          if (!isMatched)
          {
            mergedFile.UnmatchedFromSecond.Add(response);
          }
        }
      }

      return mergedFile;
    }
  }
}
