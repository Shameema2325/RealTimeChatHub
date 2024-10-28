using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealTimeChatHub.Models
{
    public class Message
    {
        public int MessageId { get; set; } // Primary key
        public int SenderId { get; set; } // Foreign key for sender
        public int ReceiverId { get; set; } // Foreign key for receiver
        public string MessageText { get; set; } // Text of the message
        public DateTime Timestamp { get; set; } // Timestamp of the message
        public bool IsDelivered { get; set; }

        // Navigation properties
        public virtual User Sender { get; set; }
        public virtual User Receiver { get; set; }
    }
}