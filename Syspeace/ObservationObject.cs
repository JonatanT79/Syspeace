using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Syspeace
{
    static class ObservationObject
    {
        const string LoginFile = @"C:\Users\jonat\source\repos\Syspeace\Syspeace\Data\LoginFile.txt";
        public static string[] TextList = File.ReadAllLines(LoginFile);

        public static List<Observation> ObservationList = new List<Observation>();

        public static void SearchForIDInLogRow(string Logrow)
        {
            Match match = Regex.Match(Logrow, @"\b\d...\b");
            string ID = match.Value;
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
                    _observation.SectionID = int.Parse(ID);
                    IPAddress = item.Substring(21);
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
                        _observation.SectionID = int.Parse(ID);
                        _observation.IPAddress = IPAddress;
                    }

                    ObservationList.Add(_observation);
                    Count++;
                }
            }
        }
    }
}
