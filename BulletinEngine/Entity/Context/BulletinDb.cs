﻿using BulletinEngine.Entity.Data;
using FessooFramework.Tools.Helpers;
using System.Configuration;
using System.Data.Entity;

namespace BulletinEngine.Entity.Context
{
    public class BulletinDb : DbContext
    {
        public DbSet<Application> Applications { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Access> Accesses { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Bulletin> Bulletins { get; set; }
        public DbSet<BulletinInstance> BulletinInstances { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<FieldTemplate> FieldTemplates { get; set; }
        public DbSet<GroupedField> GroupedFields { get; set; }
        public DbSet<SelectOption> SelectOptions { get; set; }
        public DbSet<CategoryTemplate> CategoryTemplates { get; set; }
        public DbSet<GroupedCategory> GroupedCategories { get; set; }
        public DbSet<BulletinField> BulletinFields { get; set; }

        public BulletinDb()
        {
            base.Configuration.ProxyCreationEnabled = false;
            base.Configuration.LazyLoadingEnabled = true;
            //base.Database.Connection.ConnectionString =
            //    EntityHelper.CreateRemoteSQL("BulletinDb",
            //   "192.168.26.116",
            //   "ExtUser",
            //   "123QWEasd");
            Database.SetInitializer(new CreateDatabaseIfNotExists<BulletinDb>());
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<BulletinDb>());
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<BulletinDb, BulletinHub.Migrations.Configuration>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }

        protected override void Dispose(bool disposing)
        {
            Configuration.LazyLoadingEnabled = false;
            base.Dispose(disposing);
        }
    }
}
