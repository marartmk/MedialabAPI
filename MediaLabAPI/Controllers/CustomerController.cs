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
public async Task<ActionResult<IEnumerable<C_ANA_Company>>> GetAll([FromQuery] Guid multitenantId)
{
    if (multitenantId == Guid.Empty)
        return BadRequest("Il parametro multitenantId è obbligatorio.");

    try
    {
        var companies = await _context.C_ANA_Companies
            .Where(c =>
                (c.IsDeleted == false || c.IsDeleted == null) &&
                c.MultiTenantId == multitenantId)
            .ToListAsync();

        return Ok(companies);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Errore interno: {ex.Message}");
    }
}

        // 🔹 GET: api/customer
        [HttpGet("customeraffiliated")]
        public async Task<IActionResult> GetAffiliatedCustomers([FromQuery] Guid multitenantId)
        {
            if (multitenantId == Guid.Empty)
                return BadRequest("MultiTenantId obbligatorio.");

            var results = await _context.C_ANA_Companies
                .Where(c =>
                    (c.IsDeleted == false || c.IsDeleted == null) &&
                    c.MultiTenantId == multitenantId &&
                    c.isAffiliate == true)
                .ToListAsync();

            return Ok(results);
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
                customer.isTenant = false;

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
                existingCustomer.Cognome = updatedCustomer.Cognome;
                existingCustomer.Nome = updatedCustomer.Nome;
                existingCustomer.PIva = updatedCustomer.PIva;
                existingCustomer.FiscalCode = updatedCustomer.FiscalCode;
                existingCustomer.Email = updatedCustomer.Email;
                existingCustomer.Telefono = updatedCustomer.Telefono;
                existingCustomer.isCustomer = updatedCustomer.isCustomer;
                existingCustomer.UpdatedAt = DateTime.UtcNow;
                existingCustomer.Tipologia = updatedCustomer.Tipologia;
                existingCustomer.Indirizzo = updatedCustomer.Indirizzo;
                existingCustomer.Cap = updatedCustomer.Cap;
                existingCustomer.Regione = updatedCustomer.Regione;
                existingCustomer.Provincia = updatedCustomer.Provincia;
                existingCustomer.Citta = updatedCustomer.Citta;
                existingCustomer.Telefono = updatedCustomer.Telefono;
                existingCustomer.Email = updatedCustomer.Email;
                existingCustomer.FiscalCode = updatedCustomer.FiscalCode;
                existingCustomer.PIva = updatedCustomer.PIva;
                existingCustomer.EmailPec = updatedCustomer.EmailPec;
                existingCustomer.CodiceSdi = updatedCustomer.CodiceSdi;
                existingCustomer.IBAN = updatedCustomer.IBAN;
                existingCustomer.MultiTenantId = updatedCustomer.MultiTenantId;

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

        [HttpGet("search")]
        public async Task<IActionResult> Search(
         [FromQuery] string query,
         [FromQuery] Guid? multitenantId,
         [FromQuery] bool? isAffiliate)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Il parametro di ricerca è obbligatorio.");

            var results = await _context.C_ANA_Companies
                .Where(c =>
                    (c.IsDeleted == false || c.IsDeleted == null) &&
                    (!multitenantId.HasValue || c.MultiTenantId == multitenantId.Value) &&
                    (!isAffiliate.HasValue || c.isAffiliate == isAffiliate.Value) && // ✔️ condizione opzionale
                    (
                        c.RagioneSociale.Contains(query) ||
                        c.PIva.Contains(query) ||
                        c.Telefono.Contains(query) ||
                        c.Citta.Contains(query)
                    ))
                .Take(50)
                .ToListAsync();

            return Ok(results);
        }
    }
}