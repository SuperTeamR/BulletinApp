using BulletinEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinEngine.Helpers
{
    static class ValueFieldHelper
    {


        public static Dictionary<string, string> GetValueFields2(Guid bulletinId)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            BCT.Execute(d =>
            {
                var dbBulletin = d.BulletinDb.Bulletins.FirstOrDefault(q => q.Id == bulletinId);
                result.Add("Название объявления", dbBulletin.Title);
                result.Add("Описание объявления", dbBulletin.Description);
                result.Add("Цена", dbBulletin.Price);
                result.Add("Вид объявления ", "Продаю свое");
                if (!string.IsNullOrEmpty(dbBulletin.Images))
                    result.Add("Фотографии", dbBulletin.Images);
            });
            return result;
        }



        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Получает словарь полей со значениями для инстанции буллетина </summary>
        ///
        /// <remarks>   SV Milovanov, 13.02.2018. </remarks>
        ///
        /// <param name="instanceId">   Identifier for the instance. </param>
        ///
        /// <returns>   The value fields. </returns>
        ///-------------------------------------------------------------------------------------------------

        [Obsolete]
        public static Dictionary<string, string> GetValueFields(Guid instanceId)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            BCT.Execute(d =>
            {
                var dbBulletinFields = d.BulletinDb.BulletinFields.Where(q => q.BulletinInstanceId == instanceId).ToArray();

                foreach(var dbf in dbBulletinFields)
                {
                    var value = dbf.Value;
                    var fieldId = dbf.FieldId;
                    var dbField = d.BulletinDb.FieldTemplates.FirstOrDefault(q => q.Id == fieldId);
                    var name = dbField.Name;

                    result.Add(name, value);
                }
            });
            return result;
        }

    }
}
