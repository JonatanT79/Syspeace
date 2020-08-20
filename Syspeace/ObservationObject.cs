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
            //Alternativ2
            //var Column4ArrayTest = Logrow.Split("\t");
            //string SessionID = Column4ArrayTest[1];
            //Match match1 = Regex.Match(SessionID, @"\b\d+\b");
            ////Checkar om regex matching fungerar
            //Console.WriteLine(match1 + " match1");
            //Console.ReadLine();

            //ALternativ1
            Regex _regex = new Regex(@"\d+");
            Match match = _regex.Match(Logrow, 9, Logrow.Length);
            string ID = match.Value;
            Console.WriteLine(ID + " match");
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
                    // IPAddress = item.Substring(21);
                    _observation.IPAddress = IPAddress;
                }
                else if (item.Contains(ID) && item.Contains("\tAUTH"))
                {
                    if (Count > 0)
                    {
                        _observation = new Observation();
                    }
                    _observation.Username = item.Substring(18);
                }
                else if (item.Contains(ID) && (item.Contains("\tSUCCESS") || item.Contains("\tFAIL")))
                {
                    int StopLenght = item.IndexOf("\t-");
                    _observation.TimeStamp = DateTime.Parse(item.Substring(0, 8));
                    _observation.Outcome = item.Substring(13, (StopLenght - 13));

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

            if (Outcome == "CONNECT")
            {
                Regex _regex = new Regex(@".*");
                Match match = _regex.Match(LogLine, StartAt + 2, Lenght);
                return match.Value;
            }
            else if (Outcome == "AUTH")
            {
                Regex _regex = new Regex(".*");
                Match match = _regex.Match(LogLine);
                return match.Value;
            }
            else if (Outcome == "SUCCESS" || Outcome == "FAIL")
            {
                Regex _regex = new Regex(".*");
                Match match = _regex.Match(LogLine);
                return match.Value;
            }

            return "";
        }
    }
}
