//namespace BulletinExample.Migrations
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class InitialCreate : DbMigration
//    {
//        public override void Up()
//        {
//            CreateTable(
//                "dbo.Accesses",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        BoardId = c.Guid(nullable: false),
//                        UserId = c.Guid(nullable: false),
//                        Login = c.String(),
//                        Password = c.String(),
//                        State = c.Int(nullable: false),
//                        LastChangeId = c.Guid(),
//                        HasRemoved = c.Boolean(nullable: false),
//                        CreateDate = c.DateTime(nullable: false),
//                    })
//                .PrimaryKey(t => t.Id);
            
//            CreateTable(
//                "dbo.Applications",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        Token = c.String(),
//                        UserId = c.Guid(nullable: false),
//                        LastChangeId = c.Guid(),
//                        HasRemoved = c.Boolean(nullable: false),
//                        CreateDate = c.DateTime(nullable: false),
//                    })
//                .PrimaryKey(t => t.Id);
            
//            CreateTable(
//                "dbo.Boards",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        Name = c.String(),
//                        LastChangeId = c.Guid(),
//                        HasRemoved = c.Boolean(nullable: false),
//                        CreateDate = c.DateTime(nullable: false),
//                    })
//                .PrimaryKey(t => t.Id);
            
//            CreateTable(
//                "dbo.BulletinInstances",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        BulletinId = c.Guid(nullable: false),
//                        BoardId = c.Guid(nullable: false),
//                        AccessId = c.Guid(nullable: false),
//                        Url = c.String(),
//                        State = c.Int(nullable: false),
//                        LastChangeId = c.Guid(),
//                        HasRemoved = c.Boolean(nullable: false),
//                        CreateDate = c.DateTime(nullable: false),
//                    })
//                .PrimaryKey(t => t.Id);
            
//            CreateTable(
//                "dbo.Bulletins",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        UserId = c.Guid(nullable: false),
//                        LastChangeId = c.Guid(),
//                        HasRemoved = c.Boolean(nullable: false),
//                        CreateDate = c.DateTime(nullable: false),
//                    })
//                .PrimaryKey(t => t.Id);
            
//            CreateTable(
//                "dbo.CategoryTemplates",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        BoardId = c.Guid(nullable: false),
//                        ParentId = c.Guid(nullable: false),
//                        Name = c.String(),
//                        LastChangeId = c.Guid(),
//                        HasRemoved = c.Boolean(nullable: false),
//                        CreateDate = c.DateTime(nullable: false),
//                    })
//                .PrimaryKey(t => t.Id);
            
//            CreateTable(
//                "dbo.FieldTemplates",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        Name = c.String(),
//                        Tag = c.String(),
//                        Attribute = c.String(),
//                        IsImage = c.Boolean(nullable: false),
//                        IsDynamic = c.Boolean(nullable: false),
//                        LastChangeId = c.Guid(),
//                        HasRemoved = c.Boolean(nullable: false),
//                        CreateDate = c.DateTime(nullable: false),
//                    })
//                .PrimaryKey(t => t.Id);
            
//            CreateTable(
//                "dbo.GroupedCategories",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        CategoryId = c.Guid(nullable: false),
//                        GroupId = c.Guid(nullable: false),
//                        LastChangeId = c.Guid(),
//                        HasRemoved = c.Boolean(nullable: false),
//                        CreateDate = c.DateTime(nullable: false),
//                    })
//                .PrimaryKey(t => t.Id);
            
//            CreateTable(
//                "dbo.GroupedFields",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        FieldId = c.Guid(nullable: false),
//                        GroupId = c.Guid(nullable: false),
//                        LastChangeId = c.Guid(),
//                        HasRemoved = c.Boolean(nullable: false),
//                        CreateDate = c.DateTime(nullable: false),
//                    })
//                .PrimaryKey(t => t.Id);
            
//            CreateTable(
//                "dbo.Groups",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        BoardId = c.Guid(nullable: false),
//                        Hash = c.String(),
//                        LastChangeId = c.Guid(),
//                        HasRemoved = c.Boolean(nullable: false),
//                        CreateDate = c.DateTime(nullable: false),
//                    })
//                .PrimaryKey(t => t.Id);
            
//            CreateTable(
//                "dbo.SelectOptions",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        Name = c.String(),
//                        Code = c.String(),
//                        FieldId = c.Guid(nullable: false),
//                        LastChangeId = c.Guid(),
//                        HasRemoved = c.Boolean(nullable: false),
//                        CreateDate = c.DateTime(nullable: false),
//                    })
//                .PrimaryKey(t => t.Id);
            
//            CreateTable(
//                "dbo.UserGroups",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        GroupId = c.Guid(nullable: false),
//                        UserId = c.Guid(nullable: false),
//                        LastChangeId = c.Guid(),
//                        HasRemoved = c.Boolean(nullable: false),
//                        CreateDate = c.DateTime(nullable: false),
//                    })
//                .PrimaryKey(t => t.Id);
            
//            CreateTable(
//                "dbo.Users",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        Login = c.String(),
//                        Hash = c.String(),
//                        State = c.Int(nullable: false),
//                        LastChangeId = c.Guid(),
//                        HasRemoved = c.Boolean(nullable: false),
//                        CreateDate = c.DateTime(nullable: false),
//                    })
//                .PrimaryKey(t => t.Id);
            
//        }
        
//        public override void Down()
//        {
//            DropTable("dbo.Users");
//            DropTable("dbo.UserGroups");
//            DropTable("dbo.SelectOptions");
//            DropTable("dbo.Groups");
//            DropTable("dbo.GroupedFields");
//            DropTable("dbo.GroupedCategories");
//            DropTable("dbo.FieldTemplates");
//            DropTable("dbo.CategoryTemplates");
//            DropTable("dbo.Bulletins");
//            DropTable("dbo.BulletinInstances");
//            DropTable("dbo.Boards");
//            DropTable("dbo.Applications");
//            DropTable("dbo.Accesses");
//        }
//    }
//}
