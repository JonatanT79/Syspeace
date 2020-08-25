using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using static Syspeace.Constants;

namespace Syspeace
{
    class Program
    {
        static void Main(string[] args)
        {
            var ObservationList = ReadTextFile(ObservationObject.FilePath);

            foreach (var observation in ObservationList)
            {
                Console.WriteLine(observation.TimeSpan + " " + observation.SessionID + " " + observation.Outcome + " " + observation.Username + " " + observation.IPAddress);
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
                bool ValidRow = false;

                if (ColumnArray.Length == 4)
                {
                    ValidRow = ObservationObject.ValidInputCheck(ColumnArray);
                }

                if (LogRow.Contains(Connect) && ValidRow)
                {
                    if (ColumnArray[SessionIDColumn] != SessionID)
                    {
                        ObservationObject.SearchForIDInLogRow(LogRow);
                        SessionID = ColumnArray[SessionIDColumn];
                    }
                }
            }

            return ObservationObject.ObservationList;
        }
    }
}

//Format:
// Tid - ID - Status - Användarnamn - IP Adress
//försök att använda path parametern
//Skriv kod för att få med datumet från 'written at date' och lägga in det i 'data' propertyn
//Försök att ersätta 3 i regex.match -> lägga in det i en variabel
//Fixa connect buggen när koden körs mot magnus fil