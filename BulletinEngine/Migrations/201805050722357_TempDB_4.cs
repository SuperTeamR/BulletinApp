//namespace BulletinHub.Contexts
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class TempDB_4 : DbMigration
//    {
//        public override void Up()
//        {
//            AddColumn("dbo.BulletinTemplates", "IsIndividualSeller", c => c.Boolean(nullable: false));
//            AddColumn("dbo.BulletinTemplates", "IsHandled", c => c.Boolean(nullable: false));
//            AlterColumn("dbo.BulletinTemplates", "Count", c => c.Int(nullable: false));
//        }
        
//        public override void Down()
//        {
//            AlterColumn("dbo.BulletinTemplates", "Count", c => c.String());
//            DropColumn("dbo.BulletinTemplates", "IsHandled");
//            DropColumn("dbo.BulletinTemplates", "IsIndividualSeller");
//        }
//    }
//}
