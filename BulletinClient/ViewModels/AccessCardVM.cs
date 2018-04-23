using BulletinBridge.Data;
using BulletinClient.HelperService;
using FessooFramework.Objects.Delegate;
using FessooFramework.Objects.ViewModel;
using FessooFramework.Tools.Controllers;
using System.Linq;
using System.Windows.Input;

namespace BulletinClient.ViewModels
{
    public class AccessCardVM : VM
    {
        #region Propety
        public AccessPackage Item
        {
            get => item.Value == null ? CreateItem : item.Value;
            set => item.Value = value;
        }
        private AccessPackage CreateItem = new AccessPackage();
        public ObjectController<AccessPackage> item = new ObjectController<AccessPackage>(null);
        public ICommand CommandAdd { get; private set; }

        #endregion
        #region Constructor
        public AccessCardVM()
        {
            CommandAdd = new DelegateCommand(AddAvito);
        }
        #endregion
        #region Methods
        public void Update(AccessPackage selectedObject)
        {
            Item = selectedObject;
            RaisePropertyChanged(() => Item);
        }
        public void Clear()
        {
            Item = null;
            RaisePropertyChanged(() => Item);
        }
        internal void Save()
        {
            if (Check())
                AccessHelper.Save(() => { }, Item);
            RaisePropertyChanged(() => Item);
        }
        private bool Check()
        {
            if (item.Value == null)
            {
                //MessageBox.Show("Необходимо выбрать объект в списке источников");
                return false;
            }
            return true;
        }
        private void AddAvito()
        {
            AccessHelper.AddAvito((a) =>
            {
                item.Value = a.FirstOrDefault();
                CreateItem = new AccessPackage();
                RaisePropertyChanged(() => Item);
            }, Item);
        }

        #endregion
    }
}