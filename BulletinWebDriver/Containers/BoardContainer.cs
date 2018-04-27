using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletinBridge.Models;
using BulletinEngine.Core;
using FessooFramework.Tools.Helpers;

namespace BulletinWebDriver.Containers
{
    public class BoardContainer : FessooFramework.Tools.IOC.IOContainer<BoardElement>
    {
        public BoardContainer()
        {
            Add(new BulletinWebDriver.Containers.BoardRealizations.Avito());
        }

        internal void Execute(TaskCache task)
        {
            BCT.Execute(c =>
            {
                if (task == null)
                {
                    ConsoleHelper.SendMessage("BoardContainer 'task' is null");
                    return;
                }
                var element = GetByName(task.Board);
                if (element == null)
                {
                    ConsoleHelper.SendMessage("BoardContainer not found realizations");
                    return;
                }
                element.Execute(task);
            });
        }
    }
}
