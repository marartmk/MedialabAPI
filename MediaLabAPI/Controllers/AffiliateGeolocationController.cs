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
    public class AffiliateGeolocationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AffiliateGeolocationController> _logger;

        public AffiliateGeolocationController(AppDbContext context, ILogger<AffiliateGeolocationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/affiliategeolocation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AffiliateGeolocationDto>>> GetAll([FromQuery] Guid? multitenantId)
        {
            try
            {
                var query = _context.AffiliateGeolocations
                    .Include(ag => ag.Affiliate)
                    .Where(ag => ag.IsActive);

                if (multitenantId.HasValue)
                {
                    query = query.Where(ag => ag.Affiliate!.MultiTenantId == multitenantId.Value);
                }

                var geolocations = await query
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
                    .ToListAsync();

                return Ok(geolocations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero delle geolocalizzazioni");
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

        // GET: api/affiliategeolocation/{affiliateId}
        [HttpGet("{affiliateId}")]
        public async Task<ActionResult<AffiliateGeolocationDto>> GetByAffiliateId(Guid affiliateId)
        {
            try
            {
                var geolocation = await _context.AffiliateGeolocations
                    .Where(ag => ag.AffiliateId == affiliateId && ag.IsActive)
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
                    return NotFound($"Geolocalizzazione per affiliato {affiliateId} non trovata");

                return Ok(geolocation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero della geolocalizzazione per affiliato {AffiliateId}", affiliateId);
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

        // POST: api/affiliategeolocation
        [HttpPost]
        public async Task<ActionResult<AffiliateGeolocationDto>> Create(CreateAffiliateGeolocationDto createDto)
        {
            try
            {
                // Verifica che l'affiliato esista
                var affiliateExists = await _context.C_ANA_Companies
                    .AnyAsync(c => c.Id == createDto.AffiliateId &&
                                  c.isAffiliate == true &&
                                  (c.IsDeleted == false || c.IsDeleted == null));

                if (!affiliateExists)
                    return BadRequest("Affiliato non trovato o non valido");

                // Verifica se esiste già una geolocalizzazione attiva
                var existingGeolocation = await _context.AffiliateGeolocations
                    .FirstOrDefaultAsync(ag => ag.AffiliateId == createDto.AffiliateId && ag.IsActive);

                if (existingGeolocation != null)
                    return Conflict("Geolocalizzazione già esistente per questo affiliato. Usa PUT per aggiornare.");

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

                return CreatedAtAction(nameof(GetByAffiliateId), new { affiliateId = geolocation.AffiliateId }, resultDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella creazione della geolocalizzazione");
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

        // PUT: api/affiliategeolocation/{affiliateId}
        [HttpPut("{affiliateId}")]
        public async Task<IActionResult> Update(Guid affiliateId, UpdateAffiliateGeolocationDto updateDto)
        {
            try
            {
                var geolocation = await _context.AffiliateGeolocations
                    .FirstOrDefaultAsync(ag => ag.AffiliateId == affiliateId && ag.IsActive);

                if (geolocation == null)
                    return NotFound($"Geolocalizzazione per affiliato {affiliateId} non trovata");

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
                _logger.LogError(ex, "Errore nell'aggiornamento della geolocalizzazione per affiliato {AffiliateId}", affiliateId);
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

        // DELETE: api/affiliategeolocation/{affiliateId}
        [HttpDelete("{affiliateId}")]
        public async Task<IActionResult> Delete(Guid affiliateId)
        {
            try
            {
                var geolocation = await _context.AffiliateGeolocations
                    .FirstOrDefaultAsync(ag => ag.AffiliateId == affiliateId && ag.IsActive);

                if (geolocation == null)
                    return NotFound($"Geolocalizzazione per affiliato {affiliateId} non trovata");

                // Soft delete
                geolocation.IsActive = false;
                geolocation.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella cancellazione della geolocalizzazione per affiliato {AffiliateId}", affiliateId);
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

        // POST: api/affiliategeolocation/check
        [HttpPost("check")]
        public async Task<ActionResult<BatchGeolocationCheckResultDto>> CheckGeolocations(BatchGeolocationCheckDto checkDto)
        {
            try
            {
                var result = new BatchGeolocationCheckResultDto
                {
                    TotalChecked = checkDto.AffiliateIds.Count
                };

                foreach (var affiliateId in checkDto.AffiliateIds)
                {
                    var geolocation = await _context.AffiliateGeolocations
                        .Where(ag => ag.AffiliateId == affiliateId && ag.IsActive)
                        .FirstOrDefaultAsync();

                    var status = new AffiliateGeolocationStatusDto
                    {
                        AffiliateId = affiliateId,
                        IsGeolocated = geolocation != null && geolocation.HasValidCoordinates,
                        Quality = geolocation?.Quality ?? "UNKNOWN",
                        NeedsUpdate = geolocation == null || !geolocation.HasValidCoordinates,
                        LastGeocodedDate = geolocation?.GeocodedDate
                    };

                    result.AffiliatesStatus.Add(status);

                    if (status.IsGeolocated)
                        result.AlreadyGeolocated++;
                    else
                        result.NeedGeolocation++;
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella verifica delle geolocalizzazioni");
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

        // PUT: api/affiliategeolocation/batch
        [HttpPut("batch")]
        public async Task<ActionResult<BatchGeolocationResultDto>> UpdateBatch(BatchGeolocationRequestDto batchDto)
        {
            var result = new BatchGeolocationResultDto
            {
                TotalProcessed = batchDto.Affiliates.Count
            };

            try
            {
                foreach (var affiliateGeo in batchDto.Affiliates)
                {
                    try
                    {
                        // Verifica che l'affiliato esista
                        var affiliateExists = await _context.C_ANA_Companies
                            .AnyAsync(c => c.Id == affiliateGeo.AffiliateId &&
                                          c.isAffiliate == true &&
                                          (c.IsDeleted == false || c.IsDeleted == null));

                        if (!affiliateExists)
                        {
                            result.Errors.Add($"Affiliato {affiliateGeo.AffiliateId} non trovato");
                            result.FailedCount++;
                            continue;
                        }

                        // Cerca geolocalizzazione esistente
                        var existingGeolocation = await _context.AffiliateGeolocations
                            .FirstOrDefaultAsync(ag => ag.AffiliateId == affiliateGeo.AffiliateId && ag.IsActive);

                        if (existingGeolocation != null)
                        {
                            // Aggiorna esistente
                            existingGeolocation.Latitude = affiliateGeo.Latitude;
                            existingGeolocation.Longitude = affiliateGeo.Longitude;
                            existingGeolocation.Address = affiliateGeo.Address;
                            existingGeolocation.Quality = affiliateGeo.Quality;
                            existingGeolocation.Notes = affiliateGeo.Notes;
                            existingGeolocation.GeocodedDate = DateTime.UtcNow;
                            existingGeolocation.UpdatedAt = DateTime.UtcNow;
                            existingGeolocation.GeocodingSource = "GoogleMaps";

                            result.UpdatedCount++;
                        }
                        else
                        {
                            // Crea nuovo
                            var newGeolocation = new AffiliateGeolocation
                            {
                                AffiliateId = affiliateGeo.AffiliateId,
                                Latitude = affiliateGeo.Latitude,
                                Longitude = affiliateGeo.Longitude,
                                Address = affiliateGeo.Address,
                                Quality = affiliateGeo.Quality,
                                Notes = affiliateGeo.Notes,
                                GeocodedDate = DateTime.UtcNow,
                                CreatedAt = DateTime.UtcNow,
                                IsActive = true,
                                GeocodingSource = "GoogleMaps"
                            };

                            _context.AffiliateGeolocations.Add(newGeolocation);
                            result.CreatedCount++;
                        }

                        result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Errore nell'elaborazione dell'affiliato {AffiliateId}", affiliateGeo.AffiliateId);
                        result.Errors.Add($"Errore per affiliato {affiliateGeo.AffiliateId}: {ex.Message}");
                        result.FailedCount++;
                    }
                }

                await _context.SaveChangesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nell'aggiornamento batch delle geolocalizzazioni");
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

        // GET: api/affiliategeolocation/region/{regionName}
        [HttpGet("region/{regionName}")]
        public async Task<ActionResult<IEnumerable<AffiliateGeolocationDto>>> GetByRegion(string regionName, [FromQuery] Guid? multitenantId)
        {
            try
            {
                var query = _context.AffiliateGeolocations
                    .Include(ag => ag.Affiliate)
                    .Where(ag => ag.IsActive &&
                                ag.Affiliate!.Regione == regionName &&
                                ag.HasValidCoordinates);

                if (multitenantId.HasValue)
                {
                    query = query.Where(ag => ag.Affiliate!.MultiTenantId == multitenantId.Value);
                }

                var geolocations = await query
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
                    .ToListAsync();

                return Ok(geolocations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero delle geolocalizzazioni per regione {RegionName}", regionName);
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }
    }
}