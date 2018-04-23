using BulletinClient.Helpers;
using BulletinClient.ViewModels;
using FessooFramework.Objects.ViewModel;

namespace BulletinClient.Views
{
    public class MainWindowVM : VM
    {
        protected override void Loaded()
        {
            base.Loaded();
            DataHelper.IsAuth.Change += () => SetContent();
        }
        public object CurrentView { get; set; } 
        private WorkTableVM workTableView = new WorkTableVM();
        private LoginVM loginView = new LoginVM();

        public MainWindowVM()
        {
            SetContent();
        }

        private void SetContent()
        {
            if (DataHelper.IsAuth.Value)
                CurrentView = workTableView;
            else
                CurrentView = loginView;
            RaisePropertyChanged(()=> CurrentView);
        }
    }
}