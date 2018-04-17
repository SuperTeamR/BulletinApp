//namespace BulletinHub.Migrations
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class BulletinDb6 : DbMigration
//    {
//        public override void Up()
//        {
//            AddColumn("dbo.Bulletins", "GroupId", c => c.Guid());
//            AddColumn("dbo.Bulletins", "Title", c => c.String());
//            AddColumn("dbo.Bulletins", "Description", c => c.String());
//            AddColumn("dbo.Bulletins", "Price", c => c.String());
//            AddColumn("dbo.Bulletins", "Images", c => c.String());
//        }
        
//        public override void Down()
//        {
//            DropColumn("dbo.Bulletins", "Images");
//            DropColumn("dbo.Bulletins", "Price");
//            DropColumn("dbo.Bulletins", "Description");
//            DropColumn("dbo.Bulletins", "Title");
//            DropColumn("dbo.Bulletins", "GroupId");
//        }
//    }
//}
