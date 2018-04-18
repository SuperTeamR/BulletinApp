using BulletinBridge.Data;
using BulletinClient.HelperService;
using FessooFramework.Objects.ViewModel;
using FessooFramework.Tools.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinClient.ViewModels
{
    public class AccessCardVM : VM
    {
        #region Propety
        public AccessPackage Item
        {
            get => item.Value;
            set => item.Value = value;
        }

        public ObjectController<AccessPackage> item = new ObjectController<AccessPackage>(null);
        #endregion

        public void Update(AccessPackage selectedObject)
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
            if (Item == null)
            {
                //MessageBox.Show("Необходимо выбрать объект в списке источников");
                return false;
            }
            return true;
        }
    }
}
