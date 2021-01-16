using Metin2Warlords.Patcher.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Metin2Warlords.Patcher.Updater
{
    class UpdaterDownloadListener
    {
        UpdateWindow window;
        HTTPDownloader downloader;

        Storyboard sb = new Storyboard();
        public UpdaterDownloadListener(UpdateWindow w,HTTPDownloader d)
        {
            window = w;
            downloader = d;
            d.setProgressChangedEventHandler(new DownloadProgressChangedEventHandler(onProgressChanged));
            d.setNewFileDownloadingEventHandler(new NewFileDownloadingEventHandler(OnNewFile));
            d.setAllFileDownloadedEventHandler(new AllFileDownloadedEventHandler(OnAllFile));
        }

        void OnAllFile(object e)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                window.singleProgressBar.Visibility = System.Windows.Visibility.Hidden;
            }));

        }
        void OnNewFile(object sender, NewFileDownloadingEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                window.singleProgressBar.Value = 0;
            }));

        }
        void onProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                window.singleProgressBar.Visibility = System.Windows.Visibility.Visible;
                window.singleProgressBar.Value = e.ProgressPercentage;
            }));

        }
    }
}
