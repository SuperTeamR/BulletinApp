using BulletinBridge.Messages.Base;
using BulletinEngine.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinEngine.Api
{
    public static class BoardApi
    {
        public static MessageBase GetUserMessage(MessageBase message)
        {
            return ServiceRouter.ExecuteRouting(message);
        }
    }
}
