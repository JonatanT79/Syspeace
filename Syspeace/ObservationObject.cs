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
            MakeLogsToObservationObject(ID);
        }
        public static void MakeLogsToObservationObject(string ID)
        {
            Observation _observation = new Observation();
            string IPAddress = "";
            string GetTime = "";
            DateTime PreviousLineTime = new DateTime();
            DateTime NextLineTime = new DateTime();
            int Count = 0;
            bool CorrectPreviousTime = false;

            //Valid Check
            foreach (var item in TextFile)
            {
                bool SuccessfulTimeParse = false;
                var Columns = item.Split("\t");


                if (CorrectPreviousTime == false)
                {
                    GetTime = GetValueFromRegexMatch(item, "Time");

                    if (DateTime.TryParse(GetTime, out DateTime PreviousTime))
                    {
                        PreviousLineTime = PreviousTime;
                        SuccessfulTimeParse = true;
                        NextLineTime = PreviousLineTime.AddHours(24);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(item) && CorrectPreviousTime == true)
                {
                    GetTime = GetValueFromRegexMatch(item, "Time");

                    if (DateTime.TryParse(GetTime, out DateTime NextTime))
                    {
                        NextLineTime = NextTime;
                        SuccessfulTimeParse = true;
                    }
                }

                if (PreviousLineTime <= NextLineTime && SuccessfulTimeParse == true)
                {
                    if (item.Contains("\t" + ID + "\t") && item.Contains("\tCONNECT\t"))
                    {
                        _observation.SessionID = int.Parse(ID);
                        IPAddress = GetValueFromRegexMatch(item, "CONNECT");
                        _observation.IPAddress = IPAddress;
                    }
                    else if (item.Contains("\t" + ID + "\t") && item.Contains("\tAUTH\t"))
                    {
                        if (Count > 0)
                        {
                            _observation = new Observation();
                        }
                        _observation.Username = GetValueFromRegexMatch(item, "AUTH");
                    }
                    else if (item.Contains("\t" + ID + "\t") && (item.Contains("\tSUCCESS\t") || item.Contains("\tFAIL\t")))
                    {
                        _observation.TimeStamp = DateTime.Parse(GetValueFromRegexMatch(item, "Time"));

                        if (item.Contains("\tSUCCESS\t"))
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

                        bool ObservationHasAllValues = SearchNullValuesInObservation(_observation);

                        if (ObservationHasAllValues == true)
                        {
                            ObservationList.Add(_observation);
                        }

                        Count++;
                    }
                }

                if (!string.IsNullOrWhiteSpace(item) && CorrectPreviousTime == true && SuccessfulTimeParse == true)
                {
                    PreviousLineTime = NextLineTime;
                }
                else if (DateTime.TryParse(GetTime, out DateTime Time))
                {
                    CorrectPreviousTime = true;
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
                Regex _regex = new Regex(@"..:..:..\t");
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
        public static bool SearchNullValuesInObservation(Observation observation)
        {
            if (
                observation.SessionID == 0 ||
                observation.IPAddress == null ||
                observation.Username == null ||
                observation.TimeStamp == null ||
                observation.Outcome == null
            )
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool ValidInputCheck(string[] input)
        {
            if (
                DateTime.TryParse(input[0], out DateTime datetime) &&
                int.TryParse(input[1], out int ID) &&
                ID >= 1
            )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
