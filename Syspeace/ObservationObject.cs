using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using static Syspeace.Constants;

namespace Syspeace
{
    class ObservationObject
    {
        public static List<Observation> ObservationList = new List<Observation>();
        public static void SearchForIDInLogRow(string Logrow, string[] TextFile)
        {
            string ID = GetValueFromRegexMatch(Logrow, SessionID);
            MakeLogsToObservationObject(ID, TextFile);
        }
        public static void MakeLogsToObservationObject(string ID, string[] TextFile)
        {
            Observation _observation = new Observation();
            string IPAddress = "";
            string IDWithTabs = "\t" + ID + "\t";
            int AuthCount = 0;
            bool CorrectPreviousTime = false;
            DateTime PreviousLineTime = new DateTime();
            DateTime NextLineTime = new DateTime();

            foreach (var LogRow in TextFile)
            {
                bool ValidRow = false;
                var Columns = LogRow.Split("\t");

                if (Columns.Length == 4)
                {
                    ValidRow = ValidInputCheck(Columns);
                }

                if (ValidRow)
                {
                    if (CorrectPreviousTime == false)
                    {
                        PreviousLineTime = DateTime.Parse(Columns[TimeSpanColumn]);
                        NextLineTime = PreviousLineTime.AddHours(24);
                    }
                    else if (CorrectPreviousTime == true)
                    {
                        NextLineTime = DateTime.Parse(Columns[TimeSpanColumn]);
                    }

                    if (PreviousLineTime <= NextLineTime)
                    {
                        if (LogRow.Contains(IDWithTabs) && LogRow.Contains(Connect))
                        {
                            _observation.SessionID = int.Parse(ID);
                            IPAddress = GetValueFromRegexMatch(LogRow, Connect);
                            _observation.IPAddress = IPAddress;
                        }
                        else if (LogRow.Contains(IDWithTabs) && LogRow.Contains(Auth))
                        {
                            if (AuthCount > 0)
                            {
                                _observation = new Observation();
                            }
                            _observation.Username = GetValueFromRegexMatch(LogRow, Auth);
                        }
                        else if (LogRow.Contains(IDWithTabs) && (LogRow.Contains(Success) || LogRow.Contains(Fail)))
                        {
                            _observation.TimeSpan = DateTime.Parse(GetValueFromRegexMatch(LogRow, Time));

                            if (LogRow.Contains(Success))
                            {
                                _observation.Outcome = GetValueFromRegexMatch(LogRow, Success);
                            }
                            else
                            {
                                _observation.Outcome = GetValueFromRegexMatch(LogRow, Fail);
                            }

                            if (AuthCount > 0)
                            {
                                _observation.SessionID = int.Parse(ID);
                                _observation.IPAddress = IPAddress;
                            }

                            bool ObservationHasAllValues = SearchNullValuesInObservation(_observation);

                            if (ObservationHasAllValues == true)
                            {
                                ObservationList.Add(_observation);
                            }

                            AuthCount++;
                        }
                    }

                    if (CorrectPreviousTime == true)
                    {
                        PreviousLineTime = NextLineTime;
                    }
                    else if (CorrectPreviousTime == false)
                    {
                        CorrectPreviousTime = true;
                    }
                }
            }
        }
        public static string GetValueFromRegexMatch(string LogLine, string Outcome)
        {
            int Index = LogLine.IndexOf(Outcome);
            int StartAt = Index + Outcome.Length;
            int Lenght = LogLine.Length - StartAt;

            if (Outcome == SessionID)
            {
                Index = LogLine.IndexOf(Connect);
                StartAt = 9;
                Lenght = Index - StartAt;
                Regex _regex = new Regex(@"\d+");
                Match match = _regex.Match(LogLine, StartAt, Lenght);
                return match.Value;
            }
            else if (Outcome == Time)
            {
                Regex _regex = new Regex(@"..:..:..\t");
                Match match = _regex.Match(LogLine);
                return match.Value;
            }
            else if (Outcome == Connect)
            {
                Regex _regex = new Regex(@".*");
                Match match = _regex.Match(LogLine, StartAt, Lenght);
                return match.Value;
            }
            else if (Outcome == Auth)
            {
                Regex _regex = new Regex(".*");
                Match match = _regex.Match(LogLine, StartAt, Lenght);
                return match.Value;
            }
            else if (Outcome == Success || Outcome == Fail)
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
                observation.TimeSpan == null ||
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
                DateTime.TryParse(input[TimeSpanColumn], out DateTime datetime) &&
                int.TryParse(input[SessionIDColumn], out int ID) &&
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
