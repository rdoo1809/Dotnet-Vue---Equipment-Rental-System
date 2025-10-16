using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Midterm_PROG3340_RDooley.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    // in-memory users
    private readonly List<(string Username, string Password, string Role)> _users = new()
    {
        ("admin", "admin123", "Admin"),
        ("user", "user123", "User")
    };

    [HttpPost("login")]
    public ActionResult<string> Login([FromBody] LoginRequest request)
    {
        var user = _users.FirstOrDefault(u => 
            u.Username == request.Username && u.Password == request.Password);

        if (user == default)
            return Unauthorized("Invalid credentials");

        var token = GenerateJwtToken(user.Username, user.Role);
        return Ok(new { token });
    }

    private object GenerateJwtToken(string userName, string userRole)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Role, userRole)
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("YourSuperSecretKeyHere1234567890"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}