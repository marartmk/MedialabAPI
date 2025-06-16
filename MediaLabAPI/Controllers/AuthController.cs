using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediaLabAPI.Configurations;

namespace MediaLabAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtSettings _jwt;

        public AuthController(IOptions<JwtSettings> jwtOptions)
        {
            _jwt = jwtOptions.Value;
        }

        public class LoginRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class TokenRequest
        {
            public string Token { get; set; } = string.Empty;
        }

        // 🔐 LOGIN
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Username == "admin@nextsrl.it" && request.Password == "test")
            {
                var jwt = GenerateJwtToken(request.Username, "Admin");
                return Ok(new { token = jwt });
            }

            return Unauthorized("Invalid credentials");
        }

        // 📝 REGISTRAZIONE (mock)
        [HttpPost("register")]
        public IActionResult Register([FromBody] LoginRequest request)
        {
            // In futuro: salva in un database con password hashata
            return Ok($"Utente {request.Username} registrato con successo (mock)");
        }

        // 🔓 LOGOUT (mock)
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // Client side: basta rimuovere il token
            return Ok("Logout effettuato con successo");
        }

        // 👤 WHOAMI - info utente autenticato
        [HttpGet("whoami")]
        [Authorize]
        public IActionResult WhoAmI()
        {
            var email = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            return Ok(new { email, role });
        }

        // ✅ PROTECTED - test autenticazione
        [HttpGet("protected")]
        [Authorize]
        public IActionResult Protected()
        {
            var userName = User.Identity?.Name;
            return Ok($"You are authenticated as {userName}");
        }

        // ✅ VALIDAZIONE TOKEN (mock)
        [HttpPost("validate")]
        public IActionResult ValidateToken([FromBody] TokenRequest request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwt.Key);

            try
            {
                tokenHandler.ValidateToken(request.Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return Ok("Token valido");
            }
            catch
            {
                return Unauthorized("Token non valido");
            }
        }

        // 🔄 REFRESH TOKEN (mock)
        [HttpPost("refresh")]
        public IActionResult RefreshToken([FromBody] TokenRequest request)
        {
            // In produzione: verifichi il refresh token da DB
            var newToken = GenerateJwtToken("admin@test.com", "Admin");
            return Ok(new { token = newToken });
        }

        // 🔧 Generatore token privato
        private string GenerateJwtToken(string email, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwt.Key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

