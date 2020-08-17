using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Syspeace
{
    class Program
    {
        static void Main(string[] args)
        {
            const string LoginFile = @"C:\Users\jonat\source\repos\Syspeace\Syspeace\Data\LoginFile.txt";
            
            string[] TextList = File.ReadAllLines(LoginFile);
            List<string> ObservationList = new List<string>();
            int i = 1;
            foreach (var item in TextList)
            {
                Console.WriteLine(item);
                //if(i == 3)
                //{
                //    break;
                //}
                i++;
            }

        }
    }
}

// Tid - ID - Status - Användarnamn - IP Adress