using BulletinBridge.Data;
using BulletinClient.Core;
using BulletinClient.HelperService;
using FessooFramework.Objects.Delegate;
using FessooFramework.Objects.ViewModel;
using FessooFramework.Tools.Controllers;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace BulletinClient.ViewModels
{
    public class BulletinCollectionVM : VM
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
        public BulletinCache SelectedObject
        {
            get => SelectedObjectController.Value;
            set => SelectedObjectController.Value = value;
        }
        private ObjectController<BulletinCache> SelectedObjectController = new ObjectController<BulletinCache>(null);
        public ObservableCollection<BulletinCache> MyItems { get; set; }
        public BulletinCardVM Card => card = card ?? new BulletinCardVM();
        public BulletinCardVM card { get; set; }
        #endregion
        #region Constructor
        public BulletinCollectionVM()
        {
            CommandClear = new DelegateCommand(Clear);
            CommandRefresh = new DelegateCommand(Refresh);
            CommandRemove = new DelegateCommand(Remove);
            Refresh();
            MyItems = new ObservableCollection<BulletinCache>();
        }
        #endregion
        #region Method
        private void Remove()
        {
            if (CheckSelected())
                BulletinHelper.Remove(Refresh, SelectedObject);
        }

        private void Refresh()
        {
            BulletinHelper.All(a =>
            {
                DCT.ExecuteMainThread(c =>
                {
                    MyItems = new ObservableCollection<BulletinCache>(a);
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
        #endregion
    }
}