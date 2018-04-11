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
        public static BulletinInstance Convert(BulletinPackage obj)
        {
            BulletinInstance result = null;
            BCT.Execute(d =>
            {
                result = new BulletinInstance();
                result.State = obj.State;
                result.Url = obj.Url;
            });
            return result;
        }
    }
}
