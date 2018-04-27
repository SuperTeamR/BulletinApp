using FessooFramework.Tools.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebDriver.Containers
{
    public class BoardElement : _IOCElement
    {
        public BoardElement(string Name)
        {
            UID = Name;
        }



    }
}
