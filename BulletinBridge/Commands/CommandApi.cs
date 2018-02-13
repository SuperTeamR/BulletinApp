using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Commands
{
    public enum CommandApi
    {
        Board_AddBulletins = 0,
        Board_EditBulletins = 1,
        Board_GetXlsForGroup = 2,
        Internal_GetBulletinWork = 3,
    }
}
