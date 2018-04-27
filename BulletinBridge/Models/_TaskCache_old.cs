using FessooFramework.Objects.Data;
using System;
using System.Runtime.Serialization;

namespace BulletinBridge.Data
{
    [DataContract]
    public class TaskCache_old : CacheObject
    {
        [DataMember]
        public Guid? BulletinId { get; set; }
        [DataMember]
        public Guid? AccessId { get; set; }
        [DataMember]
        public int Command { get; set; }
        [DataMember]
        public string TargetType { get; set; }
        [DataMember]
        public DateTime? TargetTime { get; set; }
        [DataMember]
        public BulletinPackage BulletinPackage { get; set; }
        [DataMember]
        public AccessPackage AccessPackage { get; set; }
    }


    public enum TaskCacheCommand
    {
        Creation = 0,
        Checking = 1,
        CheckingDetails = 2,
        Editing = 2,
        Cloning = 3,
    }
    public enum TaskCacheState
    {
        Created = 0,
        Doing = 1,
        Completed = 2,
        Error = 99,
    }

}