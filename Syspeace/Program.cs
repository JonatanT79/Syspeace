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
            var ObservationList = ObservationObject.ReadTextFile(FilePath);
            Validation.PrintOutObservationList(ObservationList);
        }
    }
}
