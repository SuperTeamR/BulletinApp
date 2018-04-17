//namespace BulletinHub.Migrations
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class BulletinDb4 : DbMigration
//    {
//        public override void Up()
//        {
//            AddColumn("dbo.Tasks", "BulletinId", c => c.Guid());
//            AddColumn("dbo.Tasks", "AccessId", c => c.Guid());
//            DropColumn("dbo.Tasks", "TargetId");
//        }
        
//        public override void Down()
//        {
//            AddColumn("dbo.Tasks", "TargetId", c => c.Guid(nullable: false));
//            DropColumn("dbo.Tasks", "AccessId");
//            DropColumn("dbo.Tasks", "BulletinId");
//        }
//    }
//}
