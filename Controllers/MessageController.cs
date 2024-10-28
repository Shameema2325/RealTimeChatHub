using System;
using System.Net.Http;
using System.Web.Http;
using Hangfire;
using Microsoft.AspNet.SignalR;
using RealTimeChatHub.Models; // Adjust this namespace as per your project structure
using RealTimeChatHub.Hubs;

namespace RealTimeChatHub.Controllers
{
    [RoutePrefix("api/messages")]
    public class MessageController : ApiController
    {
        private readonly ChatAppDBContext _context = new ChatAppDBContext();

        [HttpPost]
        [Route("send")]
        public IHttpActionResult SendMessage([FromBody] MessageDto messageDto)
        {
            // Validate input
            if (messageDto == null || string.IsNullOrWhiteSpace(messageDto.MessageText))
                return BadRequest("Invalid message data.");

            // Create and save the message
            var message = new Message
            {
                SenderId = messageDto.SenderId,
                ReceiverId = messageDto.ReceiverId,
                MessageText = messageDto.MessageText,
                Timestamp = DateTime.UtcNow, // Store in UTC
                IsDelivered = false
            };

            _context.Messages.Add(message);
            _context.SaveChanges();

            // Send message using SignalR
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            var receiverClient = hubContext.Clients.User(messageDto.ReceiverId.ToString());

            if (receiverClient != null)
            {
                // Deliver message immediately if online
                receiverClient.receiveMessage(messageDto.SenderId.ToString(), messageDto.MessageText, message.Timestamp);
                message.IsDelivered = true;
                _context.SaveChanges();
            }
            else
            {
                // Schedule delivery if the receiver is offline
                ScheduleMessageDelivery(message.MessageId, messageDto.ReceiverId);
            }

            return Ok();
        }

        private void ScheduleMessageDelivery(int messageId, int receiverId)
        {
            // Schedule delivery to try again after a delay (1 minute in this case)
            BackgroundJob.Schedule(() => DeliverMessage(messageId, receiverId), TimeSpan.FromMinutes(1));
        }

        public void DeliverMessage(int messageId, int receiverId)
        {
            using (var dbContext = new ChatAppDBContext())
            {
                var message = dbContext.Messages.Find(messageId);
                if (message != null && !message.IsDelivered)
                {
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    var receiverClient = hubContext.Clients.User(receiverId.ToString());

                    if (receiverClient != null)
                    {
                        // Deliver the message if the user is online
                        receiverClient.receiveMessage(message.SenderId.ToString(), message.MessageText, message.Timestamp);
                        message.IsDelivered = true;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        // Reschedule if the user is still offline
                        ScheduleMessageDelivery(messageId, receiverId);
                    }
                }
            }
        }
    }

    // DTO for message transfer
    public class MessageDto
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string MessageText { get; set; }
    }
}
