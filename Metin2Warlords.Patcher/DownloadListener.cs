using Metin2Warlords.Patcher.Common;
using System;
using System.Net;
using System.Windows;

namespace Metin2Warlords.Patcher
{
    public class DownloadListener
    {
        PatcherWindow window;
        HTTPDownloader downloader;

        int totalSize;
        int baseCurrentSize;
        int currentSize;

        DateTime lastUpdate;
        long lastBytes = 0;
        long bytesForSecond = 0;

        public DownloadListener(PatcherWindow w, HTTPDownloader d)
        {
            window = w;
            downloader = d;
            baseCurrentSize = 0;
            totalSize = d.getTotalSize();

            d.setProgressChangedEventHandler(new DownloadProgressChangedEventHandler(OnDownloadProgress));
            d.setAfterNewFileEventHandler(new AfterFileDownloadedEventHandler(OnAfterNewFile));
            d.setAllFileDownloadedEventHandler(new AllFileDownloadedEventHandler(allFileDownloaded));

        }

        protected void allFileDownloaded(object sender)
        {
            Application.Current.Dispatcher.Invoke((Action)(() => {
                window.startButton.IsEnabled = true;
                window.multiProgressBar.Value = 100;
            }));

        }
        protected void OnDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {

            if(lastBytes == 0)
            {
                lastBytes = e.BytesReceived;
                lastUpdate = DateTime.Now;
            } else
            {
                TimeSpan changedTime = DateTime.Now - lastUpdate;
                if (changedTime.Milliseconds > 500)
                {
                    long changedBytes = e.BytesReceived - lastBytes;
                    bytesForSecond = changedBytes / changedTime.Milliseconds;
                    lastBytes = e.BytesReceived;
                    lastUpdate = DateTime.Now;
                }
            }
            

            currentSize = baseCurrentSize + (int)e.BytesReceived;
            if (totalSize - currentSize < 100) {
                int i = 2;
            }
            double globalPercentage = ((double)currentSize / (double)totalSize) * 100;
            string text = String.Format("{0} kB of {1}, {2} kB/s, {3}%", e.BytesReceived/1000, e.TotalBytesToReceive/1000, bytesForSecond, e.ProgressPercentage);
            Application.Current.Dispatcher.Invoke((Action)(() => {
                window.progressStatus.Text = text;
                window.multiProgressBar.Value = globalPercentage;
                window.globalPercentage.Text = ((int)globalPercentage).ToString() + "%";
            }));

        }
        protected void OnAfterNewFile(object sender, AfterFileDownloadedEventArgs e)
        {
            baseCurrentSize = currentSize;
            currentSize = 0;
            lastBytes = 0;
        }


    }
}
