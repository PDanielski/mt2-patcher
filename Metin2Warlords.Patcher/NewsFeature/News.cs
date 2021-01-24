using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metin2Warlords.Patcher.NewsFeature
{
    public class News
    {
        public string Title { get; internal set; }
        public string Description { get; internal set; }
        public string Href { get; internal set; }
        public string ImagePath { get; internal set; }

        public News(string title, string description, string href, string imagePath)
        {
            Title = title;
            Description = description;
            Href = href;
            ImagePath = imagePath;
        }
    }
}
