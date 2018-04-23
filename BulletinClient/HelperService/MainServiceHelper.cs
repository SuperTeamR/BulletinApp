using BulletinClient.Core;
using System;

namespace BulletinClient.HelperService
{
    public static class MainServiceHelper
    {
        public static void SignIn(string email, string password, Action<bool> callback)
        {
            DCT.Execute(c =>
            {
                using (var main = new MainService())
                {
                    var ping = main.Ping();
                    main.SignIn(callback, email, password);
                }

            });
        }
        public static void Registration(string email, string password, string firstname, string secondname, string middlename,  Action<bool> callback)
        {
            DCT.Execute(c =>
            {
                using (var main = new MainService())
                {
                    var ping = main.Ping();
                    main.Registration(callback, email, "799977777", password, firstname, secondname, middlename);
                }
            });
        }
    }
}