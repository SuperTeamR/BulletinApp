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

                result = new BulletinPackage
                {
                    Url = obj.Url,
                    Signature = groupSignature,
                    Access = access,
                    ValueFields = valueFields,
                    AccessFields = accessFields
                };
            });
            return result;
        }
    }
}
