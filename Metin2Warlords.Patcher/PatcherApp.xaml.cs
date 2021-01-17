using Metin2Warlords.Patcher.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Metin2Warlords.Patcher
{
    public partial class PatcherApp : Application
    {
        private const string VersionFileName = "AutoUpdate";
        private const string RemoteFileFolder = "source";
        private const string RemoteFileManifestPath = "metaFiles/Index";

        private WebClient Client { get; set; }
        private PipeDelimitedFileManifestParser FileManifestParser { get; set; } = new PipeDelimitedFileManifestParser();
        private ObsoleteFileFinder ObsoleteFileFinder { get; set; } = new ObsoleteFileFinder();

        protected async override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

            StopIfApplicationAlreadyRunning();

#if !DEBUG
            StopIfArgumentDoesNotMatch();
#endif

            PreloadWindow preloadWindow = new PreloadWindow();
            preloadWindow.Show();

            InitializeWebClient();

            var remotePatchFiles = GetRemotePatchFiles();
            var obsoletePatchFiles = new List<PatchFile>();
            if(IsUpdateRequired(remotePatchFiles))
            {
                preloadWindow.ShowUpdate();
                obsoletePatchFiles = await SearchForObsoletePatchFiles(remotePatchFiles);
            }

            preloadWindow.Hide();

            PatcherWindow patcherWindow = new PatcherWindow(Client, ObsoleteFileFinder, remotePatchFiles, obsoletePatchFiles);   
            patcherWindow.Show();
            patcherWindow.StartUpdateIfNeeded();

        }

        private void StopIfApplicationAlreadyRunning()
        {
            var mutexKey = ConfigurationManager.AppSettings["startMutexKey"];
            if (mutexKey == null)
                return;

            var mutex = new Mutex(false, mutexKey);
            if (!mutex.WaitOne(0, false))
                Application.Current.Shutdown();
            
        }

        private void StopIfArgumentDoesNotMatch()
        {
            var requiredArgument = ConfigurationManager.AppSettings["startupRequiredArgument"];
            if (requiredArgument == null)
                return;

            var args = Environment.GetCommandLineArgs();
            if (args.Length != 2 || args[1] != requiredArgument) 
                Application.Current.Shutdown();
        }

        private void InitializeWebClient()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            Client = new WebClient();
            Client.BaseAddress = ConfigurationManager.AppSettings["remoteServerURL"];
        }

        private List<PatchFile> GetRemotePatchFiles()
        {
            var fileManifest = Client.DownloadString(RemoteFileManifestPath);
            return FileManifestParser.Parse(fileManifest);
        }

        private bool IsUpdateRequired(List<PatchFile> remotePatchFiles)
        {
            var versionPatchFile = remotePatchFiles.Find(f => f.Name == VersionFileName);
            if (versionPatchFile == null)
                return true;
            return !ObsoleteFileFinder.IsFileUpToDate(versionPatchFile);

        }

        private async Task<List<PatchFile>> SearchForObsoletePatchFiles(List<PatchFile> remotePatchFiles)
        {
            return await ObsoleteFileFinder.SearchObsoleteAsync(remotePatchFiles);
        }

        private void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            MessageBox.Show("Critical error: a log file has been created!");
            System.IO.StreamWriter file = new System.IO.StreamWriter("patcherError+" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".txt", true);
            string s = e.ExceptionObject.ToString();
            file.WriteLine(s);

            file.Close();
        }
    }



}
