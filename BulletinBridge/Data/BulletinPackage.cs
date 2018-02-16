﻿using FessooFramework.Objects.Data;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BulletinBridge.Data
{
    public class BulletinPackage : CacheObject
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public int State { get; set; }
        public string Views { get; set; }
        public GroupSignature Signature { get; set; }
        public AccessPackage Access { get; set; }
        public Dictionary<string, string> ValueFields { get; set; }
        public Dictionary<string, FieldPackage> AccessFields { get; set; }

        public BulletinPackage()
        {

        }

    }

    public enum BulletinState
    {
        Created = 0,
        WaitPublication = 1,
        OnModeration = 2,
        Rejected = 3,
        Blocked = 4,
        Publicated = 5,
        Edited = 6,
        Removed = 7,
        Error = 99,
    }
}