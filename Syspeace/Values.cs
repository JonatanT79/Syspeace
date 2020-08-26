using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static Syspeace.Constants;

namespace Syspeace
{
    class Values
    {
        public static Observation GiveObservationValues(string ID, string LogRow, Observation observation, int AuthCount, string IPAddress, DateTime PreviousLineTime, DateTime NextLineTime)
        {
            string IDWithTabs = "\t" + ID + "\t";

            if (PreviousLineTime <= NextLineTime)
            {
                if (LogRow.Contains(IDWithTabs) && LogRow.Contains(Connect))
                {
                    observation.SessionID = int.Parse(ID);
                    observation.IPAddress = IPAddress;
                }
                else if (LogRow.Contains(IDWithTabs) && LogRow.Contains(Auth))
                {
                    if (AuthCount > 0)
                    {
                        observation = new Observation();
                    }
                    observation.Username = GetValueFromRegexMatch(LogRow, Auth);
                }
                else if (LogRow.Contains(IDWithTabs) && (LogRow.Contains(Success) || LogRow.Contains(Fail)))
                {
                    observation.TimeSpan = DateTime.Parse(GetValueFromRegexMatch(LogRow, Time));

                    if (LogRow.Contains(Success))
                    {
                        observation.Outcome = GetValueFromRegexMatch(LogRow, Success);
                    }
                    else
                    {
                        observation.Outcome = GetValueFromRegexMatch(LogRow, Fail);
                    }

                    if (AuthCount > 0)
                    {
                        observation.SessionID = int.Parse(ID);
                        observation.IPAddress = IPAddress;
                    }

                    bool ObservationHasAllValues = Validation.SearchNullValuesInObservation(observation);

                    if (ObservationHasAllValues == true)
                    {
                        ObservationObject.ObservationList.Add(observation);
                    }
                }
            }
            return observation;
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
            else if (Outcome == Date)
            {
                Regex _regex = new Regex(@"\b....-..-..\b");
                Match match = _regex.Match(LogLine);
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
    }
}
