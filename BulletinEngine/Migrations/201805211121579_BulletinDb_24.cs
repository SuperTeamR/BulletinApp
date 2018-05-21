//namespace BulletinEngine.Contexts
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class BulletinDb_24 : DbMigration
//    {
//        public override void Up()
//        {
//            AddColumn("dbo.UserStatistics", "TotalProducts", c => c.Int(nullable: false));
//            AddColumn("dbo.UserStatistics", "TotalInstances", c => c.Int(nullable: false));
//        }
        
//        public override void Down()
//        {
//            DropColumn("dbo.UserStatistics", "TotalInstances");
//            DropColumn("dbo.UserStatistics", "TotalProducts");
//        }
//    }
//}
