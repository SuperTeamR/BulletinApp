using BulletinBridge.Data;
using BulletinWebWorker.Containers.Base;
using BulletinWebWorker.Containers.Base.Access;
using BulletinWebWorker.Containers.Base.FieldValue;
using BulletinWebWorker.Helpers;
using FessooFramework.Tools.DCT;
using FessooFramework.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BulletinWebWorker.Containers.Fake
{
    class FakeBulletinPackageContainer : BulletinPackageContainerBase
    {
        public override Guid Uid => BoardIds.Fake;

        public override void AddBulletins(IEnumerable<BulletinPackage> packages)
        {
            UiHelper.UpdateWorkState("Добавление списка буллетинов");
            DCT.Execute(d =>
            {
                var createdBulletins = new List<BulletinPackage>();

                {
                    var fieldValueContainer = FieldValueContainerList.Get(Uid);
                    var accessContainer = AccessContainerList.Get(Uid);

                    foreach (var bulletin in packages)
                    {
                        var name = bulletin.ValueFields["Название объявления"];
                        UiHelper.UpdateObjectState($"Bulletin {name}, state = {bulletin.State}");

                        UiHelper.UpdateActionState("Попытка авторизоваться");
                        if (accessContainer.TryAuth(bulletin.Access))
                        {
                            UiHelper.UpdateActionState("Ожидание прогрузки страницы");
                            Thread.Sleep(2000);

                            UiHelper.UpdateActionState("Переход на страницу - additem");
                            Thread.Sleep(1000);

                            UiHelper.UpdateActionState("Выбор категорий");
                            ChooseCategories(bulletin.Signature);

                            UiHelper.UpdateActionState("Установка значений");
                            Thread.Sleep(1000);
                            SetValueFields(bulletin, fieldValueContainer);
                            Thread.Sleep(1000);

                            ContinueAddOrEdit(EnumHelper.GetValue<BulletinState>(bulletin.State));
                            Thread.Sleep(1000);

                            Publicate(bulletin);
                            Thread.Sleep(1000);

                            GetUrl(bulletin);
                            Thread.Sleep(1000);
                        }
                    }
                }

                {
                    DCT.ExecuteAsync(d2 =>
                    {
                        UiHelper.UpdateActionState("Проверка Url и установка состояний");
                        Thread.Sleep(1000);

                        foreach (var b in packages)
                        {
                            if (string.IsNullOrEmpty(b.Url))
                            {
                                b.State = (int)BulletinState.Error;
                            }
                            else
                            {
                                b.State = (int)BulletinState.OnModeration;
                            }
                            var name = b.ValueFields["Название объявления"];
                            UiHelper.UpdateObjectState($"Bulletin {name}, state = {b.State}");
                        }

                        UiHelper.UpdateActionState("Отправка коллбека");
                        Thread.Sleep(1000);
                    });
                }
            });
        }

        public override void CloneBulletins(IEnumerable<AggregateBulletinPackage> packages)
        {
            UiHelper.UpdateWorkState("Клонирование буллетинов");
            DCT.Execute(d =>
            {
                var createdBulletins = new List<BulletinPackage>();
                {
                    var fieldValueContainer = FieldValueContainerList.Get(Uid);
                    var accessContainer = AccessContainerList.Get(Uid);

                    foreach (var package in packages)
                    {
                        var name = package.Bulletin.ValueFields["Название объявления"];
                        UiHelper.UpdateObjectState($"Bulletin {name}, state = {package.Bulletin.State}");

                        UiHelper.UpdateActionState("Попытка авторизоваться");
                        var accesses = package.Accesses.ToArray();
                        foreach (var access in accesses)
                        {
                            if (accessContainer.TryAuth(access))
                            {
                                UiHelper.UpdateActionState("Ожидание прогрузки страницы");
                                Thread.Sleep(2000);

                                UiHelper.UpdateActionState("Переход на страницу - additem");
                                Thread.Sleep(1000);

                                UiHelper.UpdateActionState("Выбор категорий");
                                ChooseCategories(package.Bulletin.Signature);

                                UiHelper.UpdateActionState("Установка значений");
                                Thread.Sleep(1000);
                                SetValueFields(package.Bulletin, fieldValueContainer);

                                ContinueAddOrEdit(BulletinState.WaitPublication);

                                Thread.Sleep(1000);

                                Publicate(package.Bulletin);
                                //
                                Thread.Sleep(20000);

                                GetUrl(package.Bulletin);

                                var newBulletin = new BulletinPackage
                                {
                                    Access = access,
                                    BulletinId = package.Bulletin.BulletinId,
                                    Url = package.Bulletin.Url,
                                    Title = package.Bulletin.Title,
                                    State = package.Bulletin.State,
                                    Signature = package.Bulletin.Signature,
                                    ValueFields = package.Bulletin.ValueFields
                                };
                                createdBulletins.Add(newBulletin);
                            }
                        }
                    }
                }
                {
                    UiHelper.UpdateActionState("Проверка Url и установка состояний");
                    Thread.Sleep(1000);
                    foreach (var p in packages)
                    {
                        var name = p.Bulletin.ValueFields["Название объявления"];
                        UiHelper.UpdateObjectState($"Bulletin {name}, state = {p.Bulletin.State}");
                    }
                    UiHelper.UpdateActionState("Отправка коллбека");
                    Thread.Sleep(1000);
                }
            });
        }


        public override void CheckModerationState(IEnumerable<BulletinPackage> packages)
        {
            UiHelper.UpdateWorkState("Проверка статуса модерации");
            DCT.Execute(d =>
            {
                {
                    foreach (var b in packages)
                    {
                        var name = b.ValueFields["Название объявления"];
                        UiHelper.UpdateObjectState($"Bulletin {name}, state = {b.State}");

                        var state = CheckBulletinState(b.Url);
                        b.State = (int)state;
                    }
                }
                {
                    UiHelper.UpdateActionState("Проверка Url и установка состояний");
                    Thread.Sleep(1000);
                    foreach (var p in packages)
                    {
                        var name = p.ValueFields["Название объявления"];
                        UiHelper.UpdateObjectState($"Bulletin {name}, state = {p.State}");
                        Thread.Sleep(1000);
                    }
                    UiHelper.UpdateActionState("Отправка коллбека");
                    Thread.Sleep(1000);
                }
            });
        }



        public override void EditBulletins(IEnumerable<BulletinPackage> packages)
        {
            DCT.Execute(d =>
            {
                {
                    var fieldValueContainer = FieldValueContainerList.Get(Uid);
                    var accessContainer = AccessContainerList.Get(Uid);

                    foreach (var bulletin in packages)
                    {
                        var name = bulletin.ValueFields["Название объявления"];
                        UiHelper.UpdateObjectState($"Bulletin {name}, state = {bulletin.State}");

                        UiHelper.UpdateActionState("Попытка авторизоваться");
                        if (accessContainer.TryAuth(bulletin.Access))
                        {
                            UiHelper.UpdateActionState("Ожидание прогрузки страницы");
                            Thread.Sleep(2000);
                            Tools.WebWorker.NavigatePage(Path.Combine(bulletin.Url, "edit"));

                            UiHelper.UpdateActionState("Установка значений");
                            Thread.Sleep(1000);
                            SetValueFields(bulletin, fieldValueContainer);
                            Thread.Sleep(1000);

                            ContinueAddOrEdit(EnumHelper.GetValue<BulletinState>(bulletin.State));
                            Thread.Sleep(1000);

                            Publicate(bulletin);
                        }
                    }

                }
                {
                    foreach (var b in packages)
                    {
                        b.State = (int)BulletinState.OnModeration;
                        var name = b.ValueFields["Название объявления"];
                        UiHelper.UpdateObjectState($"Bulletin {name}, state = {b.State}");
                        Thread.Sleep(1000);
                    }
                    UiHelper.UpdateActionState("Отправка коллбека");
                    Thread.Sleep(1000);
                }
            });
        }

       
        public override void GetBulletinList(IEnumerable<AccessCache> accesses)
        {
            UiHelper.UpdateWorkState("Выгрузка списка буллетинов");

            var bulletins = new List<BulletinPackage>();
            DCT.Execute(d =>
            {
                {
                    foreach (var access in accesses)
                    {
                        UiHelper.UpdateObjectState($"Access {access.Login}");
                        var r = GetBulletinList(access);
                        bulletins.AddRange(r);
                    }
                }
                {
                    UiHelper.UpdateActionState("Отправка коллбека");
                    Thread.Sleep(1000);
                }

            });
        }

        public override void GetBulletinDetails(IEnumerable<BulletinPackage> packages)
        {
            UiHelper.UpdateWorkState("Выгрузка полей для списка буллетинов");

            DCT.Execute(d =>
            {
                {
                    var fieldValueContainer = FieldValueContainerList.Get(Uid);
                    var accessContainer = AccessContainerList.Get(Uid);
                    var accessCollection = packages.Cast<BulletinPackage>().Where(q => q.Access != null).GroupBy(q => q.Access.Login).Select(q => new { Access = q.Key, Collection = q.ToList() }).ToList();
                    foreach (var a in accessCollection)
                    {
                        UiHelper.UpdateObjectState($"Access {a.Access}");
                        
                        var bulletins = a.Collection;
                        foreach (var bulletin in bulletins)
                        {
                            UiHelper.UpdateActionState("Попытка авторизоваться");
                            Thread.Sleep(2000);
                            if (accessContainer.TryAuth(bulletin.Access))
                            {
                                Thread.Sleep(1000);

                                var url = Path.Combine(bulletin.Url, "edit");
                                UiHelper.UpdateActionState($"Переход на страницу {url}");
                                Thread.Sleep(2000);

                                var values = new Dictionary<string, string>();
                                foreach (var pair in bulletin.AccessFields)
                                {
                                    var v = fieldValueContainer.GetFieldValue(bulletin.AccessFields, pair.Key);
                                    values.Add(pair.Key, v);
                                }
                                bulletin.ValueFields = values;
                                bulletin.State = (int)CheckBulletinState(bulletin.Url);
                            }
                        }
                    }
                }
                {
                    UiHelper.UpdateActionState("Отправка коллбека");
                    Thread.Sleep(1000);
                }
            });
        }


        public override void RepublicateBulletins(IEnumerable<BulletinPackage> packages)
        {
        }


        void ChooseCategories(GroupSignature signature)
        {
            DCT.Execute(d =>
            {
                if (!string.IsNullOrEmpty(signature.Category1))
                {
                    Thread.Sleep(1000);

                }
                //2
                if (!string.IsNullOrEmpty(signature.Category2))
                {
                    Thread.Sleep(1000);
                }
                //3
                if (!string.IsNullOrEmpty(signature.Category3))
                {
                   
                    Thread.Sleep(1000);
                }
                //4
                if (!string.IsNullOrEmpty(signature.Category4))
                {
                   
                    Thread.Sleep(1000);
                }
                //5
                if (!string.IsNullOrEmpty(signature.Category5))
                {
                   
                    Thread.Sleep(1000);
                }
            });
        }

        void SetValueFields(BulletinPackage bulletin, FieldValueContainerBase fieldContainer)
        {
            DCT.Execute(d =>
            {
                foreach (var pair in bulletin.ValueFields)
                {
                    if (string.IsNullOrEmpty(pair.Value)) continue;
                    var template = bulletin.AccessFields.FirstOrDefault(q => q.Key == pair.Key);
                    fieldContainer.SetFieldValue(bulletin.AccessFields, template.Key, pair.Value);
                }
            });

        }


        void ContinueAddOrEdit(BulletinState state)
        {
            DCT.Execute(d =>
            {
                if (state == BulletinState.Edited)
                {
                    UiHelper.UpdateActionState("Выбор \"Продолжить без пакет\"");

                }
                else if (state == BulletinState.WaitPublication || state == BulletinState.WaitRepublication)
                {
                    UiHelper.UpdateActionState("Выбор \"Продолжить с пакетом «Обычная продажа»\"");
                }
                else
                {

                    UiHelper.UpdateActionState("Выбор \"Продолжить\"");
                }
            });
        }

        void Publicate(BulletinPackage bulletin)
        {
            DCT.Execute(d =>
            {
                if (bulletin.State == (int)BulletinState.WaitPublication)
                {
                    UiHelper.UpdateActionState("Переход на страницу - additem/confirm");
                }
                else if (bulletin.State == (int)BulletinState.Edited)
                {
                    UiHelper.UpdateActionState($"Переход на страницу - {Path.Combine(bulletin.Url, "edit", "confirm")}");
                }
                Thread.Sleep(1000);

                UiHelper.UpdateActionState("Снятие премиум-галочек");
                Thread.Sleep(1000);


                UiHelper.UpdateActionState("Подтверждение публикации");
                Thread.Sleep(1000);
            });
        }

        void GetUrl(BulletinPackage bulletin)
        {
            DCT.Execute(d =>
            {
                if(!string.IsNullOrEmpty(bulletin.Url))
                {
                    UiHelper.UpdateActionState("URL успешно считан");
                }
                else
                {
                    UiHelper.UpdateActionState("URL is NULL");
                }
            });
        }


        BulletinState CheckBulletinState(string url)
        {
            var result = BulletinState.Error;
            DCT.Execute(d =>
            {
                UiHelper.UpdateActionState("Проверка состояния");
                Thread.Sleep(1000);

                var hasFounded = true;
                if(hasFounded)
                {
                    result = BulletinState.OnModeration;
                    UiHelper.UpdateActionState($"Новое состояние {(int)result}");
                    Thread.Sleep(1000);
                }
            });

            return result;
        }


        IEnumerable<BulletinPackage> GetBulletinList(AccessCache access)
        {
            var result = Enumerable.Empty<BulletinPackage>();
            DCT.Execute(d =>
            {
                var tabStates = new List<TabState>();
                var bulletins = new List<BulletinPackage>();

                var fieldValueContainer = FieldValueContainerList.Get(Uid);
                var accessContainer = AccessContainerList.Get(Uid);

                UiHelper.UpdateActionState("Попытка авторизоваться");
                if (accessContainer.TryAuth(access))
                {
                    Thread.Sleep(2000);

                    UiHelper.UpdateActionState("Ожидание прогрузки страницы");
                    Thread.Sleep(2000);

                    UiHelper.UpdateActionState("Переход на страницу профиля");
                    Thread.Sleep(2000);

                    UiHelper.UpdateActionState("Считывание списка буллетинов");
                    Thread.Sleep(2000);

                }
                result = bulletins;
            });
            return result;
        }

        public override void AddBulletins2(IEnumerable<TaskCache_old> packages)
        {
            throw new NotImplementedException();
        }

        public override void GetBulletinList2(IEnumerable<TaskCache_old> tasks)
        {
            throw new NotImplementedException();
        }

        public override void GetBulletinDetails2(IEnumerable<TaskCache_old> tasks)
        {
            throw new NotImplementedException();
        }
    }
}
