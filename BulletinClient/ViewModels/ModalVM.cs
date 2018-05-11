using System.Windows.Input;
using BulletinClient.Core;
using BulletinClient.Helpers;
using FessooFramework.Objects.Delegate;
using FessooFramework.Objects.ViewModel;

namespace BulletinClient.ViewModels
{
    public class ModalVM : VM
    {
        #region Properties

        public ModalWindow Window { get; set; }
        public bool IsEnable { get; set; }
        public object DialogContent { get; set; }

        #endregion
        #region Command
        public ICommand CommandDialogClose { get; private set; }
        #endregion

        public ModalVM()
        {
            CommandDialogClose = new DelegateCommand(ModalHelper.CloseDialog);
        }

        public void SetContent(ModalWindow content)
        {
            DCT.Execute((data) =>
            {
                if (content == null)
                    IsEnable = false;
                else
                {
                    Window = content;
                    DialogContent = Window.Content;
                    IsEnable = true;
                }
                RaisePropertyChanged(() => IsEnable);
                RaisePropertyChanged(() => DialogContent);
            });
        }
    }
}