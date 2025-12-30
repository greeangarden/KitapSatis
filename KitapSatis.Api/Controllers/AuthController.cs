using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using KitapSatis.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace KitapSatis.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public record LoginRequest(string Username, string Password);

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
            if (user == null) return Unauthorized("Invalid credentials");

            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");


            var claims = new[]
            {
                new Claim("uid", user.Id.ToString()),

                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
