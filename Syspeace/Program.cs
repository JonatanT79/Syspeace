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
            var ObservationList = ReadTextFile(FilePath);
            ObservationObject.PrintOutObservationList(ObservationList);
        }
        static List<Observation> ReadTextFile(string FilePath)
        {
            string[] TextFile = File.ReadAllLines(FilePath);
            DateTime LogDate = new DateTime();
            string SessionID = "";

            for (int i = 0; i < TextFile.Length; i++)
            {
                string LogRow = TextFile[i];
                var ColumnArray = LogRow.Split("\t");
                bool ValidRow = false;

                ValidRow = ObservationObject.ValidInputCheck(ColumnArray);

                if (ColumnArray.Length == 1 && LogDate == new DateTime())
                {
                    string StringDate = ObservationObject.GetValueFromRegexMatch(LogRow, Date);
                    if (DateTime.TryParse(StringDate, out LogDate)) { }
                }

                if (LogRow.Contains(Connect) && ValidRow)
                {
                    if (ColumnArray[SessionIDColumn] != SessionID)
                    {
                        ObservationObject.SearchForIDInLogRow(LogRow, LogDate, TextFile);
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
//Överför funktioner till metoder
//Försök att ersätta 3 i regex.match -> lägga in det i en variabel
//Hitta ett sätt att få bort den globala listan(om möjligt)
//Fixa connect buggen när koden körs mot magnus fil