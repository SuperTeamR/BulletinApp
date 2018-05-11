using BulletinBridge.Data;
using BulletinClient.HelperService;
using FessooFramework.Objects.Delegate;
using FessooFramework.Objects.ViewModel;
using FessooFramework.Tools.Controllers;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BulletinClient.Helpers;

namespace BulletinClient.ViewModels
{
    public class AccessCardVM : VM
    {
        #region Propety
        public AccessCache Item
        {
            get => item.Value == null ? CreateItem : item.Value;
            set => item.Value = value;
        }
        private AccessCache CreateItem = new AccessCache();
        public ObjectController<AccessCache> item = new ObjectController<AccessCache>(null);
        public ICommand CommandAdd { get; private set; }
        #endregion
        #region Constructor
        public AccessCardVM()
        {
            CommandAdd = new DelegateCommand(AddAvito);
        }

        #endregion
        #region Methods

        public void Update(AccessCache selectedObject)
        {
            if (selectedObject == null)
                Clear();
            else
            {
                Item = selectedObject;
                RaisePropertyChanged(() => Item);
            }
        }
        public void Clear()
        {
            Item = null;
            RaisePropertyChanged(() => Item);
        }
        internal void Save()
        {
            AccessHelper.Save(() => { }, Item);
            RaisePropertyChanged(() => Item);
        }
        private void AddAvito()
        {
            AccessHelper.AddAvito((a) =>
            {
                item.Value = a.FirstOrDefault();
                CreateItem = new AccessCache();
                RaisePropertyChanged(() => Item);
                ModalHelper.CloseDialog();
            }, Item);
        }
        #endregion
    }
}