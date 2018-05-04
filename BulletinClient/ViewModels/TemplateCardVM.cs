using System;
using System.Linq;
using System.Windows.Input;
using BulletinBridge.Data;
using BulletinBridge.Models;
using BulletinClient.HelperService;
using FessooFramework.Objects.ViewModel;
using FessooFramework.Tools.Controllers;

namespace BulletinClient.ViewModels
{
    public class TemplateCardVM : VM
    {
        #region Propety
        public BulletinTemplateCache Item
        {
            get => item.Value == null ? CreateItem : item.Value;
            set => item.Value = value;
        }
        private BulletinTemplateCache CreateItem = new BulletinTemplateCache();
        public ObjectController<BulletinTemplateCache> item = new ObjectController<BulletinTemplateCache>(null);
        #endregion
        #region Methods
        public void Update(BulletinTemplateCache selectedObject)
        {
            Item = selectedObject;
            RaisePropertyChanged(() => Item);
        }

        #endregion
    }
}