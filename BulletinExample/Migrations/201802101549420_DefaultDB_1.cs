//namespace BulletinExample.Entity.Context
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class DefaultDB_1 : DbMigration
//    {
//        public override void Up()
//        {
//            //AddColumn("dbo.Applications", "State", c => c.Int(nullable: false));
//            //AddColumn("dbo.Boards", "State", c => c.Int(nullable: false));
//            //AddColumn("dbo.Bulletins", "State", c => c.Int(nullable: false));
//            //AddColumn("dbo.CategoryTemplates", "State", c => c.Int(nullable: false));
//            //AddColumn("dbo.FieldTemplates", "State", c => c.Int(nullable: false));
//            //AddColumn("dbo.GroupedCategories", "State", c => c.Int(nullable: false));
//            //AddColumn("dbo.GroupedFields", "State", c => c.Int(nullable: false));
//            //AddColumn("dbo.Groups", "State", c => c.Int(nullable: false));
//            //AddColumn("dbo.SelectOptions", "State", c => c.Int(nullable: false));
//            //AddColumn("dbo.UserGroups", "State", c => c.Int(nullable: false));
//        }
        
//        public override void Down()
//        {
//            DropColumn("dbo.UserGroups", "State");
//            DropColumn("dbo.SelectOptions", "State");
//            DropColumn("dbo.Groups", "State");
//            DropColumn("dbo.GroupedFields", "State");
//            DropColumn("dbo.GroupedCategories", "State");
//            DropColumn("dbo.FieldTemplates", "State");
//            DropColumn("dbo.CategoryTemplates", "State");
//            DropColumn("dbo.Bulletins", "State");
//            DropColumn("dbo.Boards", "State");
//            DropColumn("dbo.Applications", "State");
//        }
//    }
//}
