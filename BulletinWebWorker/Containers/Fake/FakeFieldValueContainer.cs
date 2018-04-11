using BulletinBridge.Data;
using BulletinWebWorker.Containers.Base.FieldValue;
using BulletinWebWorker.Helpers;
using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BulletinWebWorker.Containers.Fake
{
    class FakeFieldValueContainer : FieldValueContainerBase
    {
        public override Guid Uid => BoardIds.Fake;

        public override string GetFieldValue(Dictionary<string, FieldPackage> fields, string name)
        { 
            DCT.Execute(d =>
            {
                UiHelper.UpdateActionState($"Получение значения для {name}");
                Thread.Sleep(1000);
            });
            return string.Empty;
        }

        public override void SetFieldValue(Dictionary<string, FieldPackage> fields, string name, string value)
        {
            DCT.Execute(d =>
            {
                UiHelper.UpdateActionState($"Установка {name} : {value}");
                Thread.Sleep(1000);
            });
        }
    }
}
