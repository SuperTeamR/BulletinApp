using BulletinEngine.Entity.Data;
using BulletinHub.Entity.Data;
using BulletinHub.Models;
using FessooFramework.Tools.Helpers;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace BulletinEngine.Contexts
{
    //Add-migration BulletinDb_1 -ConfigurationTypeName BulletinEngine.Contexts.ConfigurationBulletinDb
    // Update-database -ConfigurationTypeName BulletinEngine.Contexts.ConfigurationBulletinDb
    internal sealed class ConfigurationBulletinDb : DbMigrationsConfiguration<BulletinEngine.Contexts.BulletinDb>
    {
        public ConfigurationBulletinDb()
        {
            AutomaticMigrationsEnabled = false;
        }
        protected override void Seed(BulletinEngine.Contexts.BulletinDb context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }

    public class BulletinDb : DbContext
    {
        public DbSet<Application> Applications { get; set; }
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
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<UserStatistics> UserStatistics { get; set; }

        public BulletinDb()
        {
            base.Configuration.ProxyCreationEnabled = false;
            base.Configuration.LazyLoadingEnabled = true;
            //base.Database.Connection.ConnectionString =
            //    EntityHelper.CreateRemoteSQL("BulletinDb",
            //   "192.168.26.116",
            //   "ExtUser",
            //   "123QWEasd");
            base.Database.Connection.ConnectionString =
            EntityHelper.CreateRemoteSQL("BulletinDb",
           "176.111.73.51",
           "AMK2",
           "OnlineHelp59");
            //Database.SetInitializer(new CreateDatabaseIfNotExists<BulletinDb>());
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<BulletinDb>());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BulletinDb, ConfigurationBulletinDb>());
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BulletinHub.Entity.Data.Task>().Property(x => x.TargetDate).HasColumnType("datetime2");
            modelBuilder.Entity<UserSettings>().Property(x => x.LastTimeGeneration).HasColumnType("datetime2");
            modelBuilder.Entity<UserSettings>().Property(x => x.NextTaskGeneration).HasColumnType("datetime2");
        }
        protected override void Dispose(bool disposing)
        {
            Configuration.LazyLoadingEnabled = false;
            base.Dispose(disposing);
        }
    }
}