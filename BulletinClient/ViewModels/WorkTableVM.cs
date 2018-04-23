using BulletinBridge.Data;
using BulletinBridge.Services.ServiceModels;
using BulletinClient.Core;
using BulletinClient.Data;
using BulletinClient.Helpers;
using BulletinClient.HelperService;
using BulletinClient.Properties;
using FessooFramework.Objects.Delegate;
using FessooFramework.Objects.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BulletinClient.ViewModels
{
    class WorkTableVM : VM
    {
        #region Property
        public string Login => DataHelper.UserLogin.Value;
        public AccessCollectionVM AccessCollectionView => accessCollectionView = accessCollectionView ?? new AccessCollectionVM();
        public AccessCollectionVM accessCollectionView { get; set; }
        public BulletinCollectionVM BulletinCollectionView => bulletinCollectionView = bulletinCollectionView ?? new BulletinCollectionVM();
        public BulletinCollectionVM bulletinCollectionView { get; set; }
        public bool Bulletin => Bulletins != null && Bulletins.Any();
        public bool NotBulletin => Bulletins == null || !Bulletins.Any();
        public string CardCategory1 { get; set; }
        public string CardCategory2 { get; set; }
        public string CardCategory3  { get; set; }
        public string CardName { get; set; }
        public string CardPrice { get; set; }
        public string CardDescription { get; set; }
        public string CardImageLinks { get; set; }

        public ObservableCollection<NewBulletin> AddBulletins { get; set; }

        public IEnumerable<BulletinView> Bulletins { get; set; }

        public Brush ConnectionColor { get; set; }

        public int SelectedIndexTab { get; set; }
        public string UploadedImageLink { get; set; }

        #endregion
        #region Commands
        public ICommand CommandGetXls { get; set; }
        public ICommand CommandAddBulletin { get; set; }
        public ICommand CommandAddBulletins { get; set; }
        public ICommand CommandLogout { get; set; }
        public ICommand CommandAddAccess { get; set; }
        public ICommand CommandCloneBulletin { get; set; }
        public ICommand CommandCheckConnection { get; set; }
        public ICommand CommandAddImage { get; set; }
        public ICommand CommandSignIn { get; set; }
        public ICommand CommandGenerate { get; set; }


        #endregion
        #region Constructor
        public WorkTableVM()
        {
            CardCategory1 = "Бытовая электроника";
            CardCategory2 = "Телефоны";
            CardCategory3 = "iPhone";
            AddBulletins = new ObservableCollection<NewBulletin>();
          
            CommandAddBulletin = new DelegateCommand(()=>AddBulletin(CardName, CardDescription, CardPrice, CardImageLinks));
            CommandLogout = new DelegateCommand(Logout);
            CommandCheckConnection = new DelegateCommand(CheckConnection);
            CommandGenerate = new DelegateCommand(BulletinHelper.Generate);
            CommandCloneBulletin = new DelegateCommand<BulletinView>(CloneBulletin);
            CommandAddImage = new DelegateCommand(AddImage);
            CheckConnection();
            GetBulletins();
        }
        #endregion
        #region Methods
        private void Logout()
        {
            Settings.Default.BoardLogin = "";
            Settings.Default.BoardPassword = "";
            Settings.Default.Save();
            DataHelper.IsAuth.Value = false;
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
                Bulletins = objs.Select(q => new BulletinView(q));
                RaisePropertyChanged(() => Bulletins);
                RaisePropertyChanged(() => Bulletin);
                RaisePropertyChanged(() => NotBulletin);

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
            if(obj == null)
            {
                MessageBox.Show("Ошибка соединения с сервером");
                return;
            }
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