using RealTimeChatHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RealTimeChatHub.Controllers
{
    public class AuthController : ApiController
    {
        private readonly ChatAppDBContext _context = new ChatAppDBContext();

        [HttpPost]
        [Route("api/auth/login")]
        public IHttpActionResult Login([FromBody] LoginRequest request)
        {
            // Validate credentials
            var user = _context.Users.FirstOrDefault(u => u.UserName == request.Username && u.UserName == request.Password);

            if (user == null)
                return Unauthorized(); // Return 401 if credentials are invalid

            // Generate a simple token or use JWT for authentication (optional)
            string token = GenerateToken(user); // Implement token generation if needed
            return Ok(new { Token = token, Username = user.UserName });
        }

        private string GenerateToken(User user)
        {
            // Implement a token generation logic here
            // Return a dummy token for this example
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}