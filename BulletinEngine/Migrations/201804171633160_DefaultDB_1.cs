namespace BulletinHub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DefaultDB_1 : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Users");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Login = c.String(),
                        Hash = c.String(),
                        LastChangeId = c.Guid(),
                        State = c.Int(nullable: false),
                        HasRemoved = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
