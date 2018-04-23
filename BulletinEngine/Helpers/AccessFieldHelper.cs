using BulletinBridge.Data;
using BulletinEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulletinEngine.Helpers
{
    static class AccessFieldHelper
    {
        public static Dictionary<string, FieldPackage> GetAccessFields2(Guid bulletinId)
        {
            Dictionary<string, FieldPackage> result = new Dictionary<string, FieldPackage>();
            BCT.Execute(d =>
            {
                var dbBulletin = d.BulletinDb.Bulletins.FirstOrDefault(q => q.Id == bulletinId);
                var groupId = dbBulletin.GroupId;

                var groupedFields = d.BulletinDb.GroupedFields.Where(q => q.GroupId == groupId).ToArray();

                foreach (var gf in groupedFields)
                {
                    var accessId = gf.HtmlId;

                    var fieldId = gf.FieldId;
                    var dbField = d.BulletinDb.FieldTemplates.FirstOrDefault(q => q.Id == fieldId);
                    var name = dbField.Name;
                    var tag = dbField.Tag;
                    var hasId = dbField.Attribute == "id";

                    var options = d.BulletinDb.SelectOptions.Where(q => q.GroupedFieldId == gf.Id).ToArray();
                    var optionTags = new List<OptionTag>();
                    foreach (var o in options)
                    {
                        var optionTag = OptionTag.Create(o.Code, o.Name);
                        optionTags.Add(optionTag);
                    }
                    if (!result.ContainsKey(name))
                        result.Add(name, FieldPackage.Create(accessId, tag, hasId, optionTags.ToArray()));
                }
            });
            return result;
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Получает словарь правил доступа к полям </summary>
        ///
        /// <remarks>   SV Milovanov, 13.02.2018. </remarks>
        ///
        /// <param name="instanceId">   Identifier for the instance. </param>
        ///
        /// <returns>   The access fields. </returns>
        ///-------------------------------------------------------------------------------------------------
        [Obsolete]
        public static Dictionary<string, FieldPackage> GetAccessFields(Guid instanceId)
        {
            Dictionary<string, FieldPackage> result = new Dictionary<string, FieldPackage>();
            BCT.Execute(d =>
            {
                var dbInstance = d.BulletinDb.BulletinInstances.FirstOrDefault(q => q.Id == instanceId);
                var groupId = dbInstance.GroupId;

                var groupedFields = d.BulletinDb.GroupedFields.Where(q => q.GroupId == groupId).ToArray();

                foreach (var gf in groupedFields)
                {
                    var accessId = gf.HtmlId;

                    var fieldId = gf.FieldId;
                    var dbField = d.BulletinDb.FieldTemplates.FirstOrDefault(q => q.Id == fieldId);
                    var name = dbField.Name;
                    var tag = dbField.Tag;
                    var hasId = dbField.Attribute == "id";

                    var options = d.BulletinDb.SelectOptions.Where(q => q.GroupedFieldId == gf.Id).ToArray();
                    var optionTags = new List<OptionTag>();
                    foreach (var o in options)
                    {
                        var optionTag = OptionTag.Create(o.Code, o.Name);
                        optionTags.Add(optionTag);
                    }
                    if(!result.ContainsKey(name))
                        result.Add(name, FieldPackage.Create(accessId, tag, hasId, optionTags.ToArray()));
                }
            });
            return result;
        }
    }
}