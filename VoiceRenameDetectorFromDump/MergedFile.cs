using Quickenshtein;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceRenameDetectorFromDump
{
  /// <summary>
  /// Represents the matching data for the voice lines from the two input files
  /// for this program. This object records which voice lines have been matched
  /// and which have yet to be matched.
  /// </summary>
  public class MergedFile
  {
    /// <summary>
    /// Contains <see cref="Response"/> pairs that are determined to be the same
    /// voice line. As lines are matched by any means, they should move from the
    /// other collections to this one.
    /// </summary>
    /// 
    /// <remarks>
    /// The relationship between the voice lines in the first and second files
    /// is 1:many. Multiple matches for the same line in the first file just
    /// means reused audio. Mutliple matches for the same line in the second
    /// file would mean ambiguity as to the correct audio to use.
    /// </remarks>
    public List<(Response, Response)> EqualResponses { get; set; }

    /// <summary>
    /// Contains <see cref="Response"/> pairs that have a good chance of being
    /// equal because the edit distance between their text is within the
    /// threshold, and there was no other <see cref="Response"/> within the
    /// same minimal edit distance. The third value in the tuple is the edit
    /// distance between the text.
    /// </summary>
    /// 
    /// <remarks>
    /// As the name implies, these are *maybe* equal responses. It's entirely
    /// possible that a <see cref="Response"/> with a larger edit distance may
    /// be the correct match. This collection represents a "best guess" at
    /// matching when there is no exact text match for a voice line. These need
    /// to be resolved manually but can provide a decent guess at the right
    /// answer.
    /// </remarks>
    public List<(Response, Response, int)> MaybeEqualResponses { get; set; }

    /// <summary>
    /// Contains <see cref="Response"/> objects that have ambiguous matches.
    /// The first item in each pair in this collection is the source
    /// <see cref="Response"/>. The second item is a collection of possible
    /// matches for that voice line.
    /// </summary>
    /// 
    /// <remarks>
    /// This doesn't actually appear to be used anywhere. I think the above does
    /// correctly reflect what this is *intended* to be. The items in this
    /// collection would also be probably matches but would require manual
    /// selection of the correct match.
    /// </remarks>
    public List<(Response, List<Response>)> AmbiguousMatches { get; set; }

    /// <summary>
    /// Contains <see cref="Response"/> objects that came from the first input
    /// file for which no match in the second file has yet been found.
    /// </summary>
    /// 
    /// <remarks>
    /// The relationship between the first and second file is 1:many. A value
    /// in the first file that is unmatched means there is no voice line in the
    /// second file that will use its audio. It is possible for this to occur in
    /// the final data set if a voice line contains errors and won't be used,
    /// for example.
    /// </remarks>
    public List<Response> UnmatchedFromFirst { get; set; }

    /// <summary>
    /// Contains <see cref="Response"/> objects that came from the second input
    /// file for which no match in the first file has yet been found.
    /// </summary>
    /// 
    /// <remarks>
    /// The relationship between the first and second file is 1:many. A value
    /// in the second file that is unmatched means that the voice line will not
    /// have audio. It is still possible for this to be valid, but generally
    /// only in the case of a voice line consisting of a single space that is
    /// there to compensate for Oblivion/Skyrim dialogue system differences. A
    /// non-whitespace voice line from the second file with no match will be
    /// unvoiced and is a likely error.
    /// </remarks>
    public List<Response> UnmatchedFromSecond { get; set; }

    /// <summary>
    /// Ctor
    /// </summary>
    public MergedFile()
    {
      EqualResponses = [];
      MaybeEqualResponses = [];
      AmbiguousMatches = [];
      UnmatchedFromFirst = [];
      UnmatchedFromSecond = [];
    }

    /// <summary>
    /// Performs automatic processing on two <see cref="DialDumpFile"/>s to
    /// match up voice lines as much as possible without manual intervention.
    /// </summary>
    /// 
    /// <param name="firstFile">
    /// The first <see cref="DialDumpFile"/> to process. This should contain
    /// data from the Oblivion side for Skyblivion. Must not be null.
    /// </param>
    /// 
    /// <param name="secondFile">
    /// The second <see cref="DialDumpFile"/> to process. This should contain
    /// data from the Skyblivion side for Skyblivion. Must not be null.
    /// </param>
    /// 
    /// <returns>
    /// A <see cref="MergedFile"/> with as many automatic matches performed as
    /// possible. From here, the data should be presented to the user of this
    /// tool for resolving the remaining matches manually. Never returns null.
    /// </returns>
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
