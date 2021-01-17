using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;

namespace Metin2Warlords.Patcher.Common
{
    public delegate void NewFileDownloadingEventHandler(object sender, NewFileDownloadingEventArgs e);
    public delegate void AfterFileDownloadedEventHandler(object sender, AfterFileDownloadedEventArgs e);
    public delegate void AllFileDownloadedEventHandler(object sender);

    public class HTTPDownloader
    {
        WebClient client;
        List<PatchFile> files;
        PatchFile currentFile;
        int currentIndex;
        string basePath;
        public event DownloadProgressChangedEventHandler Progress;
        public event AsyncCompletedEventHandler Completed;
        public event NewFileDownloadingEventHandler newFile;
        public event AfterFileDownloadedEventHandler afterNewFile;
        public event AllFileDownloadedEventHandler allFileDownloaded;
        public HTTPDownloader(List<PatchFile> files, WebClient client, string basePath)
        {
            this.files = files;
            this.client = client;
            this.basePath = basePath;
        }

        public void setProgressChangedEventHandler(DownloadProgressChangedEventHandler e)
        {
            Progress += e;
        }
        public void setAllFileDownloadedEventHandler(AllFileDownloadedEventHandler e)
        {
            allFileDownloaded += e;
        }
        public void setDownloadDataCompletedEventHandler(AsyncCompletedEventHandler e){
            Completed += e;
        }

        public void setNewFileDownloadingEventHandler(NewFileDownloadingEventHandler e)
        {
            newFile += e;
        }

        public int getTotalSize()
        {
            int totalSize = 0;
            foreach(PatchFile f in files)
            {
                totalSize += f.Size;
            }
            return totalSize;
        }

        
        public void startDownload()
        {
            if (Progress != null) client.DownloadProgressChanged += Progress;
            if (Completed != null) client.DownloadFileCompleted += Completed;

            if (client.BaseAddress.Length == 0)
            {
                throw new Exception("BaseAddress missing in the WebClient Object");
            }

            int index = 0;
            PatchFile f = files[index];
            setCurrentIndex(index + 1);
            OnNewFile(f);
            setCurrentFile(f);
            string path = Path.Combine(f.BasePath, f.Name);
            string downloadPath = basePath + path;
            if (f.BasePath != String.Empty) Directory.CreateDirectory(Path.GetDirectoryName(path));
            client.DownloadFileAsync(new Uri(new Uri(client.BaseAddress), downloadPath), path);
            void onFileDownloaded(object se, AsyncCompletedEventArgs es)
            {
                if (files.Count > index + 1)
                {
                    index = index + 1;
                    f = files[index];
                    OnAfterNewFile(f);
                    setCurrentIndex(index + 1);
                    OnNewFile(f);
                    path = Path.Combine(f.BasePath, f.Name);
                    downloadPath = basePath + path;
                    if (f.BasePath != String.Empty) Directory.CreateDirectory(Path.GetDirectoryName(path));
                    client.DownloadFileAsync(new Uri(new Uri(client.BaseAddress), downloadPath), path);
                }
                else
                {
                    client.DownloadFileCompleted -= onFileDownloaded;
                    OnAllFileDownloaded();
                }
            }
            client.DownloadFileCompleted += onFileDownloaded;

        }

        public void setAfterNewFileEventHandler(AfterFileDownloadedEventHandler e)
        {
            afterNewFile += e;
        }
        public PatchFile getCurrentFile()
        {
            return currentFile;
        }
        
        protected void setCurrentFile(PatchFile file)
        {
            currentFile = file;
        }

        protected void setCurrentIndex(int i)
        {
            currentIndex = i;
        }

        public int getCurrentIndex()
        {
            return currentIndex;
        }

        protected virtual void OnNewFile(PatchFile file)
        {
            if(newFile != null)
            {
                NewFileDownloadingEventArgs args = new NewFileDownloadingEventArgs();
                args.file = file;
                args.remainingFiles = getCurrentIndex();
                newFile(this, args);
            }
        }

        protected virtual void OnAllFileDownloaded()
        {
            allFileDownloaded?.Invoke(this);
            if (Progress != null) client.DownloadProgressChanged -= Progress;
            if (Completed != null) client.DownloadFileCompleted -= Completed;
        }


        protected virtual void OnAfterNewFile(PatchFile file)
        {
            if(afterNewFile != null)
            {
                AfterFileDownloadedEventArgs e = new AfterFileDownloadedEventArgs();
                e.file = file;
                afterNewFile(this, e);
            }
        }
        public int getTotalFiles()
        {
            if(files == null)
            {
                return 0;
            }
            return files.Count;
        }

    }
}
