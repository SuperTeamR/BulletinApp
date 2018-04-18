using BulletinBridge.Data;
using BulletinClient.Core;
using BulletinClient.HelperService;
using FessooFramework.Objects.Delegate;
using FessooFramework.Objects.ViewModel;
using FessooFramework.Tools.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
        public AccessPackage SelectedObject
        {
            get => SelectedObjectController.Value;
            set => SelectedObjectController.Value = value;
        }
        private ObjectController<AccessPackage> SelectedObjectController = new ObjectController<AccessPackage>(null);
        public ObservableCollection<AccessPackage> MyItems { get; set; }
        public AccessCardVM Card => card = card ?? new AccessCardVM();
        public AccessCardVM card { get; set; }
        #endregion
        #region Constructor
        public AccessCollectionVM()
        {
            CommandClear = new DelegateCommand(Clear);
            CommandRefresh = new DelegateCommand(Refresh);
            CommandRemove = new DelegateCommand(Remove);
            Refresh();
            MyItems = new ObservableCollection<AccessPackage>();
        }
        #endregion
        #region Method
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
                    MyItems = new ObservableCollection<AccessPackage>(a);
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
