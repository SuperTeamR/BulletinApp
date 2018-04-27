using BulletinBridge.Data;
using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using BulletinEngine.Helpers;

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
                    BulletinId = obj.BulletinId,
                    BulletinInstanceId = obj.Id,
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
        public static BulletinInstance Convert(BulletinPackage obj, BulletinInstance entity)
        {
            BulletinInstance result = null;
            BCT.Execute(d =>
            {
                entity.State = obj.State;
                entity.Url = obj.Url;

                result = entity;
            });
            return result;
        }
    }
}