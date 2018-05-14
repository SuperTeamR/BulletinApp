namespace BulletinEngine.Contexts
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BulletinDb_20 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bulletins", "Views", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bulletins", "Views");
        }
    }
}
