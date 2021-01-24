using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Metin2Warlords.Patcher.NewsFeature
{
    public class PatchServerNewsProvider : INewsProvider
    {
        private string _newsRemoteEndpoint;
        private string _newsImageFolder;
        public PatchServerNewsProvider(string newsRemoteEndpoint, string newsImageFolder)
        {
            _newsRemoteEndpoint = newsRemoteEndpoint;
            _newsImageFolder = newsImageFolder;
        }


        public List<News> GetNews()
        {
            try
            {
                using (var client = new WebClient())
                {
                    var jsonNews = client.DownloadString(_newsRemoteEndpoint);
                    var response = _convertResponse(jsonNews);
                    if (response == null || response.News == null || response.News.Count < 1)
                        return new List<News>();

                    //TODO handle all the news, for example with a carousel
                    var firstNews = response.News[0];
                    return new List<News>
                {
                    _processNewsDTO(firstNews)
                };
                }
            } catch (Exception ex)
            {
                return new List<News>();
            }
        }

        private PatchServerNewsResponse _convertResponse(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<PatchServerNewsResponse>(json);
            } catch (Exception ex)
            {
                return null;
            }
        }

        private News _processNewsDTO(PatchServerNewsDTO newsDTO)
        {
            return new News(
                    newsDTO.Title,
                    newsDTO.Description,
                    newsDTO.Href,
                    _getImagePath(newsDTO.Image)
                    );
        }

        private string _getImagePath(PatchServerNewsImageDTO imageDTO)
        {
            try
            {
                if (!Directory.Exists(_newsImageFolder))
                    Directory.CreateDirectory(_newsImageFolder);

                var remoteFileName = Path.GetFileName(imageDTO.Href);
                var localFileName = $"{imageDTO.Id}-{remoteFileName}";
                var localImagePath = Path.Combine(_newsImageFolder, localFileName);
                if (File.Exists(localImagePath))
                    return localImagePath;

                using (var client = new WebClient())
                {
                    client.DownloadFile(imageDTO.Href, localImagePath);
                    return localImagePath;
                }
            } catch (Exception ex)
            {
                return null;
            }
        }
   
    }
}
