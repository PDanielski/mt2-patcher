using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Metin2Warlords.Patcher.Common
{
    public delegate void SearchingCompletedEventHandler(object sender);

    public class ObsoleteFileFinder
    {
        private const int MaxNumOfWorkers = 1;
        public event SearchingCompletedEventHandler SearchingCompleted;

        public List<PatchFile> SearchObsoleteSyncronized(List<PatchFile> files)
        {
            var obsoleteFiles = new List<PatchFile>();
            foreach (var file in files)
            {
                if (!IsFileUpToDate(file))
                    obsoleteFiles.Add(file);
            }
            OnSearchingCompleted();
            return obsoleteFiles;
        }

        public async Task<List<PatchFile>> SearchObsoleteAsync(List<PatchFile> files)
        {
            if (MaxNumOfWorkers == 1 || files.Count <= MaxNumOfWorkers)
                return await Task.Run(() => SearchObsoleteSyncronized(files));

            var maxBatchSize = files.Count / MaxNumOfWorkers;
            var tasks = new List<Task<List<PatchFile>>>();
            for(int i = 0; i < files.Count; i += maxBatchSize)
            {
                var batchFiles = files.GetRange(i, Math.Min(maxBatchSize, files.Count - i));
                tasks.Add(Task.Run(() => SearchObsoleteSyncronized(batchFiles)));
            }
            await Task.WhenAll(tasks);

            var obsoletePatchFiles = new List<PatchFile>();
            tasks.ForEach(t => obsoletePatchFiles.AddRange(t.Result));
            return obsoletePatchFiles;
        }

        public bool IsFileUpToDate(PatchFile file)
        {
            var path = Path.Combine(file.BasePath, file.Name);
            if (!File.Exists(path))
                return false;

            using (var stream = File.OpenRead(path))
            {
                var md5 = BitConverter.ToString(MD5.Create().ComputeHash(stream)).Replace("-", "");
                return md5 == file.MD5;
            }
        }

        protected virtual void OnSearchingCompleted()
        {
            SearchingCompleted?.Invoke(this);
        }
    }
}
