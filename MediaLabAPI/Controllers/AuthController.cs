using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediaLabAPI.Configurations;
using MediaLabAPI.Data;
using MediaLabAPI.DTOs.Common;
using MediaLabAPI.Models;

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

        // CREAZIONE NUOVO UTENTE
        [HttpPost("create-user")]
        [Authorize]
        public IActionResult CreateUser([FromBody] CreateUserDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verifica che l'username non esista già
                var existingUser = _db.SysUsers.FirstOrDefault(u => u.Username == request.Username);
                if (existingUser != null)
                {
                    return Conflict("Username già esistente");
                }

                // Verifica che l'email non esista già (se fornita)
                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    var existingEmail = _db.SysUsers.FirstOrDefault(u => u.Email == request.Email);
                    if (existingEmail != null)
                    {
                        return Conflict("Email già registrata");
                    }
                }

                // Verifica che la company esista
                var company = _db.C_ANA_Companies.FirstOrDefault(c => c.Id == request.IdCompany);
                if (company == null)
                {
                    return BadRequest("IdCompany non valido");
                }

                // Hash della password
                var passwordHash = Convert.ToBase64String(
                    System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(request.Password))
                );

                // Creazione nuovo utente
                var newUser = new SysUser
                {
                    Id = Guid.NewGuid(),
                    IdWhr = request.IdWhr,
                    IdCompany = request.IdCompany,
                    Username = request.Username,
                    PasswordHash = passwordHash,
                    Email = request.Email,
                    IsAdmin = request.IsAdmin,
                    IsEnabled = true,
                    AccessLevel = request.AccessLevel,
                    CreatedAt = DateTime.UtcNow
                };

                _db.SysUsers.Add(newUser);
                _db.SaveChanges();

                return Ok(new CreateUserResponseDto
                {
                    Id = newUser.Id,
                    Username = newUser.Username,
                    Email = newUser.Email ?? string.Empty,
                    IdCompany = newUser.IdCompany,
                    CompanyName = company.RagioneSociale ?? "N/A",
                    Message = "Utente creato con successo",
                    CreatedAt = newUser.CreatedAt,
                    IsAdmin = newUser.IsAdmin,
                    AccessLevel = newUser.AccessLevel
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante la creazione dell'utente: {ex.Message}");
            }
        }

        // CREAZIONE UTENTE AFFILIATO (combinata)
        [HttpPost("create-affiliate-user")]
        [Authorize]
        public IActionResult CreateAffiliateUser([FromBody] CreateAffiliateUserDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verifica che il customer/company esista e sia affiliato
                var customer = _db.C_ANA_Companies.FirstOrDefault(c => c.Id == request.IdCustomer && c.isAffiliate == true);
                if (customer == null)
                {
                    return BadRequest("Customer non trovato o non affiliato");
                }

                // Verifica che l'username non esista già
                var existingUser = _db.SysUsers.FirstOrDefault(u => u.Username == request.Username);
                if (existingUser != null)
                {
                    return Conflict("Username già esistente");
                }

                // Verifica che l'email non esista già (se fornita)
                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    var existingEmail = _db.SysUsers.FirstOrDefault(u => u.Email == request.Email);
                    if (existingEmail != null)
                    {
                        return Conflict("Email già registrata");
                    }
                }

                // Hash della password
                var passwordHash = Convert.ToBase64String(
                    System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(request.Password))
                );

                // Creazione nuovo utente affiliato
                var newUser = new SysUser
                {
                    Id = Guid.NewGuid(),
                    IdCompany = request.IdCustomer,
                    Username = request.Username,
                    PasswordHash = passwordHash,
                    Email = request.Email,
                    IsAdmin = false, // Gli affiliati non sono admin
                    IsEnabled = true,
                    AccessLevel = request.AccessLevel,
                    CreatedAt = DateTime.UtcNow
                };

                _db.SysUsers.Add(newUser);
                _db.SaveChanges();

                return Ok(new CreateAffiliateUserResponseDto
                {
                    UserId = newUser.Id,
                    Username = newUser.Username,
                    Email = newUser.Email ?? string.Empty,
                    CustomerId = request.IdCustomer,
                    CustomerName = customer.RagioneSociale ?? "N/A",
                    AccessLevel = newUser.AccessLevel ?? "Affiliate",
                    CreatedAt = newUser.CreatedAt,
                    Message = "Utente affiliato creato con successo"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante la creazione dell'utente affiliato: {ex.Message}");
            }
        }

        // AGGIORNAMENTO UTENTE
        [HttpPut("update-user/{userId}")]
        [Authorize]
        public IActionResult UpdateUser(Guid userId, [FromBody] UpdateUserDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = _db.SysUsers.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return NotFound("Utente non trovato");
                }

                // Aggiorna solo i campi forniti
                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    var existingEmail = _db.SysUsers.FirstOrDefault(u => u.Email == request.Email && u.Id != userId);
                    if (existingEmail != null)
                    {
                        return Conflict("Email già registrata");
                    }
                    user.Email = request.Email;
                }

                if (request.IsEnabled.HasValue)
                    user.IsEnabled = request.IsEnabled.Value;

                if (request.IsAdmin.HasValue)
                    user.IsAdmin = request.IsAdmin.Value;

                if (request.AccessLevel != null)
                    user.AccessLevel = request.AccessLevel;

                _db.SaveChanges();
                return Ok("Utente aggiornato con successo");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante l'aggiornamento: {ex.Message}");
            }
        }

        // LISTA UTENTI PER COMPANY
        [HttpGet("users/{companyId}")]
        [Authorize]
        public IActionResult GetUsersByCompany(Guid companyId)
        {
            try
            {
                var company = _db.C_ANA_Companies.FirstOrDefault(c => c.Id == companyId);
                if (company == null)
                {
                    return NotFound("Company non trovata");
                }

                var users = _db.SysUsers
                    .Where(u => u.IdCompany == companyId)
                    .Select(u => new UserDetailDto
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                        IdCompany = u.IdCompany,
                        CompanyName = company.RagioneSociale ?? "N/A",
                        IsAdmin = u.IsAdmin,
                        IsEnabled = u.IsEnabled,
                        AccessLevel = u.AccessLevel,
                        CreatedAt = u.CreatedAt,
                        LastLogin = u.LastLogin,
                        IdWhr = u.IdWhr
                    })
                    .OrderBy(u => u.Username)
                    .ToList();

                var response = new UsersListDto
                {
                    Users = users,
                    TotalCount = users.Count,
                    CompanyId = companyId,
                    CompanyName = company.RagioneSociale ?? "N/A"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante il recupero: {ex.Message}");
            }
        }

        // CAMBIO PASSWORD
        [HttpPut("change-password/{userId}")]
        [Authorize]
        public IActionResult ChangePassword(Guid userId, [FromBody] ChangePasswordDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = _db.SysUsers.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return NotFound("Utente non trovato");
                }

                var newPasswordHash = Convert.ToBase64String(
                    System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(request.NewPassword))
                );

                user.PasswordHash = newPasswordHash;
                _db.SaveChanges();

                return Ok("Password cambiata con successo");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante il cambio password: {ex.Message}");
            }
        }

        // DISABILITAZIONE/RIABILITAZIONE UTENTE
        [HttpPut("toggle-user-status/{userId}")]
        [Authorize]
        public IActionResult ToggleUserStatus(Guid userId)
        {
            try
            {
                var user = _db.SysUsers.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return NotFound("Utente non trovato");
                }

                user.IsEnabled = !user.IsEnabled;
                _db.SaveChanges();

                var status = user.IsEnabled ? "abilitato" : "disabilitato";
                return Ok($"Utente {status} con successo");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante il cambio stato: {ex.Message}");
            }
        }

    }
}

