using BulletinBridge.Services.ServiceModels;
using FessooFramework.Components.LoggerComponent;
using FessooFramework.Tools.DCT;
using FessooFramework.Tools.Web;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Services
{
    public abstract class ImageServiceAPI : ServiceBaseAPI
    {
        #region Property
        public override string Name => "ImageServiceAPI";

        static readonly string ExternallUrl = @"http://176.111.73.51:44444/Images";
        static readonly string Address = @"V:\Services\FotoService\Images";
        //public static readonly string Address = @"E:\1";

        protected override IEnumerable<ServiceRequestConfigBase> Configurations =>
          new ServiceRequestConfigBase[] {
                ServiceRequestConfig<Request_AddImage, Response_AddImage>.New(_AddImage),
          };

        #endregion

        public static Response_AddImage _AddImage(Request_AddImage request)
        {
            var result = new Response_AddImage();
            DCT.Execute(d =>
            {
                var path = Path.Combine(Address, request.Name + ".png");

                var image = Image.FromStream(new MemoryStream(request.Image));
                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                    {
                        image.Save(memory, ImageFormat.Jpeg);
                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                        result.Url = Path.Combine(ExternallUrl, request.Name + ".png");
                    }
                }
            }, continueExceptionMethod: (d2, e) => 
            {
                result.Error = e.ToString();
            });
            return result;
        }
    }
}
