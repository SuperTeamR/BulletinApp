using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BulletinBridge.Data;
using BulletinBridge.Models;
using BulletinClient.Core;
using BulletinClient.HelperService;
using FessooFramework.Objects.Delegate;
using FessooFramework.Objects.ViewModel;
using FessooFramework.Tools.Controllers;

namespace BulletinClient.ViewModels
{
    public class InstanceCollectionVM : VM
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
        public ICommand CommandRefresh { get; private set; }
        public BulletinInstanceCache SelectedObject
        {
            get => SelectedObjectController.Value;
            set => SelectedObjectController.Value = value;
        }
        private ObjectController<BulletinInstanceCache> SelectedObjectController = new ObjectController<BulletinInstanceCache>(null);
        public ObservableCollection<BulletinInstanceCache> MyItems { get; set; }
        public InstanceCardVM Card => card = card ?? new InstanceCardVM();
        public InstanceCardVM card { get; set; }
        public Action<BulletinInstanceCache> UseAsTemplateAction { get; set; }

        #endregion
        #region Constructor
        public InstanceCollectionVM()
        {
            CommandRefresh = new DelegateCommand(Refresh);
            Refresh();
            MyItems = new ObservableCollection<BulletinInstanceCache>();
        }
        #endregion
        #region Method

        public void Refresh()
        {
            InstanceHelper.All(a =>
            {
                DCT.ExecuteMainThread(c =>
                {
                    MyItems = new ObservableCollection<BulletinInstanceCache>(a);
                    RaisePropertyChanged(() => MyItems);
                });
            });
        }
        #endregion
    }
}