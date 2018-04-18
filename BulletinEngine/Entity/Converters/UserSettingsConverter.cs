using BulletinBridge.Data;
using BulletinEngine.Core;
using BulletinHub.Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinHub.Entity.Converters
{
    class UserSettingsConverter
    {
        public static UserSettingsCache Convert(UserSettings obj)
        {
            UserSettingsCache result = null;
            BCT.Execute(d =>
            {
            });
            return result;
        }
        public static UserSettings Convert(UserSettingsCache obj, UserSettings entity)
        {
            var result = default(Data.UserSettings);
            BCT.Execute(d =>
            {
                result.TaskGenerationPeriod = 7 * 24;
                entity = result;
            });
            return entity;
        }
    }
}
