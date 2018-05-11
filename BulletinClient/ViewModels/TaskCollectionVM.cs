using System.Collections.ObjectModel;
using System.Windows.Input;
using BulletinBridge.Models;
using BulletinClient.Core;
using BulletinClient.HelperService;
using FessooFramework.Objects.Delegate;
using FessooFramework.Objects.ViewModel;
using FessooFramework.Tools.Controllers;

namespace BulletinClient.ViewModels
{
    public class TaskCollectionVM : VM
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
        public TaskCache SelectedObject
        {
            get => SelectedObjectController.Value;
            set => SelectedObjectController.Value = value;
        }
        private ObjectController<TaskCache> SelectedObjectController = new ObjectController<TaskCache>(null);
        public ObservableCollection<TaskCache> MyItems { get; set; }
        public TaskCardVM Card => card = card ?? new TaskCardVM();
        public TaskCardVM card { get; set; }

        #endregion
        #region Constructor
        public TaskCollectionVM()
        {
            CommandRefresh = new DelegateCommand(Refresh);
            Refresh();
            MyItems = new ObservableCollection<TaskCache>();
        }
        #endregion
        #region Method

        public void Refresh()
        {
            TaskHelper.All(a =>
            {
                DCT.ExecuteMainThread(c =>
                {
                    MyItems = new ObservableCollection<TaskCache>(a);
                    RaisePropertyChanged(() => MyItems);
                });
            });
        }

        #endregion
    }
}