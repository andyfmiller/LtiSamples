namespace Provider.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovePairedUser : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.PairedUsers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PairedUsers",
                c => new
                    {
                        PairedUserId = c.Int(nullable: false, identity: true),
                        ConsumerId = c.Int(nullable: false),
                        ConsumerUserId = c.String(),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.PairedUserId);
            
        }
    }
}
