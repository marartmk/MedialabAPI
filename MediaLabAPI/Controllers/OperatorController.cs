using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediaLabAPI.Data;
using MediaLabAPI.Models;
using AutoMapper;
using MediaLabAPI.DTOs;

namespace MediaLabAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OperatorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OperatorController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // 🔹 GET: api/operator
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OperatorDto>>> GetAll()
        {
            try
            {
                var operators = await _context.C_ANA_Operators
                    .Where(c => c.IsDeleted == false || c.IsDeleted == null)
                    .ToListAsync();

                var dto = _mapper.Map<List<OperatorDto>>(operators);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

        // 🔹 GET: api/customer/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OperatorDto>> GetById(Guid id)
        {
            try
            {
                var op = await _context.C_ANA_Operators
                    .Where(c => c.Id == id && (c.IsDeleted == false || c.IsDeleted == null))
                    .FirstOrDefaultAsync();

                if (op == null)
                    return NotFound($"Operatore con ID {id} non trovato");

                var dto = _mapper.Map<OperatorDto>(op);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

        // 🔹 POST: api/operators
        [HttpPost]
        public async Task<ActionResult<C_ANA_Operators>> Create(C_ANA_Operators operators)
        {
            try
            {
                // 🔹 VALORI OBBLIGATORI
                operators.Id = Guid.NewGuid();
                operators.CreatedAt = DateTime.UtcNow;
                operators.IsDeleted = false;

                // 🔹 VALORI DEFAULT PER CAMPI NON NULLABLE
                operators.EmailConfirmed = operators.EmailConfirmed;
                operators.PhoneNumberConfirmed = operators.PhoneNumberConfirmed;
                operators.TwoFactorEnabled = operators.TwoFactorEnabled;
                operators.LockoutEnabled = operators.LockoutEnabled;
                operators.AccessFailedCount = operators.AccessFailedCount;
                operators.TwoFactorEnabled = operators.TwoFactorEnabled;
                operators.FirstName = operators.FirstName;
                operators.LastName = operators.LastName;
                operators.ReceiveSms = operators.ReceiveSms;
                operators.ReceiveEmail = operators.ReceiveEmail;

                _context.C_ANA_Operators.Add(operators);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = operators.Id }, operators);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante la creazione: {ex.Message}");
            }
        }

        // 🔹 PUT: api/customer/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, C_ANA_Operators updatedOperators)
        {
            try
            {
                if (id != updatedOperators.Id)
                    return BadRequest("ID non corrispondente");

                var existingOperators = await _context.C_ANA_Operators
                    .Where(c => c.Id == id && (c.IsDeleted == false || c.IsDeleted == null))
                    .FirstOrDefaultAsync();

                if (existingOperators == null)
                    return NotFound($"Operatore con ID {id} non trovato");

                // 🔹 AGGIORNA SOLO I CAMPI MODIFICABILI
                existingOperators.UserName = updatedOperators.UserName;
                existingOperators.Email = updatedOperators.Email;
                existingOperators.InternalCode = updatedOperators.InternalCode;
                existingOperators.FirstName = updatedOperators.FirstName;
                existingOperators.LastName = updatedOperators.LastName;
                existingOperators.PhoneNumber = updatedOperators.PhoneNumber;
                existingOperators.Idcompany = updatedOperators.Idcompany;
                existingOperators.Active  = updatedOperators.Active;
                existingOperators.DataCreazione = DateTime.UtcNow;
                existingOperators.Regione = updatedOperators.Regione;
                existingOperators.Indirizzo = updatedOperators.Indirizzo;
                existingOperators.Cap = updatedOperators.Cap;
                existingOperators.Regione = updatedOperators.Regione;
                existingOperators.Provincia = updatedOperators.Provincia;
                existingOperators.Citta = updatedOperators.Citta;
                existingOperators.Indirizzo = updatedOperators.Indirizzo;
                existingOperators.Cap = updatedOperators.Cap;
                existingOperators.CodiceFiscale = updatedOperators.CodiceFiscale;              
                existingOperators.MultiTenantId = updatedOperators.MultiTenantId;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante l'aggiornamento: {ex.Message}");
            }
        }

        // 🔹 DELETE: api/customer/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            try
            {
                var customer = await _context.C_ANA_Operators
                    .Where(c => c.Id == id && (c.IsDeleted == false || c.IsDeleted == null))
                    .FirstOrDefaultAsync();

                if (customer == null)
                    return NotFound($"Operatore con ID {id} non trovato");

                customer.IsDeleted = true;
                customer.DataEliminazione = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante la cancellazione: {ex.Message}");
            }
        }

        // 🔹 SEARCH: Ricerca utente con parametri
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] Guid? multitenantId)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Il parametro di ricerca è obbligatorio.");

            var results = await _context.C_ANA_Operators
                .Where(c =>
                    (c.IsDeleted == false || c.IsDeleted == null) &&
                    (!multitenantId.HasValue || c.MultiTenantId == multitenantId.Value) && // filtro per tenant
                    (
                        c.FirstName.Contains(query) ||
                        c.LastName.Contains(query) ||                        
                        c.PhoneNumber.Contains(query) ||
                        c.Citta.Contains(query)
                    ))
                .Take(50)
                .ToListAsync();

            return Ok(results);
        }
    }
}
