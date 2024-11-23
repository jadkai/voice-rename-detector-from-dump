using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VoiceRenameDetectorFromDump
{
  public static class DialDumpParser
  {
    /// <summary>
    /// Regex to match a form ID - an 8 character string of hex digits.
    /// </summary>
    private static readonly Regex formIdRegex = new Regex(
      @"[0-9A-Fa-f]{8}",
      RegexOptions.Singleline);

    /// <summary>
    /// Regex to match the start of an INFO header line. These lines start with
    /// either "[INFO" or the editor ID of the INFO in quotes followed by that.
    /// </summary>
    private static readonly Regex infoLineStartRegex = new Regex(
      @"^(\""[^\""]*\"" )?\[INFO",
      RegexOptions.Singleline);

    /// <summary>
    /// Regex to match the portion of the INFO header line that contains the
    /// details about the DIAL that the INFO belongs to. This starts with either
    /// "[DIAL" or the editor ID of the DIAL in quotes followed by that.
    /// </summary>
    private static readonly Regex dialRegex = new Regex(
      @"(\""[^\""]*\"" )?\[DIAL",
      RegexOptions.Singleline);

    /// <summary>
    /// Regex to match the portion of the INFO header line that contains the
    /// details about the QUST that the INFO belongs to. This starts with either
    /// "[QUST" or the editor ID of the QUST in quotes followed by that.
    /// </summary>
    private static readonly Regex qustRegex = new Regex(
      @"(\""[^\""]*\"" )?\[QUST",
      RegexOptions.Singleline);

    /// <summary>
    /// 
    /// </summary>
    private static readonly Regex responseLineRegex = new Regex(
      @"[0-9]+:",
      RegexOptions.Singleline);

    /// <summary>
    /// Parses the specified file to read the INFO response data.
    /// </summary>
    /// 
    /// <param name="path">
    /// The path to the file to parse. Must not be null.
    /// </param>
    /// 
    /// <returns>
    /// A <see cref="DialDumpFile"/> containing the parsed data. Never returns
    /// null.
    /// </returns>
    /// 
    /// <exception cref="Exception">
    /// Thrown if an error occurs while parsing.
    /// </exception>
    public static DialDumpFile Parse(string path)
    {
      var lines = File.ReadAllLines(path);
      var dialDumpFile = new DialDumpFile();

      var infoGroups = GetInfoGroups(lines);

      foreach (var infoGroup in infoGroups)
      {
        var (infoFormId, topicEditorId, questEditorId) = ParseInfoHeader(infoGroup[0]);

        var responses = infoGroup
          .Skip(1) // first line in group is the header
          .Select(line => line.TrimStart())
          .Where(line => !string.IsNullOrEmpty(line)) // skip blank lines
          .Select(responseLine =>
          {
            if (!responseLineRegex.IsMatch(responseLine))
            {
              throw new Exception($"Unexpected format in response line: {responseLine}");
            }

            int firstQuoteIndex = responseLine.IndexOf('"');
            int lastQuoteIndex = responseLine.LastIndexOf('"');
            var responseText = responseLine[(firstQuoteIndex + 1)..lastQuoteIndex];
            var responseNumber = int.Parse(responseLine[0..responseLine.IndexOf(':')]);

            return new Response
            {
              ResponseNumber = responseNumber,
              InfoFormId = infoFormId,
              QuestEditorId = questEditorId,
              ResponseText = responseText,
              TopicEditorId = topicEditorId,
            };
          })
          .ToList(); // going to enumerate more than once

        if (!responses.Any())
        {
          throw new Exception($"Found empty INFO: {infoFormId}");
        }

        dialDumpFile.AddResponses(responses);
      }

      return dialDumpFile;
    }

    /// <summary>
    /// Given all the lines in a file, separates and returns the data for each
    /// INFO record described by the file.
    /// </summary>
    /// 
    /// <param name="lines">
    /// The lines in the file. Must not be null.
    /// </param>
    /// 
    /// <returns>
    /// A <see cref="List{T}"/> where each entry is an array of strings. In
    /// each of the arrays, the first line is the introducing line for the INFO
    /// record that contains the details about the INFO, the DIAL it belongs to,
    /// and the QUST it belongs to. The remaining entries in the array are the
    /// lines that contain the individual responses that make up the INFO. The
    /// returned list could be empty but will never be null.
    /// </returns>
    /// 
    /// <exception cref="Exception">
    /// Thrown if an error occurs processing the file.
    /// </exception>
    private static List<string[]> GetInfoGroups(string[] lines)
    {
      int nextGroupStartIndex = -1;
      int prevGroupStartIndex = -1;
      List<string[]> infoGroups = [];

      int lastIndex = lines.Length - 1;

      for (int i = 0; i < lines.Length; ++i)
      {
        var line = lines[i];
        var trimmedLine = line.TrimStart();
        bool atEndOfGroup = false;

        if (infoLineStartRegex.IsMatch(trimmedLine))
        {
          prevGroupStartIndex = nextGroupStartIndex;
          nextGroupStartIndex = i;
          atEndOfGroup = true;
        }
        else if (i == lastIndex)
        {
          prevGroupStartIndex = nextGroupStartIndex;
          nextGroupStartIndex = lines.Length;
          atEndOfGroup = true;
        }

        if (atEndOfGroup && prevGroupStartIndex != -1)
        {
          infoGroups.Add(lines[prevGroupStartIndex..nextGroupStartIndex]);
        }
      }

      if (nextGroupStartIndex == lastIndex)
      {
        throw new Exception(
          "Encountered INFO block at end of file with no responses");
      }

      return infoGroups;
    }

    /// <summary>
    /// Given the first line in an INFO group, returns the form ID of the INFO
    /// record, the editor ID of the DIAL record, and the editor ID of the QUST
    /// record associated with the INFO.
    /// </summary>
    /// 
    /// <param name="header">
    /// A string containing the header text. Must not be null.
    /// </param>
    /// 
    /// <returns>
    /// A tuple containing the form ID of the INFO, the editor ID of the DIAL,
    /// and the editor ID of the QUST. The INFO form ID will never be null or
    /// empty. The editor IDs could be empty but never null.
    /// </returns>
    /// 
    /// <exception cref="Exception">
    /// Thrown if an error occurs parsing the line.
    /// </exception>
    private static (string infoFormId, string topicEditorId, string questEditorId) ParseInfoHeader(string header)
    {
      // There can be an editor ID for the INFO at the beginning of the line.
      // We don't care about that, so just jump right to the "[INFO" part,
      // because that's where the form ID that we do care about is located.
      var infoLeadIn = "[INFO ";
      int infoFormIdIndex = header.IndexOf(infoLeadIn) + infoLeadIn.Length + 2;
      var infoFormId = "00" + header[infoFormIdIndex..(infoFormIdIndex + 6)];

      if (!formIdRegex.IsMatch(infoFormId))
      {
        throw new Exception($"Bad form ID for INFO: {infoFormId}");
      }

      var dialSectionMatch = dialRegex.Match(header);

      if (!dialSectionMatch.Success)
      {
        throw new Exception($"Couldn't find DIAL section in string: {header}");
      }

      var dialIndex = dialSectionMatch.Index;

      // The DIAL might not have an editor ID - that's not an error.
      var topicEditorId =
        header[dialIndex] == '"' ? header[(dialIndex + 1)..header.IndexOf('"', dialIndex + 1)] :
        string.Empty;

      var qustSectionMatch = qustRegex.Match(header);

      if (!qustSectionMatch.Success)
      {
        throw new Exception($"Couldn't find QUST section in string: {header}");
      }

      var qustIndex = qustSectionMatch.Index;

      // It's not an error if the QUST doesn't have an editor ID.
      var questEditorId =
        header[qustIndex] == '"' ? header[(qustIndex + 1)..header.IndexOf('"', qustIndex + 1)] :
        string.Empty;

      return (infoFormId, topicEditorId, questEditorId);
    }
  }
}
 