using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClientCLI
{
    class Log
    {
        public static void Line(string l)
        {
            Console.WriteLine(l);
        }

        public static void Write(string l)
        {
            Console.Write(l);
        }
    }
}
