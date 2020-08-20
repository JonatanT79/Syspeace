using System;
using System.Collections.Generic;
using System.Text;

namespace Syspeace
{
    class Observation
    {
        public int SessionID { get; set; }
        public string IPAddress { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Outcome { get; set; }
        public string Username { get; set; }
    }
}
