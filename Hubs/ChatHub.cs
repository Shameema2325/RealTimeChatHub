using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RealTimeChatHub.Hubs
{
    public class ChatHub : Hub
    {
        // Method to send a message to a specific user
        public void SendMessage(int senderId, int receiverId, string messageText)
        {
            // Ensure message details are valid
            if (senderId <= 0 || receiverId <= 0 || string.IsNullOrWhiteSpace(messageText))
            {
                Clients.Caller.receiveError("Invalid message or user ID.");
                return;
            }

            // Log the action for debugging
            Console.WriteLine($"Sending message from User {senderId} to User {receiverId}: {messageText}");

            // Deliver message to the specific user
            Clients.User(receiverId.ToString()).receiveMessage(senderId.ToString(), messageText, DateTime.UtcNow, false);
        }

        // Handle user connection
        public override Task OnConnected()
        {
            var userId = Context.QueryString["userId"];
            if (!string.IsNullOrEmpty(userId))
            {
                Groups.Add(Context.ConnectionId, userId);
                Console.WriteLine($"User {userId} connected with Connection ID: {Context.ConnectionId}");
            }
            else
            {
                Console.WriteLine("Unable to identify the connecting user.");
            }

            return base.OnConnected();
        }

        // Handle user disconnection
        public override Task OnDisconnected(bool stopCalled)
        {
            var userId = Context.QueryString["userId"];
            if (!string.IsNullOrEmpty(userId))
            {
                Groups.Remove(Context.ConnectionId, userId);
                Console.WriteLine($"User {userId} disconnected with Connection ID: {Context.ConnectionId}");
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}
