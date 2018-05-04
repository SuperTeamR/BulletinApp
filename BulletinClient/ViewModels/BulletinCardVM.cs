using BulletinBridge.Data;
using BulletinClient.HelperService;
using FessooFramework.Objects.Delegate;
using FessooFramework.Objects.ViewModel;
using FessooFramework.Tools.Controllers;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BulletinBridge.Models;

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
        public TemplateCollectionVM TemplateCollectionView =>
            templateCollectionView = templateCollectionView ?? CreateTemplateCollectionView();
        public TemplateCollectionVM templateCollectionView { get; set; }

        public bool CanPublicate => Item.State != 1 && Item.State != 2 && !Item.InPublicationProcess;

        public ICommand CommandAdd { get; private set; }
        public ICommand CommandPublicate { get; set; }
        #endregion
        #region Constructor
        public BulletinCardVM()
        {
            CardCategory1 = "Бытовая электроника";
            CardCategory2 = "Телефоны";
            CardCategory3 = "iPhone";
            Item.City = "Подольск";
            CommandAdd = new DelegateCommand(AddAvito);
            CommandPublicate = new DelegateCommand(Publicate);
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
            if (CheckIfCanEdit())
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
        private bool CheckIfCanEdit()
        {
            if (item.Value == null || Item.State == (int)(BulletinState.Created))
            {
                //MessageBox.Show("Необходимо выбрать объект в списке источников");
                return false;
            }
            return true;
        }

        private void Publicate()
        {
            if (!CanPublicate)
            {
                MessageBox.Show("Объявление уже было отправлено на публикацию");
                return;
            }
            BulletinHelper.Publicate(a =>
            {
                Item.InPublicationProcess = true;
                MessageBox.Show("Объявление отправлено на публикацию");
                RaisePropertyChanged(() => CanPublicate);
            }, Item);
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

        private void AddAvitoWithTemplate(BulletinTemplateCache template)
        {
            //Объект не выбран. Просто подставляем поля
            if (item.Value == null)
            {
                CreateItem.Title = template.Title;
                CreateItem.Description = template.Description;
                CreateItem.Images = template.Images;
            }
            //Объект выбран. Создаем новый буллетин на базе шаблона и цены старого буллетина
            else
            {
                CreateItem = new BulletinCache
                {
                    Title = template.Title,
                    Description = template.Description,
                    Images = template.Images,
                    Price = Item.Price,
                    GroupSignature = Item.GroupSignature,
                    City = Item.City
                };
                item.Value = null;
                AddAvito();
                TemplateHelper.MarkAsUsed(templateCollectionView.Refresh, template);
            }
            RaisePropertyChanged(() => Item);
        }

        public TemplateCollectionVM CreateTemplateCollectionView()
        {
            var collection = new TemplateCollectionVM();
            collection.UseAsTemplateAction = AddAvitoWithTemplate;
            return collection;
        }

        #endregion
    }
}