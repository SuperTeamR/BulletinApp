//namespace BulletinExample.Migrations
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class BulletinDb_5 : DbMigration
//    {
//        public override void Up()
//        {
//            AddColumn("dbo.SelectOptions", "GroupedFieldId", c => c.Guid(nullable: false));
//            DropColumn("dbo.SelectOptions", "FieldId");
//        }
        
//        public override void Down()
//        {
//            AddColumn("dbo.SelectOptions", "FieldId", c => c.Guid(nullable: false));
//            DropColumn("dbo.SelectOptions", "GroupedFieldId");
//        }
//    }
//}
