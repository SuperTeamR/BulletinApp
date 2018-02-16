using BulletinClient.Core;
using FessooFramework.Objects.ViewModel;

namespace BulletinClient.General
{
    class MainWindowVM : VM
    {
        public Forms.MainView.ViewModel CurrentView => DCT.Execute(d => d.ContainerViewModel.GetInstance<Forms.MainView.ViewModel>());
    }
}
