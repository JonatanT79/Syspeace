using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Syspeace
{
    static class ObservationObject
    {
        public const string FilePath = @"C:\Users\jonat\source\repos\Syspeace\Syspeace\Data\LoginFile.txt";
        public static string[] TextFile = File.ReadAllLines(FilePath);
        public static List<Observation> ObservationList = new List<Observation>();

        public static void SearchForIDInLogRow(string Logrow)
        {
            string ID = GetValueFromRegexMatch(Logrow, "SessionID");
            if (int.TryParse(ID, out int SessionID))
            {
                MakeLogsToObservationObject(ID);
            }
        }
        public static void MakeLogsToObservationObject(string ID)
        {
            Observation _observation = new Observation();
            string IPAddress = "";
            DateTime PreviousLineTime = DateTime.Now;
            DateTime NextLineTime = DateTime.Now.AddMinutes(60);
            int Count = 0;
            int TimeCounter = 0;

            foreach (var item in TextFile)
            {
                if (!string.IsNullOrWhiteSpace(item) && TimeCounter == 0)
                {
                    PreviousLineTime = DateTime.Parse(GetValueFromRegexMatch(item, "Time"));
                }
                else if (!string.IsNullOrWhiteSpace(item) && TimeCounter > 0)
                {
                    NextLineTime = DateTime.Parse(GetValueFromRegexMatch(item, "Time"));
                }

                if (PreviousLineTime < NextLineTime || TimeCounter == 0)
                {
                    if (item.Contains("\t" + ID) && item.Contains("\tCONNECT"))
                    {
                        _observation.SessionID = int.Parse(ID);
                        IPAddress = GetValueFromRegexMatch(item, "CONNECT");
                        _observation.IPAddress = IPAddress;
                    }
                    else if (item.Contains("\t" + ID) && item.Contains("\tAUTH"))
                    {
                        if (Count > 0)
                        {
                            _observation = new Observation();
                        }
                        _observation.Username = GetValueFromRegexMatch(item, "AUTH");
                    }
                    else if (item.Contains("\t" + ID) && (item.Contains("\tSUCCESS") || item.Contains("\tFAIL")))
                    {
                        _observation.TimeStamp = DateTime.Parse(GetValueFromRegexMatch(item, "Time"));

                        if (item.Contains("\tSUCCESS"))
                        {
                            _observation.Outcome = GetValueFromRegexMatch(item, "SUCCESS");
                        }
                        else
                        {
                            _observation.Outcome = GetValueFromRegexMatch(item, "FAIL");
                        }

                        if (Count > 0)
                        {
                            _observation.SessionID = int.Parse(ID);
                            _observation.IPAddress = IPAddress;
                        }

                        ObservationList.Add(_observation);
                        Count++;
                    }

                    if (!string.IsNullOrWhiteSpace(item) && TimeCounter != 0)
                    {
                        PreviousLineTime = NextLineTime;
                    }
                    else if(!string.IsNullOrWhiteSpace(item) && TimeCounter == 0)
                    {
                        TimeCounter++;
                    }
                }
            }
        }
        public static string GetValueFromRegexMatch(string LogLine, string Outcome)
        {
            int Index = LogLine.IndexOf("\t" + Outcome);
            int StartAt = Index + Outcome.Length;
            int Lenght = LogLine.Length - (StartAt + 2);

            if (Outcome == "SessionID")
            {
                Index = LogLine.IndexOf("\tCONNECT");
                Lenght = Index - 9;
                Regex _regex = new Regex(@"\d+");
                Match match = _regex.Match(LogLine, 9, Lenght);
                return match.Value;
            }
            else if (Outcome == "Time")
            {
                Regex _regex = new Regex(@"..:..:..");
                Match match = _regex.Match(LogLine);
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
