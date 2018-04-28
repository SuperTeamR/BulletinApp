using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebDriver.Helpers
{
    public static class ImageHelper
    {
        public static string ImageToTemp(IEnumerable<string> urls)
        {
            var result = "";
            foreach (var url in urls)
            {
                if (!string.IsNullOrWhiteSpace(result))
                    result += ", ";
                result += ImageToTemp(url);
            }
            return result;
        }
        public static string ImageToTemp(string url)
        {
            var result = "";
            try
            {
                var tempPath = Path.GetTempPath();
                var fileName = GetFilenameFromUrl(url);
                var myFolder = $"{Path.GetTempPath()}BulletinWorker\\";
                if (!Directory.Exists(myFolder))
                {
                    Directory.CreateDirectory(myFolder);
                }
                var localPath = $"{myFolder}{fileName}";
                using (WebClient client = new WebClient())
                    client.DownloadFile(new Uri(url), localPath);
                result = localPath;
            }
            catch (Exception ex)
            {
                var r = 1;
            }
            return result;
        }
        public static string GetFilenameFromUrl(string url)
        {
            return String.IsNullOrEmpty(url.Trim()) || !url.Contains(".") ? string.Empty : Path.GetFileName(new Uri(url).AbsolutePath);
        }
    }
}
