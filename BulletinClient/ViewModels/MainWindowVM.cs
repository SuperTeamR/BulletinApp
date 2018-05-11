using System.Windows.Input;
using BulletinClient.Helpers;
using BulletinClient.ViewModels;
using FessooFramework.Objects.Delegate;
using FessooFramework.Objects.ViewModel;

namespace BulletinClient.Views
{
    public class MainWindowVM : VM
    {
        protected override void Loaded()
        {
            base.Loaded();
            DataHelper.IsAuth.Change += () => SetContent();
            ModalHelper.DialogChangeAction += () => SetDialog();
        }
        public object CurrentView { get; set; } 
        private WorkTableVM workTableView = new WorkTableVM();
        private LoginVM loginView = new LoginVM();
        public ModalVM DialogView
        {
            get => dialogView == null ? new ModalVM() : dialogView;
            set => dialogView = value;
        }
        ModalVM dialogView = new ModalVM();

        public ICommand CommandModalWindowClose { get; set; }

        public MainWindowVM()
        {
            CommandModalWindowClose = new DelegateCommand(ModalHelper.CloseDialog);
            SetContent();
            InitializeModalView();
        }

        private void SetContent()
        {
            if (DataHelper.IsAuth.Value)
                CurrentView = workTableView;
            else
                CurrentView = loginView;
            RaisePropertyChanged(()=> CurrentView);
        }

        private void InitializeModalView()
        {
            ModalHelper.Initialize(DialogView);
        }
        private void SetDialog()
        {
            RaisePropertyChanged(() => DialogView);
        }
    }
}