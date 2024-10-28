namespace RealTimeChatHub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        SenderId = c.Int(nullable: false),
                        ReceiverId = c.Int(nullable: false),
                        MessageText = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                        IsDelivered = c.Boolean(nullable: false),
                        Receiver_UserId = c.Int(),
                        Sender_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.Users", t => t.Receiver_UserId)
                .ForeignKey("dbo.Users", t => t.Sender_UserId)
                .Index(t => t.Receiver_UserId)
                .Index(t => t.Sender_UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        PasswordHash = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "Sender_UserId", "dbo.Users");
            DropForeignKey("dbo.Messages", "Receiver_UserId", "dbo.Users");
            DropIndex("dbo.Messages", new[] { "Sender_UserId" });
            DropIndex("dbo.Messages", new[] { "Receiver_UserId" });
            DropTable("dbo.Users");
            DropTable("dbo.Messages");
        }
    }
}
