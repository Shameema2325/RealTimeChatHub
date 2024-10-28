using RealTimeChatHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;

namespace RealTimeChatHub.Controllers
{
    public class UserController : ApiController
    {
        private readonly ChatAppDBContext _context = new ChatAppDBContext();

        [HttpPost]
        [Route("api/user/register")]
        public IHttpActionResult Register([FromBody] RegisterRequest registerRequest)
        {
            if (_context.Users.Any(u => u.UserName == registerRequest.UserName))
            {
                return BadRequest("Username already exists.");
            }

            // Hash the password before storing it
            var hashedPassword = HashPassword(registerRequest.Password);

            var newUser = new User
            {
                UserName = registerRequest.UserName,
                PasswordHash = hashedPassword
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();
            return Ok("Registration successful");
        }


        [HttpPost]
        [Route("api/user/login")]
        public IHttpActionResult Login([FromBody] LoginRequest loginRequest)
        {
            // Find user by username and verify password
            var user = _context.Users.SingleOrDefault(u => u.UserName == loginRequest.Username);

            if (user == null || !VerifyPassword(loginRequest.Password, user.PasswordHash))
            {
                return Unauthorized(); // Invalid username or password
            }

            // Generate and return a token if the login is successful (simplified token for example)
            string token = GenerateToken(user);            
            return Ok(new { UserId = user.UserId, Token = token });
        }

        [HttpGet]
        public IHttpActionResult GetUsers()
        {
            var users = _context.Users.Select(u => new { u.UserId, u.UserName }).ToList();
            return Ok(users);
        }        

        private bool VerifyPassword(string password, string storedHash)
        {
            return HashPassword(password) == storedHash;
        }

        private string GenerateToken(User user)
        {
            // Implement a token generation strategy here (e.g., JWT)
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray()); // Simple token example
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
        public class RegisterRequest
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }
    }
}