using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Syspeace
{
    class Program
    {
        const string LoginFile = @"C:\Users\jonat\source\repos\Syspeace\Syspeace\Data\LoginFile.txt";
        static string[] TextList = File.ReadAllLines(LoginFile);

        static List<string> SortedLogsByID = new List<string>();
        static List<string> ObservationSort = new List<string>();
        static List<string> FullObservation = new List<string>();
        static void Main(string[] args)
        {
            for (int i = 0; i < TextList.Length; i++)
            {
                string LogRow = TextList[i];

                if (LogRow.Contains("\tCONNECT"))
                {
                    ConnectSearch(LogRow);
                }

                //  Console.WriteLine(LogRow);
            }

            ObservationSort = SortedLogsByID;
            foreach (var item in ObservationSort)
            {
                Console.WriteLine(item);
            }
        }
        static void ConnectSearch(string Logrow)
        {
            Match match = Regex.Match(Logrow, @"\b\d...\b");
            string ID = match.Value;
            SearchLogByID(ID);
        }
        static void SearchLogByID(string ID)
        {
            foreach (var item in TextList)
            {
                if (item.Contains(ID))
                {
                    SortedLogsByID.Add(item);
                }
            }
        }
    }
}

// Tid - ID - Status - Användarnamn - IP Adress