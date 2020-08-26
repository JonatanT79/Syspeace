using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using static Syspeace.Constants;

namespace Syspeace
{
    class ObservationObject
    {
        public static List<Observation> ObservationList = new List<Observation>();
        public static List<Observation> ReadTextFile(string FilePath)
        {
            string[] TextFile = File.ReadAllLines(FilePath);
            DateTime LogDate = new DateTime();
            string SessionID = "";

            for (int i = 0; i < TextFile.Length; i++)
            {
                string LogRow = TextFile[i];
                var ColumnArray = LogRow.Split("\t");
                bool ValidRow = false;

                ValidRow = Validation.ValidInputCheck(ColumnArray);

                if (ColumnArray.Length == 1 && LogDate == new DateTime())
                {
                    string StringDate = Values.GetValueFromRegexMatch(LogRow, Date);
                    if (DateTime.TryParse(StringDate, out LogDate)) { }
                }

                if (LogRow.Contains(Connect) && ValidRow)
                {
                    if (ColumnArray[SessionIDColumn] != SessionID)
                    {
                        SearchForIDInLogRow(LogRow, LogDate, TextFile);
                        SessionID = ColumnArray[SessionIDColumn];
                    }
                }
            }
            return ObservationList;
        }
        public static void SearchForIDInLogRow(string Logrow, DateTime LogDate, string[] TextFile)
        {
            string ID = Values.GetValueFromRegexMatch(Logrow, SessionID);
            SearchForSpecificIDInTextFile(ID, LogDate, TextFile);
        }
        public static void SearchForSpecificIDInTextFile(string ID, DateTime LogDate, string[] TextFile)
        {
            Observation _observation = new Observation();
            string IPAddress = "";
            int AuthCount = 0;
            bool CorrectPreviousTime = false;
            DateTime PreviousLineTime = new DateTime();
            DateTime NextLineTime = new DateTime();

            foreach (var LogRow in TextFile)
            {
                bool ValidRow = false;
                var Columns = LogRow.Split("\t");

                ValidRow = Validation.ValidInputCheck(Columns);

                if (ValidRow)
                {
                    if (CorrectPreviousTime == false)
                    {
                        PreviousLineTime = DateTime.Parse(Columns[TimeSpanColumn]);
                        NextLineTime = PreviousLineTime.AddHours(24);
                    }
                    else if (CorrectPreviousTime == true)
                    {
                        NextLineTime = DateTime.Parse(Columns[TimeSpanColumn]);
                    }

                    if (LogRow.Contains("\t" + ID + "\t") && LogRow.Contains(Connect))
                    {
                        IPAddress = Values.GetValueFromRegexMatch(LogRow, Connect);
                    }
                    _observation.Date = LogDate;
                    _observation = Values.GiveObservationValues(ID, LogRow, _observation, AuthCount, IPAddress, PreviousLineTime, NextLineTime);

                    if (LogRow.Contains("\t" + ID + "\t") && (LogRow.Contains(Success) || LogRow.Contains(Fail)))
                    {
                        AuthCount++;
                    }

                    if (CorrectPreviousTime == true)
                    {
                        PreviousLineTime = NextLineTime;
                    }
                    else if (CorrectPreviousTime == false)
                    {
                        CorrectPreviousTime = true;
                    }
                }
            }
        }
    }
}
