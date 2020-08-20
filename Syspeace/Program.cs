using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Syspeace
{
    class Program
    {
        static void Main(string[] args)
        {
            var ObservationList = ReadTextFile(ObservationObject.TextFile);

            foreach (var item in ObservationList)
            {
                Console.WriteLine(item.TimeStamp + " " + item.SectionID + "  " + item.Outcome + " " + item.Username + " " + item.IPAddress);
            }
        }
        static List<Observation> ReadTextFile(string FilePath)
        {
            for (int i = 0; i < ObservationObject.TextList.Length; i++)
            {
                string LogRow = ObservationObject.TextList[i];

                if (LogRow.Contains("\tCONNECT"))
                {
                    ObservationObject.SearchForIDInLogRow(LogRow);
                }
            }

            return ObservationObject.ObservationList;
        }
    }
}

// Tid - ID - Status - Användarnamn - IP Adress
