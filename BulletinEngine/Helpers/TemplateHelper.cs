using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using BulletinHub.Models;
using FessooFramework.Objects.Data;

namespace BulletinHub.Helpers
{
    static class TemplateHelper
    {
        public static IEnumerable<BulletinTemplate> All()
        {
            var result = Enumerable.Empty<BulletinTemplate>();
            BCT.Execute(c =>
            {
                result = c.TempDB.BulletinTemplate.Where(q => q.State != (int)DefaultState.Disable).ToArray();
            });
            return result;
        }

        public static BulletinTemplate MarkAsUsed(BulletinTemplate template)
        {
            BCT.Execute(c =>
            {
                template.StateEnum = DefaultState.Disable;
                c.SaveChanges();
            });
            return template;
        }
    }
}