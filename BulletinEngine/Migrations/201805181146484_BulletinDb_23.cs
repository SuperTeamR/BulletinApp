//namespace BulletinEngine.Contexts
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class BulletinDb_23 : DbMigration
//    {
//        public override void Up()
//        {
//            CreateTable(
//                "dbo.Messages",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        AccessId = c.Guid(nullable: false),
//                        Text = c.String(),
//                        IsAnswered = c.String(),
//                        PublicationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
//                        Url = c.String(),
//                        LastChangeId = c.Guid(),
//                        State = c.Int(nullable: false),
//                        HasRemoved = c.Boolean(nullable: false),
//                        CreateDate = c.DateTime(nullable: false),
//                    })
//                .PrimaryKey(t => t.Id);
            
//            AddColumn("dbo.Accesses", "LastMessage", c => c.DateTime(precision: 7, storeType: "datetime2"));
//        }
        
//        public override void Down()
//        {
//            DropColumn("dbo.Accesses", "LastMessage");
//            DropTable("dbo.Messages");
//        }
//    }
//}
