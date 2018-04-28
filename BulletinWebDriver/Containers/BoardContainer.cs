using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletinBridge.Models;
using BulletinWebDriver.Core;
using BulletinWebDriver.Helpers;
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
            DCT.Execute(c =>
            {
                if (task == null)
                {
                    ConsoleHelper.SendMessage("BoardContainer 'task' is null");
                    return;
                }
                if (task.Board == null)
                {
                    DriverTaskHelper.Complete(task);
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
