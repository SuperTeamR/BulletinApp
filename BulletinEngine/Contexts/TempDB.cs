using BulletinHub.Models;
using FessooFramework.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinHub.Contexts
{
    #region Configuration
    //Add-migration TempDB_1 -ConfigurationTypeName BulletinHub.Contexts.ConfigurationTempDB
    // Update-database -ConfigurationTypeName BulletinHub.Contexts.ConfigurationTempDB
    internal sealed class ConfigurationTempDB : DbMigrationsConfiguration<TempDB>
    {
        public ConfigurationTempDB() { AutomaticMigrationsEnabled = true; }
        protected override void Seed(TempDB context)
        {

        }
    }
    #endregion
    #region Context
    public class TempDB : DbContext
    {
        public DbSet<BulletinHub.Entity.Data.Task> Tasks { get; set; }
        public DbSet<BulletinTemplate> BulletinTemplate { get; set; }
        public DbSet<Call> Calls { get; set; }

        public TempDB()
        {
            base.Configuration.ProxyCreationEnabled = false;
            base.Configuration.LazyLoadingEnabled = true;
            this.Database.Connection.ConnectionString = EntityHelper.CreateRemoteSQL("BulletinTempDB", "176.111.73.51", "AMK2", "OnlineHelp59");
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<TempDB, ConfigurationTempDB>());
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Call>().Property(x => x.CallDate).HasColumnType("datetime2");
        }
        protected override void Dispose(bool disposing)
        {
            Configuration.LazyLoadingEnabled = false;
            base.Dispose(disposing);
        }
    }
    #endregion
}
