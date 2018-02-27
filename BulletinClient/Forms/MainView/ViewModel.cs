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
using System.Collections.ObjectModel;
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

        public string CardCategory1 { get; set; }
        public string CardCategory2 { get; set; }
        public string CardCategory3  { get; set; }
        public string CardName { get; set; }
        public string CardPrice { get; set; }
        public string CardDescription { get; set; }
        public string CardImageLinks { get; set; }

        public ObservableCollection<NewBulletin> AddBulletins { get; set; }

        public IEnumerable<BulletinView> Bulletins { get; set; }

        #endregion
        #region Commands
        public ICommand CommandGetXls { get; set; }
        public ICommand CommandBoardAuth { get; set; }
        public ICommand CommandAddBulletin { get; set; }
        public ICommand CommandAddBulletins { get; set; }
        public ICommand CommandLogout { get; set; }
        #endregion
        #region Constructor
        public ViewModel()
        {
            CardCategory1 = "Хобби и отдых";
            CardCategory2 = "Спорт и отдых";
            CardCategory3 = "Другое";
            AddBulletins = new ObservableCollection<NewBulletin>();
            CommandGetXls = new DelegateCommand(GetXls);
            CommandBoardAuth = new DelegateCommand(BoardAuth);
            CommandAddBulletin = new DelegateCommand(()=>AddBulletin(CardName, CardDescription, CardPrice, CardImageLinks));
            CommandLogout = new DelegateCommand(Logout);
            GetBulletins();
            CommandAddBulletins = new DelegateCommand(AddBulletinsCollections);
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

        private void AddBulletinsCollections()
        {
            if (AddBulletins.Any())
            {
                foreach (var bulletin in AddBulletins)
                {
                    AddBulletin(bulletin.Заголовок, bulletin.Описание, bulletin.Цена,);
                }
            }
        }
        #endregion






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

        void AddBulletin(string cardName, string cardDescription, string cardPrice, string cardImageLinks)
        {
            DCT.Execute(d =>
            {
                if(string.IsNullOrEmpty(cardName)
                || string.IsNullOrEmpty(cardDescription)
                || string.IsNullOrEmpty(cardPrice))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля для добавления объявления");
                    return;
                }
                var access = new AccessPackage
                {
                    Login = Settings.Default.BoardLogin,
                    Password = Settings.Default.BoardPassword,
                };
                var signature = new GroupSignature(CardCategory1, CardCategory2, CardCategory3);
                var fields = new Dictionary<string, string>
                {
                    {"Вид объявления", "Продаю свое" },
                    {"Название объявления", cardName },
                    {"Описание объявления", cardDescription },
                    {"Цена", cardPrice },
                };
                if(!string.IsNullOrEmpty(cardImageLinks))
                {
                    fields.Add(@"Фотографии 
Вы можете прикрепить не более 10 фотографий", cardImageLinks);
                }
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


    public class NewBulletin
    {
        public string Заголовок { get; set; }
        public string Описание { get; set; }
        public string Цена { get; set; }
        public string Ссылка { get; set; }
    }
}
