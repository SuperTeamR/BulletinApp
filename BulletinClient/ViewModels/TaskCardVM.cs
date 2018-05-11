using BulletinBridge.Models;
using FessooFramework.Objects.ViewModel;
using FessooFramework.Tools.Controllers;

namespace BulletinClient.ViewModels
{
    public class TaskCardVM : VM
    {
        #region Propety
        public TaskCache Item
        {
            get => item.Value == null ? CreateItem : item.Value;
            set => item.Value = value;
        }
        private TaskCache CreateItem = new TaskCache();
        public ObjectController<TaskCache> item = new ObjectController<TaskCache>(null);
        #endregion
        #region Methods
        public void Update(TaskCache selectedObject)
        {
            Item = selectedObject;
            RaisePropertyChanged(() => Item);
        }
        #endregion 
    }
}