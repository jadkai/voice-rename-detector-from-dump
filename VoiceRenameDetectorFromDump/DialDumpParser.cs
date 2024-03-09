using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceRenameDetectorFromDump
{
  public static class DialDumpParser
  {
    public static DialDumpFile Parse(string path)
    {
      using var file = File.OpenText(path);
      int recordHeaderLevel = 0;
      bool isFileTypeKnown = false;
      bool isSkyrim = false;

      while (!file.EndOfStream && !isFileTypeKnown)
      {
        var line = file.ReadLine();

        if (line != null)
        {
          var trimmedLine = line.TrimStart();
          int curLevel = line.TakeWhile(c => char.IsWhiteSpace(c)).Count();

          if (trimmedLine.StartsWith("Record Header"))
          {
            recordHeaderLevel = curLevel;
          }
          else if (recordHeaderLevel > 0)
          {
            if (curLevel <= recordHeaderLevel)
            {
              isFileTypeKnown = true;
            }
            else if (trimmedLine.StartsWith("Form Version"))
            {
              isSkyrim = true;
              isFileTypeKnown = true;
            }
          }
        }
      }

      if (isFileTypeKnown)
      {
        if (isSkyrim)
        {
          return ParseSkyrim(file);
        }
        else
        {
          return ParseOblivion(file);
        }
      }
      else
      {
        throw new Exception("Couldn't determine file type");
      }
    }

    private static DialDumpFile ParseOblivion(StreamReader file)
    {
      int infoLevel = 0;
      int responseLevel = 0;
      string questEditorId = string.Empty;
      string topicEditorId = string.Empty;
      string infoFormId = string.Empty;
      var curResponse = null as Response;
      var dialDumpFile = new DialDumpFile();

      while (!file.EndOfStream)
      {
        var line = file.ReadLine();

        if (line != null)
        {
          var trimmedLine = line.TrimStart();
          int curLevel = line.TakeWhile(c => char.IsWhiteSpace(c)).Count();

          if (trimmedLine.StartsWith("INFO - Dialog response"))
          {
            infoLevel = curLevel;
            int startIndex = "INFO - Dialog response [".Length;
            int afterIndex = trimmedLine.IndexOf(']', startIndex);
            infoFormId = "00" + trimmedLine.Substring(startIndex + 2, afterIndex - startIndex - 2);
          }
          else if (infoLevel > 0)
          {
            if (curLevel <= responseLevel)
            {
              if (curResponse != null)
              {
                dialDumpFile.AddResponse(curResponse);
              }
              else
              {
                throw new ArgumentException("Response is null");
              }

              responseLevel = 0;
              curResponse = null;
            }

            if (curLevel <= infoLevel || trimmedLine.StartsWith("Result Script"))
            {
              infoLevel = 0;
            }
            else if (trimmedLine.StartsWith("Topic: DIAL"))
            {
              int startIndex = "Topic: DIAL - Dialog Topic [00000000] <".Length;
              int afterIndex = trimmedLine.IndexOf('>', startIndex);
              topicEditorId = trimmedLine.Substring(startIndex, afterIndex - startIndex);
            }
            else if (trimmedLine.StartsWith("QSTI"))
            {
              int startIndex = "QSTI - Quest: QUST - Quest [00010602] <".Length;
              int afterIndex = trimmedLine.IndexOf('>', startIndex);
              questEditorId = trimmedLine.Substring(startIndex, afterIndex - startIndex);
            }
            else if (trimmedLine == "Response")
            {
              responseLevel = curLevel;
              
              curResponse = new()
              {
                QuestEditorId = questEditorId,
                TopicEditorId = topicEditorId,
                InfoFormId = infoFormId
              };
            }
            else if (trimmedLine.StartsWith("Response number"))
            {
              curResponse!.ResponseNumber = int.Parse(trimmedLine.Substring("Response number: ".Length));
            }
            else if (trimmedLine.StartsWith("NAM1"))
            {
              curResponse!.ResponseText = trimmedLine.Substring("NAM1 - Response Text: ".Length);
            }
          }
        }
      }

      if (curResponse != null)
      {
        dialDumpFile.AddResponse(curResponse);
      }

      return dialDumpFile;
    }

    private static DialDumpFile ParseSkyrim(StreamReader file)
    {
      int dialLevel = 0;
      int infoLevel = 0;
      int responseLevel = 0;
      string questEditorId = string.Empty;
      string topicEditorId = string.Empty;
      string infoFormId = string.Empty;
      var curResponse = null as Response;
      var dialDumpFile = new DialDumpFile();

      while (!file.EndOfStream)
      {
        var line = file.ReadLine();

        if (line != null)
        {
          var trimmedLine = line.TrimStart();
          int curLevel = line.TakeWhile(c => char.IsWhiteSpace(c)).Count();

          if (trimmedLine.StartsWith("DIAL - Dialog Topic"))
          {
            dialLevel = curLevel;
          }
          else if (dialLevel > 0)
          {
            if (trimmedLine.StartsWith("QNAM"))
            {
              int startIndex = "QNAM - Quest: QUST - Quest [00010602] <".Length;
              int afterIndex = trimmedLine.IndexOf('>', startIndex);
              questEditorId = trimmedLine.Substring(startIndex, afterIndex - startIndex);
            }
            else if (trimmedLine.StartsWith("INFO - Dialog response"))
            {
              infoLevel = curLevel;
              int startIndex = "INFO - Dialog response [".Length;
              int afterIndex = trimmedLine.IndexOf(']', startIndex);
              infoFormId = "00" + trimmedLine.Substring(startIndex + 2, afterIndex - startIndex - 2);
            }
            else if (infoLevel > 0)
            {
              if (curLevel <= responseLevel)
              {
                if (curResponse != null)
                {
                  dialDumpFile.AddResponse(curResponse);
                }
                else
                {
                  throw new ArgumentException("Response is null");
                }

                responseLevel = 0;
                curResponse = null;
              }

              if (curLevel <= infoLevel || trimmedLine.StartsWith("Result Script"))
              {
                infoLevel = 0;
              }
              else if (trimmedLine.StartsWith("DNAM"))
              {
                // This means the INFO is using a SharedInfo for its responses,
                // which isn't interesting for rename detection. Skip it.
                infoLevel = 0;
                curResponse = null;
              }
              else if (trimmedLine.StartsWith("Topic: DIAL"))
              {
                if (trimmedLine.Length == "Topic: DIAL - Dialog Topic [01210CCB]".Length)
                {
                  // This is an unnamed topic. This can happen in scenes.
                  topicEditorId = string.Empty;
                }
                else
                {
                  int startIndex = "Topic: DIAL - Dialog Topic [01210CCB] <".Length;
                  int afterIndex = trimmedLine.IndexOf('>', startIndex);
                  topicEditorId = trimmedLine.Substring(startIndex, afterIndex - startIndex);
                }
              }
              else if (trimmedLine == "Response")
              {
                responseLevel = curLevel;

                curResponse = new Response()
                {
                  QuestEditorId = questEditorId,
                  TopicEditorId = topicEditorId,
                  InfoFormId = infoFormId
                };
              }
              else if (trimmedLine.StartsWith("Response number"))
              {
                curResponse!.ResponseNumber = int.Parse(trimmedLine.Substring("Response number: ".Length));
              }
              else if (trimmedLine.StartsWith("NAM1"))
              {
                curResponse!.ResponseText = trimmedLine.Substring("NAM1 - Response Text: ".Length);
              }
            }
          }
        }
      }

      if (curResponse != null)
      {
        dialDumpFile.AddResponse(curResponse);
      }

      return dialDumpFile;
    }
  }
}
 