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
            totalSize = downloader.getTotalSize();
            baseCurrentSize = 0;
            currentSize = 0;
            lastBytes = 0;
            bytesForSecond = 0;
            Application.Current.Dispatcher.Invoke((Action)(() => {
                window.startButton.IsEnabled = true;
                window.repairButton.IsEnabled = true;
                window.multiProgressBar.Value = 100;
                window.progressStatus.Text = "";
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

            double globalPercentage = ((double)currentSize / (double)totalSize) * 100;

            double mbReceived = currentSize / 1000000;
            double mbTotal = totalSize / 1000000;
            double mbPerSecond = (double)((double)bytesForSecond / (double)1000);
            string text = String.Format("{0} MB of {1} MB, {2} MB/s, {3}%", mbReceived, mbTotal, mbPerSecond.ToString("F"), (int)globalPercentage);
            Application.Current.Dispatcher.Invoke((Action)(() => {
                window.progressStatus.Text = text;
                window.multiProgressBar.Value = globalPercentage;
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
