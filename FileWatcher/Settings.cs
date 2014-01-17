using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace PgpFileWatcher
{
    class Settings
    {
        public string WatcherPath { get; set; }
        public string OutputPath { get; set; }
        public string PgpPath { get; set; }
        public string PublicKeyPath { get; set; }
        public string Recipient { get; set; }
        public string Filter { get; set; }
        public string FilterExclude { get; set; }
        public string TypeOfLogging { get; set; }

        public Settings()
        {
            WatcherPath = ConfigurationManager.AppSettings["WatcherPath"]; //?
            OutputPath = ConfigurationManager.AppSettings["OutputPath"]; //\\agdadrift.se\kunder\DK_3415\INTERFACE\OUT\ANNUAL_SALARY\WORK
            PgpPath = ConfigurationManager.AppSettings["PgpPath"]; //C:\Program Files\PGP Corporation\PGP Command Line
            PublicKeyPath = ConfigurationManager.AppSettings["PublicKeyPath"]; //D:\PGP\PGPhome
            Recipient = ConfigurationManager.AppSettings["Recipient"]; //0x5604C84A
            Filter = ConfigurationManager.AppSettings["Filter"]; //IN_SAP_HC_*.*
            FilterExclude = ConfigurationManager.AppSettings["FilterExclude"]; //*.pgp; *.tmp
            TypeOfLogging = ConfigurationManager.AppSettings["TypeOfLog"]; //*.pgp; *.tmp
        }

        public Settings(Settings fileWatcherSettings)
        {
            WatcherPath = fileWatcherSettings.WatcherPath;
            OutputPath = fileWatcherSettings.OutputPath;
            PgpPath = fileWatcherSettings.PgpPath;
            PublicKeyPath = fileWatcherSettings.PublicKeyPath;
            Recipient = fileWatcherSettings.Recipient;
            Filter = fileWatcherSettings.Filter;
            FilterExclude = fileWatcherSettings.FilterExclude;
            TypeOfLogging = fileWatcherSettings.TypeOfLogging;
        }

        internal void ReloadSettings()
        {
            ConfigurationManager.RefreshSection("appSettings");
            WatcherPath = ConfigurationManager.AppSettings["WatcherPath"]; //?
            OutputPath = ConfigurationManager.AppSettings["OutputPath"]; //\\agdadrift.se\kunder\DK_3415\INTERFACE\OUT\ANNUAL_SALARY\WORK
            PgpPath = ConfigurationManager.AppSettings["PgpPath"]; //C:\Program Files\PGP Corporation\PGP Command Line
            PublicKeyPath = ConfigurationManager.AppSettings["PublicKeyPath"]; //D:\PGP\PGPhome
            Recipient = ConfigurationManager.AppSettings["Recipient"]; //0x5604C84A
            Filter = ConfigurationManager.AppSettings["Filter"]; //IN_SAP_HC_*.*
            FilterExclude = ConfigurationManager.AppSettings["FilterExclude"]; //*.pgp; *.tmp
            TypeOfLogging = ConfigurationManager.AppSettings["TypeOfLog"]; //*.pgp; *.tmp
        }

        internal List<string> GetChanges(Settings oldFileWatcherSettings)
        {
            List<string> changes = new List<string>();
            if (this.WatcherPath != oldFileWatcherSettings.WatcherPath)
            {
                changes.Add(string.Format("WatcherPath changed from {0} to {1}", oldFileWatcherSettings.WatcherPath, this.WatcherPath));
            }

            if (this.OutputPath != oldFileWatcherSettings.OutputPath)
            {
                changes.Add(string.Format("OutputPath changed from {0} to {1}", oldFileWatcherSettings.OutputPath, this.OutputPath));
            }

            if (this.PgpPath != oldFileWatcherSettings.PgpPath)
            {
                changes.Add(string.Format("PgpPath changed from {0} to {1}", oldFileWatcherSettings.PgpPath, this.PgpPath));
            }

            if (this.PublicKeyPath != oldFileWatcherSettings.PublicKeyPath)
            {
                changes.Add(string.Format("PublicKeyPath changed from {0} to {1}", oldFileWatcherSettings.PublicKeyPath, this.PublicKeyPath));
            }

            if (this.Recipient != oldFileWatcherSettings.Recipient)
            {
                changes.Add(string.Format("Recipient changed from {0} to {1}", oldFileWatcherSettings.Recipient, this.Recipient));
            }

            if (this.Filter != oldFileWatcherSettings.Filter)
            {
                changes.Add(string.Format("Filter changed from {0} to {1}", oldFileWatcherSettings.Filter, this.Filter));
            }

            if (this.FilterExclude != oldFileWatcherSettings.FilterExclude)
            {
                changes.Add(string.Format("FilterExclude changed from {0} to {1}", oldFileWatcherSettings.FilterExclude, this.FilterExclude));
            }

            if (this.TypeOfLogging != oldFileWatcherSettings.TypeOfLogging)
            {
                changes.Add(string.Format("TypeOfLogging changed from {0} to {1}", oldFileWatcherSettings.TypeOfLogging, this.TypeOfLogging));
            }
            return changes;
        }
    }
}
