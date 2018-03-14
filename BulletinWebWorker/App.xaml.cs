using BulletinWebWorker.Managers;
using FessooFramework.Tools.DCT;
using System.Windows;

namespace BulletinWebWorker
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        MainWindow view;
        protected override void OnStartup(StartupEventArgs e)
        {
            DCT.Execute(d =>
            {
                Bootstrapper.Current.Run();
                view = new MainWindow();
                view.Show();
                WebWorkerManager.Execute();
                //WebWorkerManager.BulletinWork.Execute();
                //WebWorkerManager.ProfileWork.Execute();
            });
        }
    }
}
