using BulletinBridge.Data;
using BulletinClient.Core;
using BulletinClient.HelperService;
using FessooFramework.Objects.Delegate;
using FessooFramework.Objects.ViewModel;
using FessooFramework.Tools.Controllers;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using BulletinClient.Helpers;

namespace BulletinClient.ViewModels
{
    public class AccessCollectionVM : VM
    {
        #region Loaded\Unloaded
        protected override void Loaded()
        {
            base.Loaded();
            SelectedObjectController.Change += () => { Card.Update(SelectedObject); };
        }
        protected override void Unloaded()
        {
            base.Unloaded();
            SelectedObjectController.Change -= () => { Card.Update(SelectedObject); };
        }
        #endregion
        #region Property
        public ICommand CommandClear { get; private set; }
        public ICommand CommandRefresh { get; private set; }
        public ICommand CommandRemove { get; private set; }
        public ICommand CommandActivate { get; set; }
        public ICommand CommandOpen { get; set; }
        public ICommand CommandUpdateObject { get; set; }
        public ICommand CommandAdd { get; set; }
        public ICommand CommandSelectAccess { get; set; }
        public AccessCache SelectedObject
        {
            get => SelectedObjectController.Value;
            set => SelectedObjectController.Value = value;
        }
        private ObjectController<AccessCache> SelectedObjectController = new ObjectController<AccessCache>(null);
        public ObservableCollection<AccessCache> MyItems { get; set; }
        public AccessCardVM Card => card = card ?? new AccessCardVM();
        public AccessCardVM card { get; set; }
        #endregion
        #region Constructor
        public AccessCollectionVM()
        {
            CommandClear = new DelegateCommand(Clear);
            CommandRefresh = new DelegateCommand(Refresh);
            CommandRemove = new DelegateCommand(Remove);
            CommandActivate = new DelegateCommand<AccessCache>(Activate);
            CommandOpen = new DelegateCommand<AccessCache>(Open);
            CommandUpdateObject = new DelegateCommand<AccessCache>(UpdateObject);
            CommandAdd = new DelegateCommand(Add);
            CommandSelectAccess = new DelegateCommand<AccessCache>(SelectAccess);
            Refresh();
            MyItems = new ObservableCollection<AccessCache>();
        }

        private void SelectAccess(AccessCache obj)
        {
            Card.Update(obj);
            ModalHelper.OpenDialog(Card);
        }

        #endregion
        #region Method
        private void Add()
        {
            SelectedObject = null;
            ModalHelper.OpenDialog(Card);
        }

        private void UpdateObject(AccessCache obj)
        {
            AccessHelper.Save(() => { }, obj);
        }

        private void Remove()
        {
            if (CheckSelected())
                AccessHelper.Remove(Refresh, SelectedObject);
        }

        private void Refresh()
        {
            AccessHelper.All(a =>
            {
                DCT.ExecuteMainThread(c =>
                {
                    MyItems = new ObservableCollection<AccessCache>(a);
                    RaisePropertyChanged(() => MyItems);
                });
            });
        }
        private void Clear()
        {
            SelectedObject = null;
            Card.Clear();
        }

        private bool CheckSelected()
        {
            if (SelectedObject == null)
            {
                MessageBox.Show("DataSource - Необходимо выбрать объект в списке");
                return false;
            }
            return true;
        }

        private void Activate(AccessCache cache)
        {
            AccessHelper.ActivateAccess(a =>
            {
                MessageBox.Show("Отправлена задача на активацию");
            }, cache);
        }

        private void Open(AccessCache cache)
        {
            System.Diagnostics.Process.Start("http://www.avito.ru/profile");
        }
        #endregion
    }
}