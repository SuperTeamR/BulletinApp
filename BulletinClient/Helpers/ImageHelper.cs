using BulletinClient.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
