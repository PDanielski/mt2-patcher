using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metin2Warlords.Patcher.NewsFeature
{
    public class PatchServerNewsDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Href { get; set; }
        public PatchServerNewsImageDTO Image { get; set; }
    }
}
