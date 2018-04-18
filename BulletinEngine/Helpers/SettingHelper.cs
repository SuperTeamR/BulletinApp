using BulletinEngine.Core;
using BulletinHub.Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinHub.Helpers
{
    static class SettingsHelper 
    {

        public static UserSettings GetSettings(Guid userId)
        {
            UserSettings result = null;
            BCT.Execute(d =>
            {
                var settings = d.BulletinDb.UserSettings.FirstOrDefault(q => q.UserId == userId);
                if(settings == null)
                {
                    settings = new UserSettings
                    {
                        UserId = userId,
                        TaskGenerationPeriod = 24 * 7,
                    };
                    settings.StateEnum = UserSettingsState.Created;

                    d.BulletinDb.SaveChanges();
                }
                result = settings;
            });
            return result;
        }
    }
}
