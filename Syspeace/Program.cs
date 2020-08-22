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
            var ObservationList = ReadTextFile(ObservationObject.FilePath);

            foreach (var item in ObservationList)
            {
                Console.WriteLine(item.TimeStamp + " " + item.SessionID + " " + item.Outcome + " " + item.Username + " " + item.IPAddress);
            }

            Console.WriteLine("");
            Console.WriteLine("Observation Count: " + ObservationList.Count);
        }
        static List<Observation> ReadTextFile(string FilePath)
        {
            string SessionID = "";

            for (int i = 0; i < ObservationObject.TextFile.Length; i++)
            {
                string LogRow = ObservationObject.TextFile[i];
                var ColumnArray = LogRow.Split("\t");

                if (LogRow.Contains("\tCONNECT"))
                {
                    if (ColumnArray[1] != SessionID)
                    {
                        ObservationObject.SearchForIDInLogRow(LogRow);
                        SessionID = ColumnArray[1];
                    }
                }
            }

            return ObservationObject.ObservationList;
        }
    }
}

//Format:
// Tid - ID - Status - Användarnamn - IP Adress

//Punkt 2.1 fungerar halvt (alla outcome får det sista fail/success)
//men användarnamnet blir rätt (alla får det senaste användarnamnet)
//Vid en andra connect med samma sessionID: rensa (ip) och ersätt med den nya (2:a connect)
//försök ta bort datetime decklaration, timecounter och isnullorwhitespace där nere (om möjligt) - Snygga till kod 