using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;

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

            // Attempt to deliver message to the specified user
            Clients.User(receiverId.ToString()).receiveMessage(senderId.ToString(), messageText, DateTime.UtcNow);
        }

        // Handle user connection
        public override Task OnConnected()
        {
            var userId = Context.User.Identity.Name; // Assumes user ID is stored in the user's identity

            if (!string.IsNullOrEmpty(userId))
            {
                // Add the connection to a group named by the user ID for easy targeting
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
            var userId = Context.User.Identity.Name;

            if (!string.IsNullOrEmpty(userId))
            {
                Groups.Remove(Context.ConnectionId, userId);
                Console.WriteLine($"User {userId} disconnected with Connection ID: {Context.ConnectionId}");
            }
            else
            {
                Console.WriteLine("Unable to identify the disconnecting user.");
            }

            return base.OnDisconnected(stopCalled);
        }

        // Broadcast a message to all users in the chat
        public void BroadcastMessage(string userName, string message)
        {
            if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine($"Broadcasting message from {userName}: {message}");
                Clients.All.receiveMessage(userName, message, DateTime.UtcNow);
            }
            else
            {
                Clients.Caller.receiveError("Invalid broadcast message.");
            }
        }
    }
}
