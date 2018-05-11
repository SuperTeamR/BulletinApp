using BulletinWebDriver.Core;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebDriver.Helpers
{
    public static class ImageHelper
    {
        public static string TempPath => $"{Path.GetTempPath()}BulletinWorker\\";
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
                if (!Directory.Exists(TempPath))
                    Directory.CreateDirectory(TempPath);
                var localPath = $"{TempPath}{fileName}";
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
        public static string ImageFileToBase64String(string path)
        {
            try
            {
                using (var image = Image.FromFile(path))
                {
                    using (var m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        var imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        var base64String = Convert.ToBase64String(imageBytes);

                        return base64String;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
