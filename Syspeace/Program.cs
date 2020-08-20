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
                Console.WriteLine(item.TimeStamp + " " + item.SessionID + "  " + item.Outcome + " " + item.Username + " " + item.IPAddress);
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
//prova ändra session id/saker i text filen på första connecten för att se om regex matchningen funkar
//Regex matchning på Event (Coonnect, Auth, Fail, Success) vid ifsatsen