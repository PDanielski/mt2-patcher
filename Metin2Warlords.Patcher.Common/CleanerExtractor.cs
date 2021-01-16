using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metin2Warlords.Patcher.Common
{
    public class CleanerExtractor
    {
        string source;
        public CleanerExtractor(string s)
        {
            source = s;
        }

        public List<PatchFile> extract()
        {
            List<PatchFile> files = new List<PatchFile>();
            string[] lines = source.Split('\n');
            foreach(string l in lines)
            {
                if (l.Length > 0)
                {
                    PatchFile file = new PatchFile(Path.GetFileName(l).Trim(), Path.GetDirectoryName(l).Trim(), null, 0);
                    file.Name = Path.GetFileName(l).Trim();
                    file.BasePath = Path.GetDirectoryName(l).Trim();
                    files.Add(file);
                }
            }

            return files;

        }
    }
}
