using FessooFramework.Core;

namespace BulletinEngine.Core
{
    public class BCT : _DCT<BulletinContext>
    {
        public static void SaveChanges()
        {
            Context.SaveChanges();
        }
    }
}