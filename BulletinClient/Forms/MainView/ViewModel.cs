using BulletinBridge.Data;
using BulletinBridge.Messages.BoardApi;
using BulletinClient.Core;
using BulletinClient.Data;
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
        public ICommand CommandAddBulletin { get; set; }
        public ICommand CommandLogout { get; set; }
        #endregion
        #region Constructor
        public ViewModel()
        {
            CommandGetXls = new DelegateCommand(GetXls);
            CommandBoardAuth = new DelegateCommand(BoardAuth);
            CommandAddBulletin = new DelegateCommand(AddBulletin);
            CommandLogout = new DelegateCommand(Logout);
            GetBulletins();
        }
        #endregion
        #region Methods
        private void Logout()
        {
            Settings.Default.BoardLogin = "";
            Settings.Default.BoardPassword = "";
            Settings.Default.Save();
            Bulletins = Enumerable.Empty<BulletinView>();
            RaiseDone();
        }
        #endregion
        public string BulletinName { get; set; }
        public string BulletinPrice { get; set; }
        public string BulletinDescription { get; set; }
        public IEnumerable<BulletinView> Bulletins { get; set; }

     

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

        private void GetBulletins()
        {
            using (var client = new ServiceClient())
            {
                client.CollectionLoad<BulletinPackage>(GetBulletinsCallback);
            }
        }
        private void GetBulletinsCallback(IEnumerable<BulletinPackage> objs)
        {
            Bulletins = objs.Select(q => new BulletinView(q)).ToArray();
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

                GetBulletins();
            });
        }

        void AddBulletin()
        {
            DCT.Execute(d =>
            {
                if(string.IsNullOrEmpty(BulletinName)
                || string.IsNullOrEmpty(BulletinDescription)
                || string.IsNullOrEmpty(BulletinPrice))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля для добавления объявления");
                    return;
                }
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
                    {"Описание объявления", BulletinDescription },
                    {"Цена", BulletinPrice }
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
        }

        private void AddBulletinCallback(BulletinPackage obj)
        {
            MessageBox.Show("Объявление было добавлено");
        }
    }
}
