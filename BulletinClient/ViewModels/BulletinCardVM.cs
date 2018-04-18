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
    public class BulletinCardVM : VM
    {
        #region Propety
        public BulletinCache Item
        {
            get => item.Value;
            set => item.Value = value;
        }

        public ObjectController<BulletinCache> item = new ObjectController<BulletinCache>(null);
        #endregion

        public void Update(BulletinCache selectedObject)
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
                BulletinHelper.Save(() => { }, Item);
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
