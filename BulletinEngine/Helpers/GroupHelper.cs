using BulletinBridge.Data;
using BulletinEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinEngine.Helpers
{
    static class GroupHelper
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Создает сигнатуру группы из инстанции буллетина. </summary>
        ///
        /// <remarks>   SV Milovanov, 12.02.2018. </remarks>
        ///
        /// <param name="instanceId">   Identifier for the instance. </param>
        ///
        /// <returns>   The group signature. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static GroupSignature GetGroupSignature2(Guid bulletinId)
        {
            GroupSignature result = null;
            BCT.Execute(d =>
            {
                var dbBulletin = d.BulletinDb.Bulletins.FirstOrDefault(q => q.Id == bulletinId);
                var groupId = dbBulletin.GroupId;
                var dbGroup = d.BulletinDb.Groups.FirstOrDefault(q => q.Id == groupId);
                var groupHash = dbGroup.Hash;
                result = GetGroupSignature(groupHash);
            });
            return result;
        }

        [Obsolete]
        public static GroupSignature GetGroupSignature(Guid instanceId)
        {
            GroupSignature result = null;
            BCT.Execute(d =>
            {
                var dbInstance = d.BulletinDb.BulletinInstances.FirstOrDefault(q => q.Id == instanceId);
                var groupId = dbInstance.GroupId;
                var dbGroup = d.BulletinDb.Groups.FirstOrDefault(q => q.Id == groupId);
                var groupHash = dbGroup.Hash;
                result = GetGroupSignature(groupHash);
            });
            return result;
         }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Создает сигнатуру группы из хэша </summary>
        ///
        /// <remarks>   SV Milovanov, 12.02.2018. </remarks>
        ///
        /// <param name="hash"> The hash. </param>
        ///
        /// <returns>   The group signature. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static GroupSignature GetGroupSignature(string hash)
        {
            GroupSignature result = null;
            BCT.Execute(data =>
            {

                var group = data.BulletinDb.Groups.FirstOrDefault(q => q.Hash == hash);
                if (group != null)
                {
                    var chosenCategories = new List<string>();
                    var groupedCategories = data.BulletinDb.GroupedCategories.Where(q => q.GroupId == group.Id).Select(q => q.CategoryId).ToArray();
                    var categories = data.BulletinDb.CategoryTemplates.Where(q => groupedCategories.Contains(q.Id)).ToArray();
                    var topCategory = categories.FirstOrDefault(q => q.ParentId == Guid.Empty);
                    var nothandledCategories = categories.Where(q => q.Id != topCategory.Id).ToList();
                    var parentId = topCategory.Id;
                    chosenCategories.Add(topCategory.Name);
                    while (nothandledCategories.Count > 0)
                    {
                        for (var i = 0; i < nothandledCategories.Count; i++)
                        {
                            if (nothandledCategories[i].ParentId == parentId)
                            {
                                parentId = nothandledCategories[i].Id;
                                chosenCategories.Add(nothandledCategories[i].Name);
                                nothandledCategories.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    result = new GroupSignature(chosenCategories.ToArray());
                }
            });
            return result;
        }
    }
}
