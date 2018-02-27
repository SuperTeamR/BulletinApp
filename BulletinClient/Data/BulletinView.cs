using BulletinBridge.Data;
using FessooFramework.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinClient.Data
{
    class BulletinView
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Views { get; set; }
        public string State { get; set; }

        public BulletinView(BulletinPackage package)
        {
            if(package.ValueFields != null)
            {
                if(package.ValueFields.ContainsKey("Название объявления"))
                    Title = package.ValueFields["Название объявления"];
                if (package.ValueFields.ContainsKey("Описание объявления"))
                    Description = package.ValueFields["Описание объявления"];

                if (int.TryParse(package.Views, out int viewResult))
                {
                    Views = viewResult;
                }
                State = EnumHelper.GetValue<BulletinState>(package.State).ToString();
            }
        }
    }
}
