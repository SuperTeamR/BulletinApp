using BulletinExample.Core;
using BulletinExample.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample.Logic.API
{
    public static class ApplicationApi
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Авторизация пользователя в системе </summary>
        ///
        /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public static void Auth(string login, string pass)
        {
            DCT.Execute(data =>
            {
                var mac = NetworkInterface
                    .GetAllNetworkInterfaces()
                    .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    .Select(nic => nic.GetPhysicalAddress().ToString())
                    .FirstOrDefault();


                var hash = Сryptography.StringToSha256String(pass);
                var user = data.Db1.Users.FirstOrDefault(q => q.Login == login && q.Hash == hash);
                if (user == null)
                    user = RegistryUser(login, hash);

                data.Objects.CurrentUser = user;

                var app = data.Db1.Applications.FirstOrDefault(q => q.Token == mac);
                if (app == null)
                    app = RegistryApplication(mac, user.Id);


            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Регистрация пользователя в системе </summary>
        ///
        /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        static Entity.Data.Application RegistryApplication(string token, Guid userId)
        {
            Entity.Data.Application result = null;
            DCT.Execute(data =>
            {
                var app = new Entity.Data.Application();
                app.Token = token;
                app.UserId = userId;
                data.Db1.Applications.Add(app);
                data.Db1.SaveChanges();
            });
            return result;
        }

        static Entity.Data.User RegistryUser(string login, string hash)
        {
            Entity.Data.User result = null;
            DCT.Execute(data =>
            {
                var user = new Entity.Data.User();
                user.Login = login;
                user.Hash = hash;
                data.Db1.Users.Add(user);
                data.Db1.SaveChanges();
            });
            return result;
        }
    }
}
