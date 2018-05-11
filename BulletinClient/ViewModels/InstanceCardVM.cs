using BulletinBridge.Data;
using FessooFramework.Objects.ViewModel;
using FessooFramework.Tools.Controllers;

namespace BulletinClient.ViewModels
{
    public class InstanceCardVM : VM
    {
        #region Propety
        public BulletinInstanceCache Item
        {
            get => item.Value == null ? CreateItem : item.Value;
            set => item.Value = value;
        }
        private BulletinInstanceCache CreateItem = new BulletinInstanceCache();
        public ObjectController<BulletinInstanceCache> item = new ObjectController<BulletinInstanceCache>(null);
        #endregion
        #region Methods
        public void Update(BulletinInstanceCache selectedObject)
        {
            Item = selectedObject;
            RaisePropertyChanged(() => Item);
        }

        #endregion
    }
}