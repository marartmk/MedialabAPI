using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediaLabAPI.Data;
using MediaLabAPI.Models;

namespace MediaLabAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 GET: api/customer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<C_ANA_Company>>> GetAll()
        {
            try
            {
                var companies = await _context.C_ANA_Companies
                    .Where(c => c.IsDeleted == false || c.IsDeleted == null)
                    .ToListAsync();

                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

        // 🔹 GET: api/customer/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<C_ANA_Company>> GetById(Guid id)
        {
            try
            {
                // 🔹 PRIMA PROVA: FirstOrDefaultAsync
                var customer = await _context.C_ANA_Companies
                    .Where(c => c.Id == id && (c.IsDeleted == false || c.IsDeleted == null))
                    .FirstOrDefaultAsync();

                if (customer == null)
                {
                    return NotFound($"Cliente con ID {id} non trovato");
                }

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

        // 🔹 POST: api/customer
        [HttpPost]
        public async Task<ActionResult<C_ANA_Company>> Create(C_ANA_Company customer)
        {
            try
            {
                // 🔹 VALORI OBBLIGATORI
                customer.Id = Guid.NewGuid();
                customer.CreatedAt = DateTime.UtcNow;
                customer.IsDeleted = false;

                // 🔹 VALORI DEFAULT PER CAMPI NON NULLABLE
                customer.EnabledFE = customer.EnabledFE; // Mantieni il valore passato
                customer.IsVendolo = customer.IsVendolo; // Mantieni il valore passato
                customer.IsVendoloFE = customer.IsVendoloFE; // Mantieni il valore passato

                _context.C_ANA_Companies.Add(customer);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante la creazione: {ex.Message}");
            }
        }

        // 🔹 PUT: api/customer/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, C_ANA_Company updatedCustomer)
        {
            try
            {
                if (id != updatedCustomer.Id)
                    return BadRequest("ID non corrispondente");

                var existingCustomer = await _context.C_ANA_Companies
                    .Where(c => c.Id == id && (c.IsDeleted == false || c.IsDeleted == null))
                    .FirstOrDefaultAsync();

                if (existingCustomer == null)
                    return NotFound($"Cliente con ID {id} non trovato");

                // 🔹 AGGIORNA SOLO I CAMPI MODIFICABILI
                existingCustomer.RagioneSociale = updatedCustomer.RagioneSociale;
                existingCustomer.PIva = updatedCustomer.PIva;
                existingCustomer.FiscalCode = updatedCustomer.FiscalCode;
                existingCustomer.EmailAziendale = updatedCustomer.EmailAziendale;
                existingCustomer.Telefono = updatedCustomer.Telefono;
                existingCustomer.isCustomer = updatedCustomer.isCustomer;
                existingCustomer.UpdatedAt = DateTime.UtcNow;

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
                var customer = await _context.C_ANA_Companies
                    .Where(c => c.Id == id && (c.IsDeleted == false || c.IsDeleted == null))
                    .FirstOrDefaultAsync();

                if (customer == null)
                    return NotFound($"Cliente con ID {id} non trovato");

                customer.IsDeleted = true;
                customer.DeletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante la cancellazione: {ex.Message}");
            }
        }

        // 🔹 DEBUG: Endpoint per verificare se l'ID esiste
        [HttpGet("debug/{id}")]
        public async Task<IActionResult> Debug(Guid id)
        {
            try
            {
                // Cerca senza filtri
                var customer = await _context.C_ANA_Companies
                    .Where(c => c.Id == id)
                    .FirstOrDefaultAsync();

                if (customer == null)
                {
                    return NotFound($"Nessun record trovato con ID: {id}");
                }

                return Ok(new
                {
                    Found = true,
                    Id = customer.Id,
                    RagioneSociale = customer.RagioneSociale,
                    IsDeleted = customer.IsDeleted,
                    CreatedAt = customer.CreatedAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore debug: {ex.Message}");
            }
        }

        // 🔹 SEARCH: Ricerca utente con parametri
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Il parametro di ricerca è obbligatorio.");

            var results = await _context.C_ANA_Companies
                .Where(c =>
                    (c.IsDeleted == false || c.IsDeleted == null) &&
                    (
                        c.RagioneSociale.Contains(query) ||
                        c.PIva.Contains(query) ||
                        c.Telefono.Contains(query) ||
                        c.Citta.Contains(query)
                    ))
                .Take(50) // limite per performance
                .ToListAsync();

            return Ok(results);
        }
    }
}