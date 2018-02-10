using BulletinExample.Entity.Data;
using Data;
using FessooFramework.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample.Entity.Context
{
    #region Configuration
    //Add-migration DefaultDB_1 -ConfigurationTypeName BulletinExample.Entity.Context.ConfigurationBulletinDb
    // Update-database
    internal sealed class ConfigurationBulletinDb : DbMigrationsConfiguration<BulletinDb>
    {
        public ConfigurationBulletinDb() { AutomaticMigrationsEnabled = true; }
        protected override void Seed(BulletinDb context) { }
    }
    #endregion
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
     
        public BulletinDb()
        {
            base.Configuration.ProxyCreationEnabled = false;
            base.Configuration.LazyLoadingEnabled = true;
            base.Database.Connection.ConnectionString = EntityHelper.CreateLocalSQL("BulletinDb_2");
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
