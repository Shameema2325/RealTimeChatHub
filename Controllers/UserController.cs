using RealTimeChatHub.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
                PasswordHash = hashedPassword,
                Email= registerRequest.Email
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

            return Ok(new { user.UserId});
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return HashPassword(password) == storedHash;
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
            public string Email { get; set; }
        }

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}