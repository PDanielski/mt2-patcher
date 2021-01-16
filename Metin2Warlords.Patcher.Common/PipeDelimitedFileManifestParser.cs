using System;
using System.Collections.Generic;
using System.IO;

namespace Metin2Warlords.Patcher.Common
{
    public class PipeDelimitedFileManifestParser
    {
        public List<PatchFile> Parse(string fileManifest)
        {
            var files = new List<PatchFile>();

            var lines = fileManifest.Split('\n');
            foreach(string line in lines)
            {
                var fields = line.Split('|');
                var path = fields[0];
                if (fields.Length >= 3 && path.Length > 0)
                {
                    int.TryParse(fields[2].Trim(), out int size);
                    files.Add(new PatchFile(
                    
                        name: Path.GetFileName(path).Trim(),
                        MD5: fields[1].Trim(),
                        basePath: Path.GetDirectoryName(path).Trim(),
                        size: size
                   ));
                }
            }
            return files;
        }

    }
}
