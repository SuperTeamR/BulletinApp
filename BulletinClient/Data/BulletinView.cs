using BulletinBridge.Data;
using FessooFramework.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinClient.Data
{
    class BulletinView
    {
        public Guid BulletinId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public int Views { get; set; }
        public string State { get; set; }
        public string Url { get; set; }
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
