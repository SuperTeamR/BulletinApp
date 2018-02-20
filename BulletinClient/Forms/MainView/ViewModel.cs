using BulletinBridge.Data;
using BulletinBridge.Messages.BoardApi;
using BulletinClient.Core;
using BulletinClient.Properties;
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
        public Visibility HasAccess
        {
            get
            {
                if (!string.IsNullOrEmpty(Settings.Default.BoardLogin))
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public Visibility DontHasAccess
        {
            get
            {
                if (HasAccess == Visibility.Visible) return Visibility.Collapsed;
                else return Visibility.Visible;
            }
        }
        public string Login { get; set; }
        public string Password { get; set; }
        public string BulletinName { get; set; }

        public string BoardLogin
        {
            get { return Settings.Default.BoardLogin; }
        }

        public IEnumerable<BulletinPackage> Bulletins { get; set; }
        public ICommand CommandGetXls { get; set; }
        public ICommand CommandBoardAuth { get; set; }
        public ICommand CommandAppAuth { get; set; }
        public ICommand CommandAddBulletin { get; set; }

        public ViewModel()
        {
            CommandGetXls = new DelegateCommand(GetXls);
            CommandBoardAuth = new DelegateCommand(BoardAuth);
            CommandAppAuth = new DelegateCommand(ApplicationAuth);
            CommandAddBulletin = new DelegateCommand(AddBulletin);

            BulletinName = "Варежки";
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

        void ApplicationAuth()
        {
            DCT.Execute(d =>
            {
                using (var main = new MainService())
                {
                    var ping = main.Ping();
                    if(ping)
                    {
                        var email = "ttt3@ttt.ru";
                        var phone = "799988888";
                        var password = "ttt3";
                        var firstname = "name";
                        var secondname = "sec";
                        var middlename = "sec";
                        var registration = main.Registration(email, phone, password, firstname, secondname, middlename);
                        if (registration)
                            Console.WriteLine($"Registration succesfull");
                        else
                            Console.WriteLine($"Registration not sucessfull");

                        var signin = main.SignIn(email, password);
                        if (signin)
                            Console.WriteLine($"Signin succesfull");
                        else
                            Console.WriteLine($"Signin not sucessfull");
                    }
                }
            });
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
                var request = new RequestAddAccessModel
                {
                    Objects = new[] { access }
                };

                using (var client = new ServiceClient())
                {
                    var response = client.Execute<RequestAddAccessModel, ResponseAddAccessModel>(request);

                    if(response.State == ResponseState.Success)
                    {
                        access = response.Objects.FirstOrDefault();
                        Settings.Default.BoardLogin = access.Login;
                        Settings.Default.BoardPassword = access.Password;
                        Settings.Default.Save();

                        RaisePropertyChanged(() => HasAccess);
                        RaisePropertyChanged(() => DontHasAccess);
                        RaisePropertyChanged(() => BoardLogin);
                    }
                }
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
                var request = new RequestAddBulletinsModel
                {
                    Objects = new[] { package }
                };
                using (var client = new ServiceClient())
                {
                    var result = client.Ping();
                    Console.WriteLine($"Ping = {result}");
                    var response = client.Execute<RequestAddBulletinsModel, ResponseAddBulletinsModel>(request);
                }
            });
        }
    }
}
