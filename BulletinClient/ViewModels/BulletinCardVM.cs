using BulletinBridge.Data;
using BulletinClient.HelperService;
using FessooFramework.Objects.Delegate;
using FessooFramework.Objects.ViewModel;
using FessooFramework.Tools.Controllers;
using System.Linq;
using System.Windows.Input;

namespace BulletinClient.ViewModels
{
    public class BulletinCardVM : VM
    {
        #region Propety
        public string CardCategory1 { get; set; }
        public string CardCategory2 { get; set; }
        public string CardCategory3 { get; set; }
        public string CardCategory4 { get; set; }
        public string CardCategory5 { get; set; }
        public BulletinCache Item
        {
            get => item.Value == null ? CreateItem : item.Value;
            set => item.Value = value;
        }
        private BulletinCache CreateItem = new BulletinCache();
        public ObjectController<BulletinCache> item = new ObjectController<BulletinCache>(null);
        public ICommand CommandAdd { get; private set; }
        #endregion
        #region Constructor
        public BulletinCardVM()
        {
            CardCategory1 = "Бытовая электроника";
            CardCategory2 = "Телефоны";
            CardCategory3 = "iPhone";
            Item.City = "Подольск";
            CommandAdd = new DelegateCommand(AddAvito);
        }
        #endregion
        #region Methods
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
            {
                SetSignature();
                BulletinHelper.Edit((a) => { }, Item);
            }
            RaisePropertyChanged(() => Item);
        }
        public void SetSignature()
        {
           Item.GroupSignature = BulletinHelper.StringToSha256String(CardCategory1, CardCategory2, CardCategory3, CardCategory4, CardCategory5);
        }
        private bool Check()
        {
            if (item.Value == null)
            {
                //MessageBox.Show("Необходимо выбрать объект в списке источников");
                return false;
            }
            return true;
        }
        private void AddAvito()
        {
            SetSignature();
            BulletinHelper.AddAvito((a) =>
            {
                item.Value = a.FirstOrDefault();
                CreateItem = new BulletinCache();
                RaisePropertyChanged(() => Item);
            }, Item);
        }
        #endregion
    }
}