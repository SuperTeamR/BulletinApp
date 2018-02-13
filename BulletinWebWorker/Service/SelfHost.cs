using System.ServiceModel;
using System.ServiceModel.Description;

namespace BulletinWebWorker.Service
{
    static class SelfHost
    {
        public static void Initialize()
        {
            using (var host = new ServiceHost(null, null))
            {
                // Enable metadata publishing.
                var smb = new ServiceMetadataBehavior();

                smb.HttpGetEnabled = true;
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(smb);

                host.Open();
            }
        }
    }
}
