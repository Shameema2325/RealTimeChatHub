using System;

public class ChatRoom
{
    public int Id { get; set; } // Unique identifier for the chat room
    public string RoomName { get; set; } // Name of the chat room
    public DateTime CreatedAt { get; set; } // Timestamp for when the room was created
    public int CreatedBy { get; set; } // ID of the user who created the room
}
