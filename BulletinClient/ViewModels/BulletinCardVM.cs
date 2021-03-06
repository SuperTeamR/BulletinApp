﻿using System;
using BulletinBridge.Data;
using BulletinClient.HelperService;
using FessooFramework.Objects.Delegate;
using FessooFramework.Objects.ViewModel;
using FessooFramework.Tools.Controllers;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BulletinBridge.Models;
using FessooFramework.Tools.DCT;
using DCT = BulletinClient.Core.DCT;

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

        public DateTime PublicationDate { get; set; }
        public bool HasCustomDate { get; set; }

        public DateTime MinimumDate
        {
            get { return DateTime.Now; }
        }

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

        public bool CanPublicate => !Item.InPublicationProcess;

        public bool SkipTextChange;

        public ICommand CommandAdd { get; private set; }
        public ICommand CommandPublicate { get; set; }
        public ICommand CommandClear { get; set; }
        public ICommand CommandAutoPublicate { get; set; }
        #endregion
        #region Constructor
        public BulletinCardVM()
        {
            CardCategory1 = "Бытовая электроника";
            CardCategory2 = "Телефоны";
            CardCategory3 = "iPhone";
            PublicationDate = DateTime.Now;
            HasCustomDate = true;
            Item.City = "Подольск";
            CommandAdd = new DelegateCommand(AddAvito);
            CommandPublicate = new DelegateCommand(Publicate);
            CommandClear = new DelegateCommand(Clear);
            CommandAutoPublicate = new DelegateCommand(AutoPublicate);
        }


        #endregion
        #region Methods
        public void Update(BulletinCache selectedObject)
        {
            SkipTextChange = true;
            Item = selectedObject;
            RaisePropertyChanged(() => Item);
            RaisePropertyChanged(() => CanPublicate);
            TemplateCollectionView.SearchPattern = Item.Title;
            TemplateCollectionView.Refresh();
           
            SkipTextChange = false;
        }
        public void Clear()
        {
            Item = null;
            RaisePropertyChanged(() => Item);
            RaisePropertyChanged(() => CanPublicate);
            TemplateCollectionView.SearchPattern = null;
            TemplateCollectionView.Refresh();
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
            DCT.ExecuteAsync(d =>
            {
                if (!CanPublicate)
                {
                    MessageBox.Show("Объявление уже было отправлено на публикацию");
                    return;
                }
                Item.PublicationDate = HasCustomDate ? (DateTime?)PublicationDate : null;
                BulletinHelper.Publicate(a =>
                {
                    Item.InPublicationProcess = true;
                    MessageBox.Show("Объявление отправлено на публикацию");
                    RaisePropertyChanged(() => CanPublicate);
                }, Item);
            });
        }

        private void AutoPublicate()
        {
            DCT.ExecuteAsync(d =>
            {
                if (!CanPublicate)
                {
                    MessageBox.Show("Объявление уже было отправлено на публикацию");
                    return;
                }
                var template = TemplateCollectionView.GetAutoTemplate();
                AddAvitoWithTemplate(template);
                BulletinHelper.AutoPublicate(a =>
                {
                    Item.InPublicationProcess = true;
                    MessageBox.Show("Объявление отправлено на публикацию");
                    RaisePropertyChanged(() => CanPublicate);
                }, Item);
            });
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
                CreateItem.Price = template.Price.ToString();
                CreateItem.Images = template.Images;
            }
            //Объект выбран. Создаем новую инстанцию на базе шаблона и цены старого буллетина
            else
            {
                var bulletin = item.Value;
                bulletin.Title = template.Title;
                bulletin.Description = template.Description;
                bulletin.Images = template.Images;
            }

            TemplateHelper.MarkAsUsed(() => templateCollectionView.Refresh(), template);
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