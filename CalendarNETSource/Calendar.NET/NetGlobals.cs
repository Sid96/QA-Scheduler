using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calendar.NET
{    
    public class NetGlobals
    {
        public static List<IEvent> customEvents = new List<IEvent> ();
        public static int docCounter = 0;
        public static string path = @"TEMPDATA•0.txt";
        public static string[] names;
        public static List<IEvent> tempEvents = new List<IEvent>();
    }
}
