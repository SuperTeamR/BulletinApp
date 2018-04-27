//namespace BulletinEngine.Contexts
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class BulletinDb_11 : DbMigration
//    {
//        public override void Up()
//        {
//            AddColumn("dbo.Accesses", "GenerationCheckLast", c => c.DateTime());
//            AddColumn("dbo.Accesses", "GenerationCheckNext", c => c.DateTime());
//            AddColumn("dbo.Bulletins", "GenerationCheckLast", c => c.DateTime());
//            AddColumn("dbo.Bulletins", "GenerationCheckNext", c => c.DateTime());
//            AddColumn("dbo.Tasks", "InstanceId", c => c.Guid());
//            DropColumn("dbo.Accesses", "ActivationCheckLast");
//            DropColumn("dbo.Accesses", "ActivationCheckNext");
//            DropColumn("dbo.Tasks", "BulletinId");
//        }
        
//        public override void Down()
//        {
//            AddColumn("dbo.Tasks", "BulletinId", c => c.Guid());
//            AddColumn("dbo.Accesses", "ActivationCheckNext", c => c.DateTime());
//            AddColumn("dbo.Accesses", "ActivationCheckLast", c => c.DateTime());
//            DropColumn("dbo.Tasks", "InstanceId");
//            DropColumn("dbo.Bulletins", "GenerationCheckNext");
//            DropColumn("dbo.Bulletins", "GenerationCheckLast");
//            DropColumn("dbo.Accesses", "GenerationCheckNext");
//            DropColumn("dbo.Accesses", "GenerationCheckLast");
//        }
//    }
//}
