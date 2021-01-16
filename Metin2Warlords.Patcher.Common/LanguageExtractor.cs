using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Media.Imaging;

namespace MTRPatcher
{
    public class LanguageExtractor
    {
        string source;
        public LanguageExtractor(string s)
        {
            source = s;
        }

        public List<Language> extract()
        {
            List<Language> l = new List<Language>();
            string[] lines = source.Split('\n');
            foreach(string li in lines)
            {
                Language lang = new Language();
                string[] part = li.Split('|');
                lang.code = part[0].Trim();
                lang.name = part[1].Trim();
                string imgUrl = part[2].Trim();
                var buffer = (new WebClient()).DownloadData(imgUrl);
                BitmapImage bi = new BitmapImage();
                using(var stream = new MemoryStream(buffer))
                {
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.StreamSource = stream;
                    bi.EndInit();
                }
                lang.flag = bi;

                l.Add(lang);
            }
            return l;
        }

    }
}
