//namespace BulletinEngine.Contexts
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class BulletinDb_22 : DbMigration
//    {
//        public override void Up()
//        {
//            AddColumn("dbo.Bulletins", "Model", c => c.String());
//            AddColumn("dbo.Bulletins", "Modifier", c => c.String());
//        }
        
//        public override void Down()
//        {
//            DropColumn("dbo.Bulletins", "Modifier");
//            DropColumn("dbo.Bulletins", "Model");
//        }
//    }
//}
