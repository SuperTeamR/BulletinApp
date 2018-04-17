//namespace BulletinHub.Migrations
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class BulletinDb3 : DbMigration
//    {
//        public override void Up()
//        {
//            AddColumn("dbo.Tasks", "TargetDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
//            DropColumn("dbo.Tasks", "TargetTime");
//        }
        
//        public override void Down()
//        {
//            AddColumn("dbo.Tasks", "TargetTime", c => c.DateTime(nullable: false));
//            DropColumn("dbo.Tasks", "TargetDate");
//        }
//    }
//}
