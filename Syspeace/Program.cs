﻿using System;
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

        static List<Observation> FullObservation = new List<Observation>();
        static void Main(string[] args)
        {
            for (int i = 0; i < TextList.Length; i++)
            {
                string LogRow = TextList[i];

                if (LogRow.Contains("\tCONNECT"))
                {
                    ConnectSearch(LogRow);
                }
            }

            foreach (var item in FullObservation)
            {
                Console.WriteLine(item.SectionID + "  " + item.Outcome);
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
            Observation _observation = new Observation();
            foreach (var item in TextList)
            {
                if (item.Contains(ID) && item.Contains("\tCONNECT"))
                {
                    _observation.TimeStamp = DateTime.Parse(item.Substring(0, 8));
                    _observation.SectionID = int.Parse(ID);
                    _observation.IPAddress = item.Substring(21);
                }
                else if (item.Contains(ID) && item.Contains("\tAUTH"))
                {
                    _observation.Username = item.Substring(18);
                }
                else if (item.Contains(ID) && (item.Contains("\tSUCCESS") || item.Contains("\tFAIL")))
                {
                    int StopLenght = item.IndexOf("\t-");
                    _observation.Outcome = item.Substring(13, (StopLenght - 13));
                    FullObservation.Add(_observation);
                }
            }
        }
    }
}

// Tid - ID - Status - Användarnamn - IP Adress
//Bugg - får 13 observationen men får för många success och för få fail
//namngivningar
