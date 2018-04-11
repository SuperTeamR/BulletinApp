using BulletinBridge.Services.ServiceModels;
using FessooFramework.Tools.DCT;
using FessooFramework.Tools.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Services
{
    public abstract class ImageServiceClient : BaseServiceClient
    {
        protected override IEnumerable<ServiceResponseConfigBase> Configurations => new ServiceResponseConfigBase[] { };
        public void AddImage(Action<Response_AddImage> callback, string name, byte[] image)
        {
            Response_AddImage response = null;
            DCT.ExecuteAsync(c =>
            {
                var result = false;
                var request = new Request_AddImage()
                {
                    Image = image,
                    Name = name,
                };
                Execute(request, callback);
            });
        }
    }
}
