//namespace BulletinHub.Migrations
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class BulletinDb2 : DbMigration
//    {
//        public override void Up()
//        {
//            CreateTable(
//                "dbo.Tasks",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        TargetId = c.Guid(nullable: false),
//                        TargetType = c.String(),
//                        TargetTime = c.DateTime(nullable: false),
//                        LastChangeId = c.Guid(),
//                        State = c.Int(nullable: false),
//                        HasRemoved = c.Boolean(nullable: false),
//                        CreateDate = c.DateTime(nullable: false),
//                    })
//                .PrimaryKey(t => t.Id);
            
//        }
        
//        public override void Down()
//        {
//            DropTable("dbo.Tasks");
//        }
//    }
//}
