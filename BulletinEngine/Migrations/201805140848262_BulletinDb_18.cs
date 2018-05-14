//namespace BulletinEngine.Contexts
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class BulletinDb_18 : DbMigration
//    {
//        public override void Up()
//        {
//            CreateTable(
//                "dbo.UserStatistics",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        UserId = c.Guid(nullable: false),
//                        TotalViews = c.Int(nullable: false),
//                        TotalCalls = c.Int(nullable: false),
//                        TotalMessages = c.Int(nullable: false),
//                        LastChangeId = c.Guid(),
//                        State = c.Int(nullable: false),
//                        HasRemoved = c.Boolean(nullable: false),
//                        CreateDate = c.DateTime(nullable: false),
//                    })
//                .PrimaryKey(t => t.Id);
            
//            AddColumn("dbo.Accesses", "Views", c => c.Int(nullable: false));
//            AddColumn("dbo.Accesses", "Calls", c => c.Int(nullable: false));
//            AddColumn("dbo.Accesses", "Messages", c => c.Int(nullable: false));
//        }
        
//        public override void Down()
//        {
//            DropColumn("dbo.Accesses", "Messages");
//            DropColumn("dbo.Accesses", "Calls");
//            DropColumn("dbo.Accesses", "Views");
//            DropTable("dbo.UserStatistics");
//        }
//    }
//}
