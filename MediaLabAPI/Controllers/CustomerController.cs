using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediaLabAPI.Data;
using MediaLabAPI.Models;
using MediaLabAPI.DTOs;

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

                // 🔹 AGGIORNA TUTTI I CAMPI MODIFICABILI
                existingCustomer.RagioneSociale = updatedCustomer.RagioneSociale;
                existingCustomer.Cognome = updatedCustomer.Cognome;
                existingCustomer.Nome = updatedCustomer.Nome;
                existingCustomer.PIva = updatedCustomer.PIva;
                existingCustomer.FiscalCode = updatedCustomer.FiscalCode;
                existingCustomer.Email = updatedCustomer.Email;
                existingCustomer.Telefono = updatedCustomer.Telefono;
                existingCustomer.isCustomer = updatedCustomer.isCustomer;
                existingCustomer.isSupplier = updatedCustomer.isSupplier; // AGGIUNTO
                existingCustomer.Tipologia = updatedCustomer.Tipologia;
                existingCustomer.Indirizzo = updatedCustomer.Indirizzo;
                existingCustomer.Cap = updatedCustomer.Cap;
                existingCustomer.Regione = updatedCustomer.Regione;
                existingCustomer.Provincia = updatedCustomer.Provincia;
                existingCustomer.Citta = updatedCustomer.Citta;
                existingCustomer.EmailPec = updatedCustomer.EmailPec;
                existingCustomer.CodiceSdi = updatedCustomer.CodiceSdi;
                existingCustomer.IBAN = updatedCustomer.IBAN;
                existingCustomer.MultiTenantId = updatedCustomer.MultiTenantId;

                // 🆕 CAMPI AFFILIAZIONE - AGGIUNTI
                existingCustomer.isAffiliate = updatedCustomer.isAffiliate;
                existingCustomer.AffiliateCode = updatedCustomer.AffiliateCode;
                existingCustomer.AffiliatedDataStart = updatedCustomer.AffiliatedDataStart;
                existingCustomer.AffiliatedDataEnd = updatedCustomer.AffiliatedDataEnd;
                existingCustomer.AffiliateStatus = updatedCustomer.AffiliateStatus;

                // 🔹 TIMESTAMP AGGIORNAMENTO
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

        // Aggiungi questi metodi al CustomerController esistente

        // GET: api/customer/customeraffiliated/with-geolocation       
        [HttpGet("customeraffiliated/with-geolocation")]
        public async Task<IActionResult> GetAffiliatedCustomersWithGeolocation([FromQuery] Guid multitenantId)
        {
            if (multitenantId == Guid.Empty)
                return BadRequest("MultiTenantId obbligatorio.");

            try
            {
                var results = await _context.C_ANA_Companies
                    .Where(c =>
                        (c.IsDeleted == false || c.IsDeleted == null) &&
                        c.MultiTenantId == multitenantId &&
                        c.isAffiliate == true)
                    .GroupJoin(
                        _context.AffiliateGeolocations.Where(ag => ag.IsActive),
                        customer => customer.Id,
                        geolocation => geolocation.AffiliateId,
                        (customer, geolocations) => new
                        {
                            // Dati base dell'affiliato
                            id = customer.Id.ToString(),
                            ragioneSociale = customer.RagioneSociale,
                            nome = customer.Nome,
                            cognome = customer.Cognome,
                            citta = customer.Citta,
                            provincia = customer.Provincia,
                            regione = customer.Regione,
                            telefono = customer.Telefono,
                            emailAziendale = customer.Email,
                            pIva = customer.PIva,
                            indirizzo = customer.Indirizzo,
                            cap = customer.Cap,

                            // Dati di geolocalizzazione - CORRETTO: usa le colonne del DB
                            lat = geolocations.FirstOrDefault() != null ? (double?)geolocations.FirstOrDefault()!.Latitude : null,
                            lng = geolocations.FirstOrDefault() != null ? (double?)geolocations.FirstOrDefault()!.Longitude : null,
                            geocoded = geolocations.Any(g => g.Latitude != null && g.Longitude != null && g.Latitude != 0 && g.Longitude != 0), // ✅ Usa le colonne DB
                            fromCache = geolocations.Any(),
                            geocodingQuality = geolocations.FirstOrDefault() != null ? geolocations.FirstOrDefault()!.Quality : null,
                            geocodingSource = geolocations.FirstOrDefault() != null ? geolocations.FirstOrDefault()!.GeocodingSource : null,
                            lastGeocodedDate = geolocations.FirstOrDefault() != null ? geolocations.FirstOrDefault()!.GeocodedDate : (DateTime?)null
                        })
                    .ToListAsync();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }
        // GET: api/customer/{id}/geolocation
        [HttpGet("{id}/geolocation")]
        public async Task<ActionResult<AffiliateGeolocationDto>> GetAffiliateGeolocation(Guid id)
        {
            try
            {
                // Verifica che sia un affiliato
                var affiliate = await _context.C_ANA_Companies
                    .FirstOrDefaultAsync(c => c.Id == id &&
                                            c.isAffiliate == true &&
                                            (c.IsDeleted == false || c.IsDeleted == null));

                if (affiliate == null)
                    return NotFound("Affiliato non trovato");

                var geolocation = await _context.AffiliateGeolocations
                    .Where(ag => ag.AffiliateId == id && ag.IsActive)
                    .Select(ag => new AffiliateGeolocationDto
                    {
                        Id = ag.Id,
                        AffiliateId = ag.AffiliateId,
                        Latitude = ag.Latitude,
                        Longitude = ag.Longitude,
                        Address = ag.Address,
                        GeocodedDate = ag.GeocodedDate,
                        Quality = ag.Quality,
                        Notes = ag.Notes,
                        IsActive = ag.IsActive,
                        GeocodingSource = ag.GeocodingSource,
                        HasValidCoordinates = ag.HasValidCoordinates,
                        CreatedAt = ag.CreatedAt,
                        UpdatedAt = ag.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (geolocation == null)
                    return NotFound("Geolocalizzazione non trovata per questo affiliato");

                return Ok(geolocation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

        // POST: api/customer/{id}/geolocation
        [HttpPost("{id}/geolocation")]
        public async Task<ActionResult<AffiliateGeolocationDto>> CreateAffiliateGeolocation(Guid id, CreateAffiliateGeolocationDto createDto)
        {
            try
            {
                // Verifica che sia un affiliato
                var affiliate = await _context.C_ANA_Companies
                    .FirstOrDefaultAsync(c => c.Id == id &&
                                            c.isAffiliate == true &&
                                            (c.IsDeleted == false || c.IsDeleted == null));

                if (affiliate == null)
                    return NotFound("Affiliato non trovato");

                // Forza l'ID dell'affiliato dal path
                createDto.AffiliateId = id;

                // Verifica se esiste già
                var existingGeolocation = await _context.AffiliateGeolocations
                    .FirstOrDefaultAsync(ag => ag.AffiliateId == id && ag.IsActive);

                if (existingGeolocation != null)
                    return Conflict("Geolocalizzazione già esistente per questo affiliato");

                // Costruisci l'indirizzo se non fornito
                if (string.IsNullOrWhiteSpace(createDto.Address) && affiliate != null)
                {
                    createDto.Address = $"{affiliate.Indirizzo}, {affiliate.Cap} {affiliate.Citta}, {affiliate.Provincia}, Italy";
                }

                var geolocation = new AffiliateGeolocation
                {
                    AffiliateId = createDto.AffiliateId,
                    Latitude = createDto.Latitude,
                    Longitude = createDto.Longitude,
                    Address = createDto.Address,
                    GeocodedDate = DateTime.UtcNow,
                    Quality = createDto.Quality ?? "UNKNOWN",
                    Notes = createDto.Notes,
                    IsActive = true,
                    GeocodingSource = createDto.GeocodingSource ?? "Manual",
                    CreatedAt = DateTime.UtcNow
                };

                _context.AffiliateGeolocations.Add(geolocation);
                await _context.SaveChangesAsync();

                var resultDto = new AffiliateGeolocationDto
                {
                    Id = geolocation.Id,
                    AffiliateId = geolocation.AffiliateId,
                    Latitude = geolocation.Latitude,
                    Longitude = geolocation.Longitude,
                    Address = geolocation.Address,
                    GeocodedDate = geolocation.GeocodedDate,
                    Quality = geolocation.Quality,
                    Notes = geolocation.Notes,
                    IsActive = geolocation.IsActive,
                    GeocodingSource = geolocation.GeocodingSource,
                    HasValidCoordinates = geolocation.HasValidCoordinates,
                    CreatedAt = geolocation.CreatedAt,
                    UpdatedAt = geolocation.UpdatedAt
                };

                return CreatedAtAction(nameof(GetAffiliateGeolocation), new { id = geolocation.AffiliateId }, resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

        // PUT: api/customer/{id}/geolocation
        [HttpPut("{id}/geolocation")]
        public async Task<IActionResult> UpdateAffiliateGeolocation(Guid id, UpdateAffiliateGeolocationDto updateDto)
        {
            try
            {
                // Verifica che sia un affiliato
                var affiliate = await _context.C_ANA_Companies
                    .FirstOrDefaultAsync(c => c.Id == id &&
                                            c.isAffiliate == true &&
                                            (c.IsDeleted == false || c.IsDeleted == null));

                if (affiliate == null)
                    return NotFound("Affiliato non trovato");

                var geolocation = await _context.AffiliateGeolocations
                    .FirstOrDefaultAsync(ag => ag.AffiliateId == id && ag.IsActive);

                if (geolocation == null)
                    return NotFound("Geolocalizzazione non trovata per questo affiliato");

                // Aggiorna solo i campi forniti
                if (updateDto.Latitude.HasValue)
                    geolocation.Latitude = updateDto.Latitude;

                if (updateDto.Longitude.HasValue)
                    geolocation.Longitude = updateDto.Longitude;

                if (!string.IsNullOrWhiteSpace(updateDto.Address))
                    geolocation.Address = updateDto.Address;

                if (!string.IsNullOrWhiteSpace(updateDto.Quality))
                    geolocation.Quality = updateDto.Quality;

                if (updateDto.Notes != null)
                    geolocation.Notes = updateDto.Notes;

                if (updateDto.IsActive.HasValue)
                    geolocation.IsActive = updateDto.IsActive.Value;

                if (!string.IsNullOrWhiteSpace(updateDto.GeocodingSource))
                    geolocation.GeocodingSource = updateDto.GeocodingSource;

                geolocation.UpdatedAt = DateTime.UtcNow;
                geolocation.GeocodedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

        // DELETE: api/customer/{id}/geolocation
        [HttpDelete("{id}/geolocation")]
        public async Task<IActionResult> DeleteAffiliateGeolocation(Guid id)
        {
            try
            {
                var geolocation = await _context.AffiliateGeolocations
                    .FirstOrDefaultAsync(ag => ag.AffiliateId == id && ag.IsActive);

                if (geolocation == null)
                    return NotFound("Geolocalizzazione non trovata per questo affiliato");

                // Soft delete
                geolocation.IsActive = false;
                geolocation.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }
    }
}