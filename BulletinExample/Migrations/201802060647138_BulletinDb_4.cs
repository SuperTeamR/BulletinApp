//namespace BulletinExample.Migrations
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class BulletinDb_4 : DbMigration
//    {
//        public override void Up()
//        {
//            AddColumn("dbo.GroupedFields", "HtmlId", c => c.String());
//            DropColumn("dbo.FieldTemplates", "HtmlId");
//        }
        
//        public override void Down()
//        {
//            AddColumn("dbo.FieldTemplates", "HtmlId", c => c.String());
//            DropColumn("dbo.GroupedFields", "HtmlId");
//        }
//    }
//}
