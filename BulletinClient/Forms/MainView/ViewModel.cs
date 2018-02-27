using BulletinBridge.Data;
using BulletinBridge.Messages.BoardApi;
using BulletinClient.Core;
using BulletinClient.Properties;
using FessooFramework.Objects.Data;
using FessooFramework.Objects.Delegate;
using FessooFramework.Objects.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace BulletinClient.Forms.MainView
{
    class ViewModel : VM
    {
        #region Proiperty
        public bool Auth => !string.IsNullOrEmpty(Settings.Default.BoardLogin);
        public bool NotAuth => !Auth;

        public bool Bulletin => Bulletins != null && Bulletins.Any();
        public bool NotBulletin => Bulletins == null || !Bulletins.Any();

        public string Login { get; set; }
        public string Password { get; set; }
        public string BoardLogin
        {
            get { return Settings.Default.BoardLogin; }
        }
        #endregion
        #region Commands
        public ICommand CommandGetXls { get; set; }
        public ICommand CommandBoardAuth { get; set; }
        public ICommand CommandAppAuth { get; set; }
        public ICommand CommandAddBulletin { get; set; }
        public ICommand CommandLogout { get; set; }
        #endregion
        #region Constructor
        public ViewModel()
        {
            CommandGetXls = new DelegateCommand(GetXls);
            CommandBoardAuth = new DelegateCommand(BoardAuth);
            CommandAppAuth = new DelegateCommand(ApplicationAuth);
            CommandAddBulletin = new DelegateCommand(AddBulletin);
            CommandLogout = new DelegateCommand(Logout);
            BulletinName = "Варежки";
            GetBulletins();
        }
        #endregion
        #region Methods
        private void Logout()
        {
            Settings.Default.BoardLogin = "";
            Settings.Default.BoardPassword = "";
            Settings.Default.Save();
            Bulletins = Enumerable.Empty<BulletinPackage>();
            RaiseDone();
        }
        #endregion
        public string BulletinName { get; set; }
        public IEnumerable<BulletinPackage> Bulletins { get; set; }

     

        void GetXls()
        {
            DCT.Execute(d =>
            {
                var group = new GroupSignature();
                var request = new RequestBoardAPI_GetXlsForGroup
                {
                    GroupSignature = group
                };
                //ClientService.ExecuteQuery<RequestBoardAPI_GetXlsForGroup, ResponseBoardAPI_GetXlsForGroup>(request, BulletinBridge.Commands.CommandApi.Board_GetXlsForGroup);
            });
        }
        string AuthLogin = "";
        string AuthPassword = "";
        void ApplicationAuth()
        {
            DCT.Execute(d =>
            {
                using (var main = new MainService())
                {
                    var ping = main.Ping();
                    if (ping)
                        Registration(RegistrationCallback);
                }
            });
        }
        void Registration(Action<bool> callback)
        {
            using (var main = new MainService())
            {
                AuthLogin = "ttt3@ttt.ru";
                AuthPassword = "799988888";
                var password = "ttt3";
                var firstname = "name";
                var secondname = "sec";
                var middlename = "sec";
                main.Registration(callback, AuthLogin, AuthPassword, password, firstname, secondname, middlename);
            }
        }
        void RegistrationCallback(bool result)
        {
            DCT.Execute(d =>
            {
                if (result)
                    Console.WriteLine($"Registration succesfull");
                else
                    Console.WriteLine($"Registration not sucessfull");
                SignIn(SignInCallback, AuthLogin, AuthPassword);
            });
        }
        void SignIn(Action<bool> callback, string email, string password)
        {
            using (var main = new MainService())
                main.SignIn(callback, email, password);
        }
        void SignInCallback(bool result)
        {
            DCT.Execute(d =>
            {
                if (result)
                    Console.WriteLine($"Signin succesfull");
                else
                    Console.WriteLine($"Signin not sucessfull");
                GetBulletins();
            });
        }
        private void GetBulletins()
        {
            using (var client = new ServiceClient())
            {
                client.CollectionLoad<BulletinPackage>(GetBulletinsCallback);
            }
        }
        private void GetBulletinsCallback(IEnumerable<BulletinPackage> objs)
        {
            Bulletins = objs;
            RaisePropertyChanged(() => Bulletins);
            RaisePropertyChanged(() => Bulletin);
            RaisePropertyChanged(() => NotBulletin);

        }

        void BoardAuth()
        {
            DCT.Execute(d =>
            {
                var access = new AccessPackage
                {
                    Login = Login,
                    Password = Password,
                };
                using (var client = new ServiceClient())
                {
                    client.Save<AccessPackage>((a)=>BoardAuthCallback(a), access);
                }
            });
        }

        private void BoardAuthCallback(AccessPackage access)
        {
            DCT.Execute(c => {
                Settings.Default.SessionUID = c._SessionInfo.SessionUID;
                Settings.Default.HashUID = c._SessionInfo.HashUID;

                Settings.Default.BoardLogin = access.Login;
                Settings.Default.BoardPassword = access.Password;
                Settings.Default.Save();

                RaisePropertyChanged(() => Auth);
                RaisePropertyChanged(() => NotAuth);
                RaisePropertyChanged(() => BoardLogin);
            });
        }

        void AddBulletin()
        {
            DCT.Execute(d =>
            {
                var access = new AccessPackage
                {
                    Login = Settings.Default.BoardLogin,
                    Password = Settings.Default.BoardPassword,
                };
                var signature = new GroupSignature("Хобби и отдых", "Спорт и отдых", "Другое");
                var fields = new Dictionary<string, string>
                {
                    {"Вид объявления", "Продаю свое" },
                    {"Название объявления", BulletinName },
                    {"Описание объявления", "Очень теплые" },
                    {"Цена", "300" }
                };
                var package = new BulletinPackage
                {
                    Signature = signature,
                    ValueFields = fields,
                    Access = access,
                };
                using (var client = new ServiceClient())
                {
                    var result = client.Ping();
                    Console.WriteLine($"Ping = {result}");
                    client.Save<BulletinPackage>(AddBulletinCallback, package);
                }
            });
            GetBulletins();
        }

        private void AddBulletinCallback(BulletinPackage obj)
        {
            MessageBox.Show("Объявление было добавлено");
        }
    }
}
