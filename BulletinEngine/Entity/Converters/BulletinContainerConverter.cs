using BulletinBridge.Data;
using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinHub.Entity.Converters
{
    static class BulletinContainerConverter
    {
        public static Bulletin Convert(BulletinPackage obj, Bulletin entity)
        {
            var result = default(Bulletin);
            BCT.Execute(d =>
            {
                var hash = obj.Signature.GetHash();
                var dbGroup = d.BulletinDb.Groups.FirstOrDefault(q => q.Hash == hash);
                var bulletinTitle = obj.ValueFields["Название объявления"];
                var bulletinDesc = obj.ValueFields["Описание объявления"];
                var bulletinPrice = obj.ValueFields["Цена"];
                var bulletinImages = obj.ValueFields.ContainsKey("Фотографии") ? obj.ValueFields["Фотографии"] : null;


                entity.UserId = d.UserId;
                entity.GroupId = dbGroup.Id;
                entity.Title = bulletinTitle;
                entity.Description = bulletinDesc;
                entity.Price = bulletinPrice;
                entity.Images = bulletinImages;

                result = entity;
            });
            return result;
        }

        public static BulletinPackage Convert(Bulletin obj)
        {
            var result = default(BulletinPackage);
            BCT.Execute(d =>
            {
                var package = new BulletinPackage();
                var valueFields = new Dictionary<string, string>();
                valueFields.Add("Название объявления", obj.Title);
                valueFields.Add("Описание объявления", obj.Description);
                valueFields.Add("Цена", obj.Price);
                valueFields.Add("Вид объявления ", "Продаю свое");
                if (string.IsNullOrEmpty(obj.Images))
                    valueFields.Add("Фотографии", obj.Images);
                package.ValueFields = valueFields;
                package.Title = obj.Title;

                result = package;

            });
            return result;
        }
    }
}
