using BulletinBridge.Data;
using BulletinWebWorker.Containers.Base.Access;
using BulletinWebWorker.Helpers;
using BulletinWebWorker.Tools;
using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BulletinWebWorker.Containers.Fake
{
    class FakeAccessContainer : AccessContainerBase
    {
        public override Guid Uid => BoardIds.Fake;

        AccessPackage currentAccess { get; set; }
        public override bool TryAuth(AccessPackage access)
        {
            var result = false;
            DCT.Execute(d =>
            {
                if (currentAccess == null ||
                (currentAccess.Login != access.Login && currentAccess.Password != access.Password))
                {
                    currentAccess = access;
                    Exit();
                    result = Auth();
                }
                else
                {
                    result = true;
                }
            });
            return result;
        }

        protected override bool Auth()
        {
            var result = false;
            DCT.Execute(d =>
            {
                UiHelper.UpdateActionState("Поиск валидного прокси");
                ProxyManager.UseProxy();
                Thread.Sleep(1000);

                UiHelper.UpdateActionState("Навигация на страницу авторизации");
                Thread.Sleep(1000);

                UiHelper.UpdateActionState("Заполнение логина и пароля");
                Thread.Sleep(1000);

                UiHelper.UpdateActionState("Ожидание авторизации...");
                Thread.Sleep(5000);
                result = true;
            });
            return result;
        }

        protected override void Exit()
        {
            DCT.Execute(q =>
            {
                UiHelper.UpdateActionState("Выход из профиля");
                Thread.Sleep(1000);
            });
        }
    }
}
