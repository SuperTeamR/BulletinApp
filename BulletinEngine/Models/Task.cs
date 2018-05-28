using BulletinBridge.Data;
using BulletinBridge.Models;
using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using BulletinEngine.Helpers;
using BulletinHub.Tools;
using FessooFramework.Objects.Data;
using FessooFramework.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BulletinHub.Entity.Data
{
    public enum TaskState
    {
        Created = 0,
        Enabled = 1,
        Disabled = 2,
        Completed = 3,
        Error = 99,
    }
    public enum TaskCommand
    {
        None = 0,
        AccessCheck = 1,
        InstancePublication = 2,
        BulletinTemplateCollector = 3,
        ActivateAccess = 4,
        Registration = 5,
        AccessStatistics = 6,
        InstanceStatistics = 7,
        ActivateInstance = 8,
        CollectMessages = 9,

        //Obsolute
        Creation = 55,
        Checking = 66,
        Editing = 77,
        Cloning = 88,
    }
    public class Task : EntityObjectALM<Task, TaskState>
    {
        #region Entity properties
        public Guid UserId { get; set; }
        public Guid BoardId { get; set; }
        public Guid? BulletinId { get; set; }
        public Guid? InstanceId { get; set; }
        public Guid? AccessId { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string ErrorDescription { get; set; }
        public int Command { get; set; }
        [NotMapped]
        public TaskCommand CommandEnum
        {
            get { return EnumHelper.GetValue<TaskCommand>(Command); }
            set { Command = (int)value; }
        }
        [Obsolete]
        public string TargetType { get; set; }
        
        public string LastStep { get; set; }

        #endregion

        #region Another methods
        public void SetComplete()
        {
            CompletedDate = DateTime.Now;
            StateEnum = TaskState.Completed;
        }
        public void SetError(string description)
        {
            CompletedDate = DateTime.Now;
            StateEnum = TaskState.Error;
        }
        #endregion

        #region ALM
        protected override IEnumerable<EntityObjectALMConfiguration<Task, TaskState>> Configurations => new[]
       {
            new EntityObjectALMConfiguration<Task, TaskState>(TaskState.Created, TaskState.Created, SetValueDefault),
        };
        protected override int GetStateValue(TaskState state)
        {
            return (int)state;
        }
        protected override Task SetValueDefault(Task arg1, Task arg2)
        {
            arg1.UserId = arg2.UserId;
            arg1.InstanceId = arg2.InstanceId;
            arg1.AccessId = arg2.AccessId;
            arg1.TargetDate = arg2.TargetDate;
            arg1.Command = arg2.Command;
            arg1.BulletinId = arg2.BulletinId;
            arg1.CompletedDate = arg2.CompletedDate;
            arg1.ErrorDescription = arg2.ErrorDescription;
            arg1.BoardId = arg2.BoardId;
            arg1.LastStep = arg2.LastStep;
            return arg1;
        }
        #endregion

        #region Creators
        protected override IEnumerable<EntityObjectALMCreator<Task>> CreatorsService => new[]
        {
             EntityObjectALMCreator<Task>.New(ToCache, ToEntity, new Version(1,0,0,0)),
             EntityObjectALMCreator<Task>.New<TaskCache>(ToCache2, ToEntity2, new Version(1,0,0,0)),
             EntityObjectALMCreator<Task>.New<TaskAccessCheckCache>(ToCache3, ToEntityDefault, new Version(1,0,0,0)),
             EntityObjectALMCreator<Task>.New<TaskInstancePublicationCache>(ToCache4, ToEntityDefault, new Version(1,0,0,0)),
             EntityObjectALMCreator<Task>.New<TaskBulletinTemplateCollectorCache>(ToCache5, ToEntityDefault, new Version(1,0,0,0)),
             EntityObjectALMCreator<Task>.New<TaskRegistrationCache>(ToCache6, ToEntityDefault, new Version(1,0,0,0)),
             EntityObjectALMCreator<Task>.New<TaskInstanceActivationCache>(ToCache7, ToEntityDefault, new Version(1,0,0,0)),
             EntityObjectALMCreator<Task>.New<TaskMessageCollectorCache>(ToCache8, ToEntityDefault, new Version(1,0,0,0))
        };
        #region Old
        public static TaskCache_old ToCache(Data.Task obj)
        {
            TaskCache_old result = null;
            BCT.Execute(d =>
            {
                var bulletinPackage = new BulletinPackage();
                var accessPackage = new AccessCache();

                if (obj.TargetType == typeof(BulletinInstance).ToString())
                {
                    var dbInstance = d.BulletinDb.BulletinInstances.FirstOrDefault(q => q.Id == obj.InstanceId);
                    var dbAccess = d.BulletinDb.Accesses.FirstOrDefault(q => q.Id == obj.AccessId);
                    var groupSignature = GroupHelper.GetGroupSignature2(dbInstance.BulletinId);
                    var valueFields = ValueFieldHelper.GetValueFields2(dbInstance.BulletinId);
                    var accessFields = AccessFieldHelper.GetAccessFields2(dbInstance.BulletinId);

                    bulletinPackage.Signature = groupSignature;
                    bulletinPackage.ValueFields = valueFields;
                    bulletinPackage.AccessFields = accessFields;
                    bulletinPackage.Access = Access.ToCache(dbAccess);
                    bulletinPackage.State = dbInstance.State;
                    bulletinPackage.Url = dbInstance.Url;
                }
                else if (obj.TargetType == typeof(Access).ToString())
                {

                }
                result = new TaskCache_old
                {
                    BulletinId = obj.InstanceId,
                    AccessId = obj.AccessId,
                    TargetType = obj.TargetType,
                    TargetTime = obj.TargetDate,
                    Command = obj.Command,
                    BulletinPackage = bulletinPackage,
                    AccessPackage = accessPackage,
                };
            });
            return result;
        }
        public static Data.Task ToEntity(TaskCache_old obj, Data.Task entity)
        {
            var result = default(Data.Task);
            BCT.Execute(d =>
            {
                if (obj.BulletinPackage != null)
                {
                    var dbInstance = d.BulletinDb.BulletinInstances.FirstOrDefault(q => q.Id == obj.BulletinId);
                    dbInstance.State = obj.BulletinPackage.State;
                    dbInstance.Url = obj.BulletinPackage.Url;
                    d.SaveChanges();
                }

                entity.InstanceId = obj.BulletinId;
                entity.AccessId = obj.AccessId;
                entity.TargetDate = obj.TargetTime;
                entity.State = obj.State;

                result = entity;
            });
            return entity;
        }
        #endregion
        #region Default
        private Task ToEntityDefault(object arg1, Task arg2)
        {
            return arg2;
        }
        #endregion
        #region Task cache - main
        private Task ToEntity2(TaskCache arg1, Task arg2)
        {
            arg2.ErrorDescription = arg1.Error;
            arg2.LastStep = arg1.LastStep;
            return arg2;
        }

        private TaskCache ToCache2(Task arg2)
        {
            var arg1 = new TaskCache();
            var board = BCT.Context.BulletinDb.Boards.Find(arg2.BoardId);
            if (board != null)
                arg1.Board = board.Name;
            arg1.Command = arg2.CommandEnum.ToString();
            arg1.StateDesc = arg2.StateEnum.ToString();
            arg1.TargetDate = arg2.TargetDate;
            return arg1;
        }
        #endregion
        #region Task AccessCheck cache
        private TaskAccessCheckCache ToCache3(Task arg2)
        {
            var arg1 = new TaskAccessCheckCache();
            var access = BCT.Context.BulletinDb.Accesses.Find(arg2.AccessId);
            if (access != null)
            {
                arg1.AccessId = access.Id;
                arg1.Login = access.Login;
                arg1.Password = access.Password;
            }
            return arg1;
        }
        #endregion
        #region Task InstancePublication cache
        private TaskInstancePublicationCache ToCache4(Task arg2)
        {
            var arg1 = new TaskInstancePublicationCache();
            var access = BCT.Context.BulletinDb.Accesses.Find(arg2.AccessId);
            if (access != null)
            {
                arg1.Login = access.Login;
                arg1.Password = access.Password;
            }
            else
            {
                arg2.ErrorDescription = "Create task model TaskInstancePublicationCache crash. Access not found";
                BackTaskHelper.Error(new[] { arg2 });
                return null;
            }
            var groupSign = GroupHelper.GetGroupSignature2(arg2.BulletinId.Value);
            if (groupSign != null)
            {
                arg1.Category1 = groupSign.Category1 == null ? "" : groupSign.Category1;
                arg1.Category2 = groupSign.Category2 == null ? "" : groupSign.Category2;
                arg1.Category3 = groupSign.Category3 == null ? "" : groupSign.Category3;
                arg1.Category4 = groupSign.Category4 == null ? "" : groupSign.Category4;
                arg1.Category5 = groupSign.Category5 == null ? "" : groupSign.Category5;
            }
            else
            {
                arg2.ErrorDescription = "Create task model TaskInstancePublicationCache crash. Groups not found";
                BackTaskHelper.Error(new[] { arg2 });
                return null;
            }
            var bulletin = BCT.Context.BulletinDb.Bulletins.Find(arg2.BulletinId);
            if (bulletin != null)
            {
                arg1.Title = bulletin.Title;
                arg1.Description = bulletin.Description;
                arg1.Price = bulletin.Price;
                arg1.Images = bulletin.Images.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).ToArray();
            }
            else
            {
                arg2.ErrorDescription = "Create task model TaskInstancePublicationCache crash. Bulletin not found";
                BackTaskHelper.Error(new[] { arg2 });
                return null;
            }
            arg1.InstanceId = arg2.InstanceId.Value;
            return arg1;
        }
        private Task ToEntity4(TaskAccessCheckCache arg1, Task arg2)
        {
            return arg2;
        }
        #endregion
        #region Task BulletinTemplateCollector Cache
        private TaskBulletinTemplateCollectorCache ToCache5(Task arg2)
        {
            var arg1 = new TaskBulletinTemplateCollectorCache();
            arg1.BulletinId = arg2.BulletinId.Value;
            var bulletin = BCT.Context.BulletinDb.Bulletins.Find(arg1.BulletinId);
            if (bulletin != null)
                arg1.Queries = new string[] { bulletin.Title };
            return arg1;
        }
        #endregion
        #region Task Registration Cache
        private TaskRegistrationCache ToCache6(Task arg2)
        {
            var arg1 = new TaskRegistrationCache();
            arg1.AccessId = arg2.AccessId.Value;
            var access = BCT.Context.BulletinDb.Accesses.Find(arg1.AccessId);
            if (access != null)
            {
                access.Login = access.Login;
                access.Password = access.Password;
            }
            return arg1;
        }
        #endregion
        #region Task InstanceActivationCache
        private TaskInstanceActivationCache ToCache7(Task arg2)
        {
            var arg1 = new TaskInstanceActivationCache();
            var access = BCT.Context.BulletinDb.Accesses.Find(arg2.AccessId.Value);
            if (access != null)
            {
                arg1.Login = access.Login;
                arg1.Password = access.Password;
            }
            var instance = BCT.Context.BulletinDb.BulletinInstances.Find(arg2.InstanceId.Value);
            if (instance != null)
            {
                arg1.Url = instance.Url;
                arg1.InstanceId = instance.Id;
            }
                
            return arg1;
        }
        #endregion
        #region Task MessageCollectorCache
        private TaskMessageCollectorCache ToCache8(Task arg2)
        {
            var arg1 = new TaskMessageCollectorCache();
            var access = BCT.Context.BulletinDb.Accesses.Find(arg2.AccessId);
            if (access != null)
            {
                arg1.AccessId = access.Id;
                arg1.Login = access.Login;
                arg1.Password = access.Password;
                arg1.LastMessage = access.LastMessage;
            }
            return arg1;
        }
        #endregion
        #endregion
        #region DataService -- Methods
        public override IEnumerable<EntityObject> CustomCollectionLoad(string code, string sessionUID = "", string hashUID = "", IEnumerable<EntityObject> obj = null, IEnumerable<Guid> id = null)
        {
            var result = Enumerable.Empty<EntityObject>();
            BCT.Execute(d =>
            {
                //Пока не заморачивался - передаётся базовый объект и требуется привести к типу
                var entities = Enumerable.Empty<Task>();
                if (obj.Any())
                    entities = obj.Select(q => (Task)q).ToArray();
                switch (code)
                {
                    case "All":
                        result = BackTaskHelper.All();
                        break;
                    case "Next":
                        result = new Task[] { BackTaskHelper.Next() };
                        break;
                    case "Complete":
                        BackTaskHelper.Complite(entities);
                        break;
                    case "Erorr":
                        BackTaskHelper.Error(entities);
                        break;
                    case "GetTask":
                        result = entities;
                        break;
                    default:
                        break;
                }
            });
            return result;
        }
        #endregion


    }

}