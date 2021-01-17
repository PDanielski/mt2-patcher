using Metin2Warlords.Patcher.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Metin2Warlords.Patcher
{
    public partial class PatcherWindow : Window
    {
        private ObsoleteFileFinder ObsoleteFileFinder { get; set; }
        private List<PatchFile> ObsoleteFiles { get; set; } = new List<PatchFile>();
        private List<PatchFile> RemoteFiles { get; set; } = new List<PatchFile>();
        public WebClient Client { get; set; }

        public PatcherWindow(WebClient client, ObsoleteFileFinder obsoleteFileFinder, List<PatchFile> remoteFiles, List<PatchFile> obsoleteFiles)
        {
            InitializeComponent();
            Client = client;
            ObsoleteFileFinder = obsoleteFileFinder;
            RemoteFiles = remoteFiles;
            ObsoleteFiles = obsoleteFiles;

            startButton.Click += startButtonClick;
            repairButton.Click += repairButtonClick;
            registerButton.Click += registerButtonClick;
            closeButton.Click += closeButtonClick;
            minifyButton.Click += minButtonClick;
            
            this.MouseDown += OnMouseDown;

            Initialize();
        }

        public void StartUpdateIfNeeded()
        {
            if(ObsoleteFiles.Count > 0)
            {
                repairButton.IsEnabled = false;
                _updateObsoleteFiles();
            }
        }

        public void Initialize()
        {
            if(ObsoleteFiles != null && ObsoleteFiles.Count > 0)
            {
                startButton.IsEnabled = false;
                repairButton.IsEnabled = false;
                
            }
        }

        void startButtonClick(object sender, RoutedEventArgs e)
        {
            var metin2Client = new Process();
            metin2Client.StartInfo.FileName = ConfigurationManager.AppSettings["metin2ExeFileName"] ;
            metin2Client.Start();
            Application.Current.Shutdown();  
        }

        public async void repairButtonClick(object sender, RoutedEventArgs e)
        {
            repairButton.IsEnabled = false;
            startButton.IsEnabled = false;
            ObsoleteFiles = await ObsoleteFileFinder.SearchObsoleteAsync(RemoteFiles);
            if(ObsoleteFiles.Count > 0)
            {
                startButton.IsEnabled = false;
                _updateObsoleteFiles();
            } else
            {
                repairButton.IsEnabled = true;
                startButton.IsEnabled = true;
            }
        }

        void registerButtonClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(ConfigurationManager.AppSettings["registrationURL"]);
        }

        void closeButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("Sei sicuro?", "Conferma", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        void minButtonClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }


        private void _updateObsoleteFiles()
        {
            HTTPDownloader downloader = new HTTPDownloader(ObsoleteFiles, Client,"source/");
            DownloadListener listener = new DownloadListener(this, downloader);
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (object s, DoWorkEventArgs e) =>
            {

                downloader.startDownload();
            };
            bw.RunWorkerAsync();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

    }
}
