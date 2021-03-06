﻿using BulletinBridge.Data;
using FessooFramework.Objects.ViewModel;
using FessooFramework.Tools.Helpers;
using System;

namespace BulletinClient.Data
{
    class BulletinView : NotifyObject
    {
        public Guid BulletinId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public int Views { get; set; }
        public string State { get; set; }
        public string Url { get; set; }

        bool _canRepublicate;
        public bool CanRepublicate
        {
            get { return _canRepublicate; }
            set { _canRepublicate = value; RaisePropertyChanged(() => CanRepublicate); }
        }
        public BulletinView(BulletinPackage package)
        {
            BulletinId = package.BulletinId;

            if(package.ValueFields != null)
            {
                if(package.ValueFields.ContainsKey("Название объявления"))
                    Title = package.ValueFields["Название объявления"];
                if (package.ValueFields.ContainsKey("Описание объявления"))
                    Description = package.ValueFields["Описание объявления"];
                if (package.ValueFields.ContainsKey("Цена"))
                    Price = package.ValueFields["Цена"];

                if (int.TryParse(package.Views, out int viewResult))
                {
                    Views = viewResult;
                }
                State = TranslateState(EnumHelper.GetValue<BulletinState>(package.State));
                Url = package.Url;
            }
        }


        string TranslateState(BulletinState state)
        {
            switch(state)
            {
                case BulletinState.Created:
                    return "Создано";
                case BulletinState.WaitPublication:
                    return "Размещается";
                case BulletinState.Publicated:
                    return "Опубликовано";
                case BulletinState.OnModeration:
                    return "Проверяется";
                case BulletinState.Rejected:
                    return "Отклонено";
                case BulletinState.Removed:
                    return "Удалено";
                case BulletinState.Closed:
                    return "Завершено";
                case BulletinState.Error:
                    return "Ошибка";
                default:
                    return string.Empty;
            }
        }
    }
}