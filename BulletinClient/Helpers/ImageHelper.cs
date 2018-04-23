using BulletinClient.Core;
using System.IO;
using System.Windows.Media.Imaging;

namespace BulletinClient.Helpers
{
    static class ImageHelper
    {
        public static byte[] ConvertImageToByte(BitmapSource Image)
        {
            byte[] result = null;
            DCT.Execute(data =>
            {
                using (var ms = new MemoryStream())
                {
                    var be = new PngBitmapEncoder();
                    be.Frames.Add(BitmapFrame.Create(Image));
                    be.Save(ms);
                    result = ms.ToArray();
                }
            });
            return result;
        }
    }
}