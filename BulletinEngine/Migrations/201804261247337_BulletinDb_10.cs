//namespace BulletinEngine.Contexts
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class BulletinDb_10 : DbMigration
//    {
//        public override void Up()
//        {
//            AddColumn("dbo.Accesses", "ActivationCheckLast", c => c.DateTime());
//            AddColumn("dbo.Accesses", "ActivationCheckNext", c => c.DateTime());
//        }
        
//        public override void Down()
//        {
//            DropColumn("dbo.Accesses", "ActivationCheckNext");
//            DropColumn("dbo.Accesses", "ActivationCheckLast");
//        }
//    }
//}
