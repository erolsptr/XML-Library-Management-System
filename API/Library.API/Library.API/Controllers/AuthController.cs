using Microsoft.AspNetCore.Mvc;
using Library.API.Models;
using System.Xml.Linq;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;

namespace Library.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _usersFilePath = "users.xml";

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin login)
        {
            // 1. Kullanıcıyı XML'den bul
            var user = AuthenticateUser(login);

            if (user != null)
            {
                // 2. Eğer kullanıcı varsa, JWT oluştur
                var token = GenerateJwtToken(user);
                return Ok(new { token });
            }

            // 3. Kullanıcı yoksa, yetkisiz hatası döndür
            return Unauthorized("Invalid username or password.");
        }

        private UserDto AuthenticateUser(UserLogin login)
        {
            try
            {
                XDocument doc = XDocument.Load(_usersFilePath);
                var userElement = doc.Descendants("User")
                    .FirstOrDefault(u =>
                        u.Element("Username")?.Value == login.Username &&
                        u.Element("Password")?.Value == login.Password);

                if (userElement != null)
                {
                    return new UserDto
                    {
                        Id = (int)userElement.Attribute("ID"),
                        Username = userElement.Element("Username").Value,
                        Role = userElement.Element("Role").Value
                    };
                }
            }
            catch
            {
                // Hata durumunda null döndür, loglama yapılabilir.
                return null;
            }

            return null;
        }

        private string GenerateJwtToken(UserDto user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Token'ın içine koyacağımız bilgiler (claims)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username), // Subject (kullanıcı adı)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Benzersiz Token ID
                new Claim("id", user.Id.ToString()), // Özel claim: kullanıcı ID'si
                new Claim(ClaimTypes.Role, user.Role) // Özel claim: kullanıcı rolü
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(120), // Token 2 saat sonra geçersiz olacak
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}