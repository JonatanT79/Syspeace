using System;
using System.Collections.Generic;
using System.Text;
using static Syspeace.Constants;
namespace Syspeace
{
    class Validation
    {
        public static bool SearchNullValuesInObservation(Observation observation)
        {
            if (
                observation.SessionID == 0 ||
                observation.IPAddress == null ||
                observation.Username == null ||
                observation.Date == new DateTime() ||
                observation.TimeSpan == new DateTime() ||
                observation.Outcome == null
            )
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool ValidInputCheck(string[] input)
        {
            if (input.Length == 4)
            {
                if (
                    DateTime.TryParse(input[TimeSpanColumn], out DateTime datetime) &&
                    int.TryParse(input[SessionIDColumn], out int ID) &&
                    ID >= 1
                )
                {
                    return true;
                }
            }
            return false;
        }
        public static void PrintOutObservationList(List<Observation> ObservationList)
        {
            foreach (var observation in ObservationList)
            {
                Console.WriteLine(observation.Date.ToString("yyyy-MM-dd") + " " + observation.TimeSpan.ToString("HH:mm:ss") + " " +
                observation.SessionID + " " + observation.Outcome + " " + observation.Username + " " + observation.IPAddress);
            }

            Console.WriteLine("");
            Console.WriteLine("Observation Count: " + ObservationList.Count);
        }
    }
}
