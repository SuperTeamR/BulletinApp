using BulletinBridge.Data;
using BulletinEngine.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinHub.Service
{
    public static class AccessResultHelper
    {

        public static IEnumerable<AccessPackage> MarkAccessAsChecked(IEnumerable<AccessPackage> packages)
        {
            var result = Enumerable.Empty<AccessPackage>(); ;
            BCT.Execute(d =>
            {
                foreach(var p in packages)
                {
                    var dbAccess = d.Db1.Accesses.FirstOrDefault(q => q.Login == p.Login && q.Password == p.Password);
                    if(dbAccess != null)
                    {
                        dbAccess.StateEnum = BulletinEngine.Entity.Data.AccessState.Activated;
                        d.Db1.SaveChanges();
                    }
                }
            });
            return result;
        }
    }
}
