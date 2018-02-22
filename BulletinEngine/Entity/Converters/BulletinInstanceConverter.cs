using BulletinBridge.Data;
using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using BulletinEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinEngine.Entity.Converters
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Конвертер инстанции буллетинов </summary>
    ///
    /// <remarks>   SV Milovanov, 13.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    static class BulletinInstanceConverter
    {
        public static BulletinPackage Convert(BulletinInstance obj)
        {
            BulletinPackage result = null;
            BCT.Execute(d =>
            {
                var groupSignature = GroupHelper.GetGroupSignature(obj.Id);
                var access = AccessHelper.GetFreeAccess(obj.Id);
                var valueFields = ValueFieldHelper.GetValueFields(obj.Id);
                var accessFields = AccessFieldHelper.GetAccessFields(obj.Id);

                var state = obj.State;

                result = new BulletinPackage
                {
                    Url = obj.Url,
                    Signature = groupSignature,
                    Access = access,
                    ValueFields = valueFields,
                    AccessFields = accessFields,
                    State = state,
                    Title = obj.Url,
                };
            });
            return result;
        }
        public static BulletinInstance Convert(BulletinPackage obj)
        {
            BulletinInstance result = null;
            BCT.Execute(d =>
            {
                var state = obj.State;
                var hash = obj.Signature.GetHash();
                var dbGroup = d.Db1.Groups.FirstOrDefault(q => q.Hash == hash);
                var dbAccess = d.Db1.Accesses.FirstOrDefault(q => q.Login == obj.Access.Login
                      && q.Password == obj.Access.Password);
                result = new BulletinInstance
                {
                    Url = obj.Url,
                    State = state,
                    GroupId = dbGroup.Id,
                    AccessId = dbAccess == null ? Guid.Empty : dbAccess.Id,
                };
                foreach (var field in obj.ValueFields)
                {
                    var dbField = d.Db1.FieldTemplates.FirstOrDefault(q => q.Name == field.Key);
                    if (dbField != null)
                    {
                        var dbBulletinField = new BulletinField
                        {
                            BulletinInstanceId = result.Id,
                            FieldId = dbField.Id,
                            Value = field.Value,
                        };
                        dbBulletinField.StateEnum = BulletinFieldState.Filled;
                    }
                }

            });
            return result;
        }
    }
}
