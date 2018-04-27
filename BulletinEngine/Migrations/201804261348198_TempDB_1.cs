//namespace BulletinHub.Contexts
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class TempDB_1 : DbMigration
//    {
//        public override void Up()
//        {
//            AddColumn("dbo.Tasks", "InstanceId", c => c.Guid());
//            DropColumn("dbo.Tasks", "BulletinId");
//        }
        
//        public override void Down()
//        {
//            AddColumn("dbo.Tasks", "BulletinId", c => c.Guid());
//            DropColumn("dbo.Tasks", "InstanceId");
//        }
//    }
//}
