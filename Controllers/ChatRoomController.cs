using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using RealTimeChatHub.Models;

namespace RealTimeChatHub.Controllers
{
    public class RoomController : ApiController
    {
        private readonly ChatAppDBContext _dbContext = new ChatAppDBContext();

        [HttpGet]
        [Route("api/rooms")]
        public IHttpActionResult GetRooms()
        {
            var rooms = _dbContext.ChatRooms.ToList();
            return Ok(rooms);
        }

        [HttpPost]
        [Route("api/rooms/create")]
        public IHttpActionResult CreateRoom([FromBody] ChatRoom room)
        {
            if (room == null || string.IsNullOrEmpty(room.RoomName))
            {
                return BadRequest("Room name is required.");
            }

            if (!_dbContext.ChatRooms.Any(r => r.RoomName == room.RoomName))
            {
                _dbContext.ChatRooms.Add(room);
                _dbContext.SaveChanges();
                return Ok(room);
            }

            return Conflict(); // Room already exists
        }
    }
}
