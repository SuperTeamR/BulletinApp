using ParserTestApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserTestApp.Containers.Base
{
    public interface IBulletinContainer
    {
        string UID { get; set; }
        bool HasAuth { get; set; }

        void PublishBulletin(int bulletinId);
        void EditBulletin(int bulletinId);
        void UpdateBulletin(int bulletinId);
        void DisableBulletin(int bulletinId);
        void GetViewStatistics(int bulletinId);

        bool Execute();
    }
}
