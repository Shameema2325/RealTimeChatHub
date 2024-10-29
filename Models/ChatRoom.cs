using System.ComponentModel.DataAnnotations;

public class ChatRoom
{
    [Key]
    public int RoomId { get; set; }
    public string RoomName { get; set; }
}
