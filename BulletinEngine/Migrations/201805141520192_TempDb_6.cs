namespace BulletinHub.Contexts
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TempDb_6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Calls",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        VirtualNumber = c.String(),
                        ForwardNumber = c.String(),
                        CallDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Status = c.String(),
                        LastChangeId = c.Guid(),
                        State = c.Int(nullable: false),
                        HasRemoved = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Calls");
        }
    }
}
