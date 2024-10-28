using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RealTimeChatHub.Models
{
    public class ChatAppDBContext : DbContext
    {
        public ChatAppDBContext() : base("name=ChatBot")
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}