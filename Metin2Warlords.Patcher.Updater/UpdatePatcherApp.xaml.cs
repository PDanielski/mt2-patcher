using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Metin2Warlords.Patcher.Common;

namespace Metin2Warlords.Patcher.Updater
{
    public partial class UpdatePatcherApp : Application
    {
        Mutex m;
        protected override void OnStartup(StartupEventArgs e)
        {

            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
 
             m = new Mutex(false, "rom2updater");
            if (!m.WaitOne(0, false))
            {
                Application.Current.Shutdown();
            }
            if (IsProcessOpen("warlords.patcher.exe"))
            {
                Application.Current.Shutdown();
            }

            //Da fare gestione config
            string updateIndexFileName = "metaFiles/UpdaterIndex";
            string cleanerFileName = "metaFiles/UpdateCleaner";
            string baseAddress = "http://patcher.metin2warlords.net/";
            string argument = "nonhofantasia";

                UpdateWindow window = new UpdateWindow();
                window.Show();
                WebClient client = new WebClient();
                client.BaseAddress = baseAddress;

                string s = client.DownloadString(updateIndexFileName);
                string c = client.DownloadString(cleanerFileName);


                CleanerExtractor ce = new CleanerExtractor(c);
                List<PatchFile> toCleanFile = ce.extract();

                foreach (PatchFile f in toCleanFile)
                {
                    string path = Path.Combine(f.BasePath, f.Name);
                    if (File.Exists(path))
                    {
                        File.SetAttributes(path, FileAttributes.Normal);
                        File.Delete(path);
                    }
                }
                PipeDelimitedFileManifestParser p = new PipeDelimitedFileManifestParser(s);
                ObsoleteFileFinder scanner = new ObsoleteFileFinder(p);

                scanner.Search();
                List<PatchFile> toUpdateFiles = scanner.getToUpdateFiles();
                List<PatchFile> sourceFiles = scanner.getSourceFiles();
                string exeName = sourceFiles[0].Name;
                BackgroundWorker bw = new BackgroundWorker();
                if (toUpdateFiles.Count > 0)
                {
                    HTTPDownloader downloader = new HTTPDownloader(toUpdateFiles, client, "sourceUpdater/");
                    UpdaterDownloadListener listener = new UpdaterDownloadListener(window, downloader);
                    downloader.allFileDownloaded += (object ss) =>
                    {
                        System.Diagnostics.Process.Start(exeName, argument);
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            window.Close();
                        }
                        ));
                    };

                    bw.DoWork += (object se, DoWorkEventArgs es) =>
                    {
                        downloader.startDownload();
                    };
                    bw.RunWorkerAsync();

                }


                if (bw.IsBusy == false)
                {
                    System.Diagnostics.Process.Start(exeName, argument);
                    window.Close();
                }


        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            
            MessageBox.Show("Critical error: a log file has been created!");
            System.IO.StreamWriter file = new System.IO.StreamWriter("updaterError+"+DateTime.Now.ToString("yyyyMMddHHmmssffff")+".txt", true);
            string s = e.ExceptionObject.ToString();
            file.WriteLine(s);

            file.Close();
        }

        public bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
