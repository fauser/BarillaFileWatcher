using PgpFileWatcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BarillaFileWatcherService
{
    public partial class BarillaFileWatcherService : ServiceBase
    {
        public BarillaFileWatcherService()
        {
            InitializeComponent();

            Thread thread = new Thread(new ThreadStart(WorkThreadFunction));
            thread.Start();
        }

        public static void WorkThreadFunction()
        {
            try
            {
                Class1 c1 = new Class1();
                while (true)
                {
                    Thread.Sleep(10000);
                    c1.RealoadWatcher();
                }
            }
            catch (Exception ex)
            {
                // log errors
            }
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }
    }
}
