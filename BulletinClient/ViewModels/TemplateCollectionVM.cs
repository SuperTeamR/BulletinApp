using System;
using System.Collections.ObjectModel;
using System.Windows;
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
    public class TemplateCollectionVM : VM
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
        public ICommand CommandUseAsTemplate { get; private set; }
        public ICommand CommandRefresh { get; private set; }
        public BulletinTemplateCache SelectedObject
        {
            get => SelectedObjectController.Value;
            set => SelectedObjectController.Value = value;
        }
        private ObjectController<BulletinTemplateCache> SelectedObjectController = new ObjectController<BulletinTemplateCache>(null);
        public ObservableCollection<BulletinTemplateCache> MyItems { get; set; }
        public TemplateCardVM Card => card = card ?? new TemplateCardVM();
        public TemplateCardVM card { get; set; }
        public Action<BulletinTemplateCache> UseAsTemplateAction { get; set; }

        #endregion
        #region Constructor
        public TemplateCollectionVM()
        {
            CommandRefresh = new DelegateCommand(Refresh);
            CommandUseAsTemplate = new DelegateCommand(UseAsTemplate);
            Refresh();
            MyItems = new ObservableCollection<BulletinTemplateCache>();
        }
        #endregion
        #region Method

        public void Refresh()
        {
            TemplateHelper.All(a =>
            {
                DCT.ExecuteMainThread(c =>
                {
                    MyItems = new ObservableCollection<BulletinTemplateCache>(a);
                    RaisePropertyChanged(() => MyItems);
                });
            });
        }
        private void UseAsTemplate()
        {
            if (SelectedObject != null)
            {
                UseAsTemplateAction?.Invoke(SelectedObject);
            }
        }

        #endregion

    }
}