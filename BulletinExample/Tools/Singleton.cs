using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample.Tools
{
    public class Singleton<T>
        where T : class, new()
    {
        public static T Instance => instance = instance ?? new T();
        private static T instance { get; set; }
    }
}
