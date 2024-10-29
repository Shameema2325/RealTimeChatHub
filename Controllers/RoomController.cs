using System;
using System.Linq;
using System.Web.Http;
using RealTimeChatHub.Models;

namespace RealTimeChatHub.Controllers
{
    [RoutePrefix("api/rooms")]
    public class RoomController : ApiController
    {
        private readonly ChatAppDBContext _context = new ChatAppDBContext();

        [HttpGet]
        [Route("list")]
        public IHttpActionResult GetChatRooms()
        {
            var rooms = _context.ChatRooms
                .Select(r => new { r.Id, r.RoomName })
                .ToList();

            return Ok(rooms);
        }

        // POST: api/rooms/create
        [HttpPost]
        [Route("create")]
        public IHttpActionResult CreateRoom([FromBody] RoomDto roomDto)
        {
            // Validate input
            if (roomDto == null || string.IsNullOrWhiteSpace(roomDto.RoomName))
                return BadRequest("Invalid room data.");

            // Create and save the room
            var room = new ChatRoom
            {
                RoomName = roomDto.RoomName,
                CreatedAt = DateTime.UtcNow // Store in UTC
            };

            _context.ChatRooms.Add(room);
            _context.SaveChanges();

            return CreatedAtRoute("GetRoom", new { id = room.Id }, room); // Return created room
        }
    }

    // DTO for room transfer
    public class RoomDto
    {
        public string RoomName { get; set; }
    }
}
