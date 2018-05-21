//namespace BulletinHub.Contexts
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class TempDb_7 : DbMigration
//    {
//        public override void Up()
//        {
//            CreateTable(
//                "dbo.Emails",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        Title = c.String(),
//                        Body = c.String(),
//                        Destination = c.String(),
//                        LastChangeId = c.Guid(),
//                        State = c.Int(nullable: false),
//                        HasRemoved = c.Boolean(nullable: false),
//                        CreateDate = c.DateTime(nullable: false),
//                    })
//                .PrimaryKey(t => t.Id);
            
//        }
        
//        public override void Down()
//        {
//            DropTable("dbo.Emails");
//        }
//    }
//}
