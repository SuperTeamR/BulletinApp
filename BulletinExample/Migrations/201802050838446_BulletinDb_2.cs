//namespace BulletinExample.Migrations
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class BulletinDb_2 : DbMigration
//    {
//        public override void Up()
//        {
//            CreateTable(
//                "dbo.BulletinFields",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        BulletinInstanceId = c.Guid(nullable: false),
//                        FieldId = c.Guid(nullable: false),
//                        Value = c.String(),
//                        LastChangeId = c.Guid(),
//                        HasRemoved = c.Boolean(nullable: false),
//                        CreateDate = c.DateTime(nullable: false),
//                    })
//                .PrimaryKey(t => t.Id);
            
//        }
        
//        public override void Down()
//        {
//            DropTable("dbo.BulletinFields");
//        }
//    }
//}
