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
                bool ValidRow = false;

                if(ColumnArray.Length == 4)
                {
                    ValidRow = ObservationObject.ValidInputCheck(ColumnArray);
                }

                if (LogRow.Contains("\tCONNECT\t") && ValidRow)
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
//lägg till using static constants för att slippa skriva 'Constants.Connect'
// - Lägga till konstanter och t.ex lägga '1' i index(program) i en variabel som t.ex heter sesionID
//försök att använda path parametern
//Skriv kod för att få med datumet från 'written at date'
//Fixa connect buggen när koden körs mot magnus fil