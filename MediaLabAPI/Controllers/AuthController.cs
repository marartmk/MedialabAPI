using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediaLabAPI.Configurations;
using MediaLabAPI.Data;

namespace MediaLabAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtSettings _jwt;
        private readonly AppDbContext _db;

        public AuthController(IOptions<JwtSettings> jwtOptions, AppDbContext db)
        {
            _jwt = jwtOptions.Value;
            _db = db;
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
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Username e Password sono obbligatori");

            // Cerca l'utente nel db
            var user = _db.SysUsers.FirstOrDefault(a =>
                a.Username == request.Username &&
                a.IsEnabled == true);

            if (user == null)
                return Unauthorized("Utente amministratore non trovato o disabilitato");

            // Verifica la password hashata
            var inputPasswordHash = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(request.Password))
            );

            if (user.PasswordHash != inputPasswordHash)
                return Unauthorized("Password errata");

            // Aggiorna data ultimo accesso
            user.LastLogin = DateTime.UtcNow;
            _db.SaveChanges();

            // Recupera il nome dell'azienda (C_ANA_Companies) tramite IdCompany
            var company = _db.C_ANA_Companies.FirstOrDefault(c => c.Id == user.IdCompany);

            // Genera il token
            var jwt = GenerateJwtToken(request.Username, "Admin");

            return Ok(new
            {
                token = jwt,
                fullName = user.Username,
                email = user.Email,
                id = user.Id,
                idCompany = user.IdCompany,
                companyName = company?.RagioneSociale ?? "Azienda sconosciuta"
            });
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

        // 🔐 LOGIN ADMIN
        [HttpPost("login-admin")]
        public IActionResult LoginAdmin([FromBody] LoginRequest request)
        {
            // Verifica che i parametri siano presenti
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Username e Password sono obbligatori");

            // Cerca l'admin nel DB
            var admin = _db.SysAdmins.FirstOrDefault(a =>
                a.Username == request.Username &&
                a.IsEnabled == true);

            if (admin == null)
                return Unauthorized("Utente amministratore non trovato o disabilitato");

            // Verifica la password hashata
            var inputPasswordHash = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(request.Password))
            );

            if (admin.PasswordHash != inputPasswordHash)
                return Unauthorized("Password errata");

            // Aggiorna data ultimo accesso
            admin.LastLogin = DateTime.UtcNow;
            _db.SaveChanges();

            // Genera il token
            var role = admin.IsSuperAdmin == true ? "SuperAdmin" : "Admin";
            var jwt = GenerateJwtToken(admin.Username, role);

            return Ok(new
            {
                token = jwt,
                fullName = admin.FullName,
                email = admin.Email,
                idCompany = admin.IdCompany
            });
        }

        // 🔐 LOGIN USER
        [HttpPost("login-user")]
        public IActionResult LoginUser([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Username e Password sono obbligatori");

            // Cerca l'utente nel db
            var user = _db.SysUsers.FirstOrDefault(a =>
                a.Username == request.Username &&
                a.IsEnabled == true);

            if (user == null)
                return Unauthorized("Utente amministratore non trovato o disabilitato");

            // Verifica la password hashata
            var inputPasswordHash = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(request.Password))
            );

            if (user.PasswordHash != inputPasswordHash)
                return Unauthorized("Password errata");

            // Aggiorna data ultimo accesso
            user.LastLogin = DateTime.UtcNow;
            _db.SaveChanges();

            // Recupera il nome dell'azienda (C_ANA_Companies) tramite IdCompany
            var company = _db.C_ANA_Companies.FirstOrDefault(c => c.Id == user.IdCompany);

            // Genera il token
            var jwt = GenerateJwtToken(request.Username, "Admin");

            return Ok(new
            {
                token = jwt,
                fullName = user.Username,
                email = user.Email,
                id = user.Id,
                idCompany = user.IdCompany,
                companyName = company?.RagioneSociale ?? "Azienda sconosciuta"
            });
        }

    }
}

