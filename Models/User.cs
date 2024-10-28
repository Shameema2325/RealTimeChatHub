using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealTimeChatHub.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
    }
}