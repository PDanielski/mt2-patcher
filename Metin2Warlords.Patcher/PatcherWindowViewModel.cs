using Metin2Warlords.Patcher.NewsFeature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Metin2Warlords.Patcher
{
    public class PatcherWindowViewModel
    {
        public News FrontNews { get; set; }
        public BitmapImage FrontImage { get 
            {
                if (FrontNews == null || String.IsNullOrEmpty(FrontNews.ImagePath))
                    return null;
                var image = new BitmapImage(new Uri(FrontNews.ImagePath, UriKind.Relative));
                return image;
            } }
    }
}
