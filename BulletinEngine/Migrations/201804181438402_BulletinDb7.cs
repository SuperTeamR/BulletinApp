namespace BulletinHub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BulletinDb7 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserSettings",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        TaskGenerationPeriod = c.Int(nullable: false),
                        LastTimeGeneration = c.DateTime(precision: 7, storeType: "datetime2"),
                        NextTaskGeneration = c.DateTime(precision: 7, storeType: "datetime2"),
                        LastChangeId = c.Guid(),
                        State = c.Int(nullable: false),
                        HasRemoved = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Tasks", "UserId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tasks", "UserId");
            DropTable("dbo.UserSettings");
        }
    }
}
