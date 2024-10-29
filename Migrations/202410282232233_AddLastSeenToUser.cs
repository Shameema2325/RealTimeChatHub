namespace RealTimeChatHub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLastSeenToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Email");
        }
    }
}
