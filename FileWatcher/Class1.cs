using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace PgpFileWatcher
{
    public class Class1
    {
        private FileSystemWatcher watcher;

        Settings fileWatcherSettings;
        Settings oldFileWatcherSettings;




        public Class1()
        {
            fileWatcherSettings = new Settings();
            oldFileWatcherSettings = new Settings();

            RegisterKeyToKeyChain();

            watcher = new FileSystemWatcher();
            watcher.Path = fileWatcherSettings.WatcherPath;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                   | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = fileWatcherSettings.Filter;
            watcher.Created += watcher_Created;
            watcher.EnableRaisingEvents = true;

            this.RealoadWatcher();
        }

        private void RegisterKeyToKeyChain()
        {
            string s = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).ToString();
            string argument = string.Format(" --import \"{0}\" --manual-import-key-pairs public", Path.Combine(s, "PublicKey").ToString() + "\\public_key.asc");

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "\"" + fileWatcherSettings.PgpPath + "\\pgp.exe\"";
            startInfo.Arguments = argument;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;

            WriteLogPost(startInfo.FileName + argument, "info");

            Process p = Process.Start(startInfo);

            while (!p.StandardError.EndOfStream)
            {
                string line = p.StandardError.ReadLine();
                if (line.Contains("0:output"))
                    WriteLogPost(line, "info");
                else
                    WriteLogPost(line, "error");
            }

            p.WaitForExit();
        }

        void watcher_Created(object sender, FileSystemEventArgs e)
        {
            PgpEncryption(e);
        }

        private void PgpEncryption(FileSystemEventArgs e)
        {
            try
            {
                string ext = Path.GetExtension(e.FullPath);
                List<string> blackList = new List<string>(fileWatcherSettings.FilterExclude.Split(';'));

                if (blackList.Contains(ext.ToLower()))
                    return;

                string filename = Path.GetFileNameWithoutExtension(e.FullPath);

                string newFileName = string.Format("{0}\\{1}{2}", fileWatcherSettings.OutputPath, filename + BarillaDateTime(DateTime.Now), ".pgp");

                if (File.Exists(newFileName))
                {
                    try
                    {
                        File.Move(newFileName, string.Format("{0}\\{1}{2}{3}", fileWatcherSettings.OutputPath, filename + BarillaDateTime(DateTime.Now), Guid.NewGuid().ToString(), ".pgp"));
                    }
                    catch (Exception ex)
                    {
                        WriteLogPost(ex.Message, "error");
                    }

                }

                string argument = string.Format(" -e \"{0}\" --recipient {1} --output \"{2}\"", e.FullPath, fileWatcherSettings.Recipient, newFileName);

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "\"" + fileWatcherSettings.PgpPath + "\\pgp.exe\"";
                startInfo.Arguments = argument;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.CreateNoWindow = true;

                WriteLogPost(startInfo.FileName + argument, "info");

                Process p = Process.Start(startInfo);

                while (!p.StandardError.EndOfStream)
                {
                    string line = p.StandardError.ReadLine();
                    if (line.Contains("0:output"))
                        WriteLogPost(line, "info");
                    else
                        WriteLogPost(line, "error");
                }

                p.WaitForExit();
            }
            catch (Exception ex)
            {
                WriteLogPost(ex.Message, "error");
            }
        }

        private string BarillaDateTime(DateTime date)
        {
            string month = date.Month < 10 ? "0" + date.Month : date.Month.ToString();
            string day = date.Day < 10 ? "0" + date.Day : date.Day.ToString();
            return string.Format("{0}{1}{2}", date.Year, month, day);
        }

        public void RealoadWatcher()
        {
            try
            {
                fileWatcherSettings.ReloadSettings();
                List<string> changes = fileWatcherSettings.GetChanges(oldFileWatcherSettings);

                if (changes.Count > 0)
                {
                    oldFileWatcherSettings = new Settings(fileWatcherSettings);

                    WriteLogPost(string.Format("Reloading watcher {0}\\{1}", fileWatcherSettings.WatcherPath, fileWatcherSettings.Filter), "info");

                    foreach (string s in changes)
                    {
                        WriteLogPost(s, "info");
                    }

                    watcher.Path = fileWatcherSettings.WatcherPath;
                    watcher.Filter = fileWatcherSettings.Filter;
                }

            }
            catch (Exception ex)
            {
                WriteLogPost(ex.Message, "error");
            }
        }

        private void WriteLogPost(string message, string type)
        {
            switch (fileWatcherSettings.TypeOfLogging.ToUpper())
            {
                case "EVENTLOG":
                    {
                        switch (type.ToUpper())
                        {
                            case "INFO":
                                EventLog.WriteEntry("Application", message, EventLogEntryType.Information);
                                break;
                            case "ERROR":
                                EventLog.WriteEntry("Application", message, EventLogEntryType.Error);
                                break;
                            case "WARN":
                                EventLog.WriteEntry("Application", message, EventLogEntryType.Warning);
                                break;
                            default:
                                EventLog.WriteEntry("Application", message, EventLogEntryType.Information);
                                break;
                        }
                        break;
                    }

                case "FILE":
                    {
                        string logFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).ToString();
                        string logFile = logFilePath + "\\" + "Log.txt";
                        try
                        {
                            FileInfo info = new FileInfo(logFile);

                            if (info.Length > 100 * 1024 * 1024)
                            {
                                File.Move(logFile, logFilePath + "\\" + "Log" + BarillaDateTime(info.CreationTime) + ".txt");
                            }
                        }
                        catch (Exception)
                        {
                        }

                        File.AppendAllLines(logFile, new string[] { DateTime.Now.ToString() + "\t" + message });
                        break;
                    }
                case "CONSOLE":
                    {
                        Console.WriteLine(message);
                        break;
                    }
            }
        }
    }
}
