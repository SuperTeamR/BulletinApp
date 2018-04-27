using BulletinEngine.Core;
using BulletinHub.Entity.Converters;
using FessooFramework.Objects.Data;
using FessooFramework.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BulletinHub.Entity.Data
{
    public class Task : EntityObjectALM<Task, TaskState>
    {
        #region Entity properties
        public Guid UserId { get; set; }
        public Guid? BulletinId { get; set; }
        public Guid? InstanceId { get; set; }
        public Guid? AccessId { get; set; }
        public string TargetType { get; set; }
        public DateTime? TargetDate { get; set; }
        public int Command { get; set; }
        [NotMapped]
        public TaskCommand CommandEnum
        {
            get { return EnumHelper.GetValue<TaskCommand>(Command); }
            set { Command = (int)value; }
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
            arg1.TargetType = arg2.TargetType;
            arg1.TargetDate = arg2.TargetDate;
            arg1.Command = arg2.Command;
            arg1.BulletinId = arg2.BulletinId;
            return arg1;
        }
        #endregion

        #region Creators
        protected override IEnumerable<EntityObjectALMCreator<Task>> CreatorsService => new[]
        {
             EntityObjectALMCreator<Task>.New(TaskConverter.Convert, TaskConverter.Convert, new Version(1,0,0,0))
        };
        #endregion
        
        #region DataService -- Methods

        public override IEnumerable<EntityObject> CustomCollectionLoad(string code, string sessionUID = "", string hashUID = "", IEnumerable<EntityObject> obj = null, IEnumerable<Guid> id = null)
        {
            var result = Enumerable.Empty<EntityObject>();
            BCT.Execute(d =>
            {
                switch (code)
                {
                    case "Save":
                        result = obj;
                        d.SaveChanges();
                        break;
                    case "Load":
                        result = new[] {
                            d.TempDB.Tasks.Where(q => q.State == (int)TaskState.Created && (q.TargetDate == null || q.TargetDate.Value < DateTime.Now)).FirstOrDefault() };
                        break;
                }
            });
            return result;
        }
        public override IEnumerable<EntityObject> _CollectionObjectLoad()
        {
            var result = Enumerable.Empty<EntityObject>();
            BCT.Execute(c =>
            {
                var id = c._SessionInfo.HashUID;
                var id2 = c._SessionInfo.SessionUID;
                if (id == "Engine")
                {
                    result = c.TempDB.Tasks.Where(q => q.State == (int)TaskState.Created && (q.TargetDate == null || q.TargetDate.Value < DateTime.Now)).ToArray();
                }
                else
                    result = base._CollectionObjectLoad();
            });
            return result;
        }
        public override IEnumerable<TDataModel> _CacheSave<TDataModel>(IEnumerable<TDataModel> objs)
        {
            var result = Enumerable.Empty<TDataModel>();
            BCT.Execute(d =>
            {
                d.SaveChanges();
                result = objs;
            });
            return result;
        }
        #endregion
    }
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
        AccessCheck =1,
        InstancePublication = 2,
        Creation = 0,
        Checking = 1,
        Editing = 2,
        Cloning = 3,
       
    }
}