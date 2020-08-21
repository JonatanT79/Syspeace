using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Syspeace
{
    static class ObservationObject
    {
        public const string TextFile = @"C:\Users\jonat\source\repos\Syspeace\Syspeace\Data\LoginFile.txt";
        public static string[] TextList = File.ReadAllLines(TextFile);
        public static List<Observation> ObservationList = new List<Observation>();

        public static void SearchForIDInLogRow(string Logrow)
        {
            string ID = GetIndexOfRegexMatch(Logrow, "SessionID");
            MakeLogsToObservationObject(ID);
        }
        public static void MakeLogsToObservationObject(string ID)
        {
            Observation _observation = new Observation();
            string IPAddress = "";
            int Count = 0;
            foreach (var item in TextList)
            {
                if (item.Contains(ID) && item.Contains("\tCONNECT"))
                {
                    _observation.SessionID = int.Parse(ID);
                    IPAddress = GetIndexOfRegexMatch(item, "CONNECT");
                    _observation.IPAddress = IPAddress;
                }
                else if (item.Contains(ID) && item.Contains("\tAUTH"))
                {
                    if (Count > 0)
                    {
                        _observation = new Observation();
                    }
                    _observation.Username = GetIndexOfRegexMatch(item, "AUTH");
                }
                else if (item.Contains(ID) && (item.Contains("\tSUCCESS") || item.Contains("\tFAIL")))
                {
                    _observation.TimeStamp = DateTime.Parse(item.Substring(0, 8));

                    if (item.Contains("\tSUCCESS"))
                    {
                        _observation.Outcome = GetIndexOfRegexMatch(item, "SUCCESS");
                    }
                    else
                    {
                        _observation.Outcome = GetIndexOfRegexMatch(item, "FAIL");
                    }

                    if (Count > 0)
                    {
                        _observation.SessionID = int.Parse(ID);
                        _observation.IPAddress = IPAddress;
                    }

                    ObservationList.Add(_observation);
                    Count++;
                }
            }
        }
        public static string GetIndexOfRegexMatch(string LogLine, string Outcome)
        {
            int Index = LogLine.IndexOf("\t" + Outcome);
            int StartAt = Index + Outcome.Length;
            int Lenght = LogLine.Length - (StartAt + 2);

            if (Outcome == "SessionID")
            {
                Regex _regex = new Regex(@"\d+");
                Match match = _regex.Match(LogLine, 9, LogLine.Length);
                return match.Value;
            }
            else if (Outcome == "CONNECT")
            {
                Regex _regex = new Regex(@".*");
                Match match = _regex.Match(LogLine, StartAt + 2, Lenght);
                return match.Value;
            }
            else if (Outcome == "AUTH")
            {
                Regex _regex = new Regex(".*");
                Match match = _regex.Match(LogLine, StartAt + 2, Lenght);
                return match.Value;
            }
            else if (Outcome == "SUCCESS" || Outcome == "FAIL")
            {
                Lenght = LogLine.Length - (Index + 3);
                Regex _regex = new Regex(".*");
                Match match = _regex.Match(LogLine, Index + 1, Lenght);
                return match.Value;
            }

            return "";
        }
    }
}
