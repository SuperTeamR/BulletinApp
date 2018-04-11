﻿using BulletinBridge.Data;
using BulletinBridge.Messages.BoardApi;
using BulletinBridge.Services.ServiceModels;
using BulletinClient.Core;
using BulletinClient.Data;
using BulletinClient.Helpers;
using BulletinClient.Properties;
using FessooFramework.Objects.Data;
using FessooFramework.Objects.Delegate;
using FessooFramework.Objects.ViewModel;
using FessooFramework.Tools.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BulletinClient.Forms.MainView
{
    class ViewModel : VM
    {
        #region Property
        public string NewAccessLogin { get; set; }
        public string NewAccessPassword { get; set; }

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
        public ObservableCollection<AccessView> Accesses { get; set; }

        public Brush ConnectionColor { get; set; }

        public int SelectedIndexTab { get; set; }
        public string UploadedImageLink { get; set; }

        #endregion
        #region Commands
        public ICommand CommandGetXls { get; set; }
        public ICommand CommandBoardAuth { get; set; }
        public ICommand CommandAddBulletin { get; set; }
        public ICommand CommandAddBulletins { get; set; }
        public ICommand CommandLogout { get; set; }
        public ICommand CommandAddAccess { get; set; }

        public ICommand CommandCloneBulletin { get; set; }

        public ICommand CommandCheckConnection { get; set; }
        public ICommand CommandAddImage { get; set; }
        #endregion
        #region Constructor
        public ViewModel()
        {
            CardCategory1 = "Бытовая электроника";
            CardCategory2 = "Телефоны";
            CardCategory3 = "iPhone";
            AddBulletins = new ObservableCollection<NewBulletin>();
            Accesses = new ObservableCollection<AccessView>();
            CommandGetXls = new DelegateCommand(GetXls);
            CommandBoardAuth = new DelegateCommand(BoardAuth);
            CommandAddBulletin = new DelegateCommand(()=>AddBulletin(CardName, CardDescription, CardPrice, CardImageLinks));
            CommandLogout = new DelegateCommand(Logout);
            CommandAddAccess = new DelegateCommand(AddAccess);
            CommandCheckConnection = new DelegateCommand(CheckConnection);
            CheckConnection();
            GetAccesses();
            GetBulletins();
        
            CommandAddBulletins = new DelegateCommand(AddBulletinsCollections);
            CommandCloneBulletin = new DelegateCommand<BulletinView>(CloneBulletin);
            CommandAddImage = new DelegateCommand(AddImage);
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
                    AddBulletin(bulletin.Заголовок, bulletin.Описание, bulletin.Цена, bulletin.Ссылка);
                }
            }
        }
        #endregion



        void AddImage()
        {
            DCT.ExecuteMainThread(d =>
            {
                var openFile = new Microsoft.Win32.OpenFileDialog
                {
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    FilterIndex = 2,
                    RestoreDirectory = true
                };
                if (openFile.ShowDialog() == true)
                {
                    using (var client = new ImageService())
                    {
                        var r = client.Ping();
                        Console.WriteLine($"Ping = {r}");
                        var bitmap = new BitmapImage(new Uri(openFile.FileName));
                        var bytes = ImageHelper.ConvertImageToByte(bitmap);
                        client.AddImage(AddImageCallback, System.IO.Path.GetFileNameWithoutExtension(openFile.SafeFileName), bytes);
                    }


                    //data.Property.Profile.Value.Avatar = ImageHelper.ConvertImageToByte(new BitmapImage(new Uri(ОткрытьФайл.FileName)));
                    //CacheData.UserProfiles._Update(data.Property.Profile.Value);
                }

            });
           
        }

        void AddImageCallback(Response_AddImage result)
        {
            UploadedImageLink = result.Url;
            RaisePropertyChanged(() => UploadedImageLink);
        }
        void CheckConnection()
        {
            DCT.Execute(d =>
            {
                ConnectionColor = Brushes.Orange;
                RaisePropertyChanged(() => ConnectionColor);
                using (var client = new ServiceClient())
                {
                    var ping = client.Ping();
                    if (ping)
                        ConnectionColor = Brushes.Green;
                    else
                        ConnectionColor = Brushes.Red;

                    RaisePropertyChanged(() => ConnectionColor);
                }
            });
        }

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

        void GetBulletins()
        {
            using (var client = new ServiceClient())
            {
                client.CollectionLoad<BulletinPackage>(GetBulletinsCallback);
            }
        }

        void GetBulletinsCallback(IEnumerable<BulletinPackage> objs)
        {
            DCT.Execute(d =>
            {
                if (objs.Count() == 0) return;
                var grouped = objs.Select(q => new BulletinView(q)).GroupBy(q => q.BulletinId);
                var temp = new List<BulletinView>();
                foreach(var g in grouped)
                {
                    var bulletin = g.FirstOrDefault();
                    bulletin.CanRepublicate = g.Count() < Accesses.Count;
                    temp.Add(bulletin);
                } 
                Bulletins = temp;
                RaisePropertyChanged(() => Bulletins);
                RaisePropertyChanged(() => Bulletin);
                RaisePropertyChanged(() => NotBulletin);

            });
        }

        void GetAccesses()
        {
            using (var client = new ServiceClient())
            {
                client.CollectionLoad<AccessPackage>(GetAccessesCallback);
            }
        }

        void GetAccessesCallback(IEnumerable<AccessPackage> objs)
        {
            Accesses = new ObservableCollection<AccessView>(objs.Select(q => new AccessView(q)).ToArray());
            RaisePropertyChanged(() => Accesses);
        }

        void AddAccess()
        {
            DCT.Execute(d => 
            {
                var access = new AccessPackage
                {
                    Login = NewAccessLogin,
                    Password = NewAccessPassword,
                };

                using (var client = new ServiceClient())
                {
                    var r = client.Ping();
                    Console.WriteLine($"Ping = {r}");
                    client.SendQueryCollection(NewAccessAdded, "CreateAccesses", objects: new[] { access });
                }
            });
        }

        void NewAccessAdded(IEnumerable<AccessPackage> a)
        {
            DCT.Execute(d =>
            {
                MessageBox.Show("Учетная запись была добавлена");

                NewAccessLogin = string.Empty;
                NewAccessPassword = string.Empty;
                RaisePropertyChanged(() => NewAccessLogin);
                RaisePropertyChanged(() => NewAccessPassword);

                GetAccesses();
            });
        }

        void CloneBulletin(BulletinView bulletin)
        {
            var package = new BulletinPackage
            {
                BulletinId = bulletin.BulletinId
            };
            using (var client = new ServiceClient())
            {
                var r = client.Ping();
                Console.WriteLine($"Ping = {r}");

                ServiceClient._CloneBulletin(package, CloneBulletinCallback);
            }

        }

        void CloneBulletinCallback(BulletinPackage bulletin)
        {
            MessageBox.Show("Объявление отправлено на републикацию");

            var b = Bulletins.FirstOrDefault(q => q.BulletinId == bulletin.BulletinId);
            b.CanRepublicate = false;
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
                    fields.Add(@"Фотографии", cardImageLinks);
                }
                var package = new BulletinPackage
                {
                    Signature = signature,
                    ValueFields = fields,
                    Access = access,
                };
                ServiceClient._CreateBulletin(package, AddBulletinCallback);
            });
        }

        private void AddBulletinCallback(BulletinPackage obj)
        {
            MessageBox.Show("Объявление было добавлено");
            SelectedIndexTab = 2;
            RaisePropertyChanged(() => SelectedIndexTab);
            GetBulletins();
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
