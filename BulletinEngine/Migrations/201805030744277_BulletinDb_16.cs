//namespace BulletinEngine.Contexts
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class BulletinDb_16 : DbMigration
//    {
//        public override void Up()
//        {
//            AddColumn("dbo.Accesses", "Phone", c => c.String());
//            AddColumn("dbo.Accesses", "HasBlocked", c => c.Boolean(nullable: false));
//        }
        
//        public override void Down()
//        {
//            DropColumn("dbo.Accesses", "HasBlocked");
//            DropColumn("dbo.Accesses", "Phone");
//        }
//    }
//}
