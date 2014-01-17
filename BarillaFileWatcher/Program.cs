using PgpFileWatcher;
using System;
using System.Configuration;
using System.Threading;

namespace BarillaFileWatcherConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Class1 c1 = new Class1();

            while (true)
            {
                Thread.Sleep(10000);
                c1.RealoadWatcher();
            }
        }
    }
}
