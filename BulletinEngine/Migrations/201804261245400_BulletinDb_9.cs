//namespace BulletinEngine.Contexts
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class BulletinDb_9 : DbMigration
//    {
//        public override void Up()
//        {
//            //DropColumn("dbo.Accesses", "ActivationCheckLast");
//            //DropColumn("dbo.Accesses", "ActivationCheckNext");
//        }
        
//        public override void Down()
//        {
//            AddColumn("dbo.Accesses", "ActivationCheckNext", c => c.DateTime());
//            AddColumn("dbo.Accesses", "ActivationCheckLast", c => c.DateTime());
//        }
//    }
//}
