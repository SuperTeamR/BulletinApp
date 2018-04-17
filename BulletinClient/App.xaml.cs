using BulletinClient.Core;
using BulletinClient.Helpers;
using FessooFramework.Core;
using System.Windows;

namespace BulletinClient
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            DCT.Execute(d =>
            {
                d._SessionInfo.HashUID = "Example";
                d._SessionInfo.SessionUID = "Example";

                var t = d.ServiceClient.Ping();
                t = t;

                var r = WindowHelper.Window;
                r = r;
            });
        }
    }
}
