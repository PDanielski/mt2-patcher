using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metin2Warlords.Patcher.NewsFeature
{
    public interface INewsProvider
    {
        List<News> GetNews();
    }

}
