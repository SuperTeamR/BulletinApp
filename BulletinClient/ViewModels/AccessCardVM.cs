using BulletinBridge.Data;
using BulletinClient.HelperService;
using FessooFramework.Objects.Delegate;
using FessooFramework.Objects.ViewModel;
using FessooFramework.Tools.Controllers;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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
        public ICommand CommandActivate { get; private set; }
        public ICommand CommandOpen { get; set; }

        #endregion
        #region Constructor
        public AccessCardVM()
        {
            CommandAdd = new DelegateCommand(AddAvito);
            CommandActivate = new DelegateCommand(ActivateAccess);
            CommandOpen = new DelegateCommand(Open);
        }



        #endregion
        #region Methods

        private void Open()
        {
            System.Diagnostics.Process.Start("http://www.avito.ru/profile");
        }

        public void Update(AccessCache selectedObject)
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
                CreateItem = new AccessCache();
                RaisePropertyChanged(() => Item);
            }, Item);
        }
        private void ActivateAccess()
        {
            AccessHelper.ActivateAccess(a =>
            {
                MessageBox.Show("Отправлена задача на активацию");
                item.Value = a.FirstOrDefault();
                CreateItem = new AccessCache();
                RaisePropertyChanged(() => Item);
            }, Item);
        }

        #endregion
    }
}