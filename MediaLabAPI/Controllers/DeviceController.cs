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
    public class DeviceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DeviceController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Device
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceRegistry>>> GetAll([FromQuery] Guid? multitenantId)
        {
            try
            {
                var query = _context.DeviceRegistry
                    .Where(d => d.IsDeleted == false || d.IsDeleted == null);

                if (multitenantId.HasValue)
                {
                    query = query.Where(d => d.MultitenantId == multitenantId.Value);
                }

                var devices = await query.ToListAsync();
                return Ok(devices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

        // GET: api/Device/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceRegistry>> GetById(int id)
        {
            try
            {
                var device = await _context.DeviceRegistry
                    .Where(d => d.Id == id && (d.IsDeleted == false || d.IsDeleted == null))
                    .FirstOrDefaultAsync();

                if (device == null)
                {
                    return NotFound($"Device con ID {id} non trovato");
                }

                return Ok(device);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

        // GET: api/Device/customer/{customerId}
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<DeviceRegistry>>> GetDevicesByCustomer(Guid customerId)
        {
            try
            {
                var devices = await _context.DeviceRegistry
                    .Where(d => d.CustomerId == customerId && (d.IsDeleted == false || d.IsDeleted == null))
                    .ToListAsync();

                return Ok(devices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

        // GET: api/Device/company/{companyId}
        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<IEnumerable<DeviceRegistry>>> GetDevicesByCompany(Guid companyId)
        {
            try
            {
                var devices = await _context.DeviceRegistry
                    .Where(d => d.CompanyId == companyId && (d.IsDeleted == false || d.IsDeleted == null))
                    .ToListAsync();

                return Ok(devices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

        // GET: api/Device/deviceid/{deviceId}
        [HttpGet("deviceid/{deviceId}")]
        public async Task<ActionResult<DeviceRegistry>> GetDeviceByDeviceId(Guid deviceId)
        {
            try
            {
                var device = await _context.DeviceRegistry
                    .FirstOrDefaultAsync(d => d.DeviceId == deviceId && (d.IsDeleted == false || d.IsDeleted == null));

                if (device == null)
                {
                    return NotFound($"Device con DeviceId {deviceId} non trovato");
                }

                return Ok(device);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

        // GET: api/Device/serial/{serialNumber}
        [HttpGet("serial/{serialNumber}")]
        public async Task<ActionResult<DeviceRegistry>> GetDeviceBySerial(string serialNumber)
        {
            try
            {
                var device = await _context.DeviceRegistry
                    .FirstOrDefaultAsync(d => d.SerialNumber == serialNumber && (d.IsDeleted == false || d.IsDeleted == null));

                if (device == null)
                {
                    return NotFound($"Device con numero seriale {serialNumber} non trovato");
                }

                return Ok(device);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }

        // POST: api/Device
        [HttpPost]
        public async Task<ActionResult<DeviceRegistry>> Create(DeviceRegistry device)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verifica che il numero seriale non esista già
                var existingDevice = await _context.DeviceRegistry
                    .FirstOrDefaultAsync(d => d.SerialNumber == device.SerialNumber && (d.IsDeleted == false || d.IsDeleted == null));

                if (existingDevice != null)
                {
                    return Conflict("Un device con questo numero seriale esiste già.");
                }

                device.CreatedAt = DateTime.UtcNow;
                device.IsDeleted = false;

                _context.DeviceRegistry.Add(device);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = device.Id }, device);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante la creazione: {ex.Message}");
            }
        }

        // PUT: api/Device/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DeviceRegistry device)
        {
            try
            {
                if (id != device.Id)
                    return BadRequest("ID non corrispondente");

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingDevice = await _context.DeviceRegistry
                    .Where(d => d.Id == id && (d.IsDeleted == false || d.IsDeleted == null))
                    .FirstOrDefaultAsync();

                if (existingDevice == null)
                    return NotFound($"Device con ID {id} non trovato");

                // Verifica che il numero seriale non sia già utilizzato da un altro device
                var duplicateDevice = await _context.DeviceRegistry
                    .FirstOrDefaultAsync(d => d.SerialNumber == device.SerialNumber && d.Id != id && (d.IsDeleted == false || d.IsDeleted == null));

                if (duplicateDevice != null)
                {
                    return Conflict("Un device con questo numero seriale esiste già.");
                }

                // Aggiorna i campi modificabili
                existingDevice.DeviceId = device.DeviceId;
                existingDevice.CustomerId = device.CustomerId;
                existingDevice.CompanyId = device.CompanyId;
                existingDevice.MultitenantId = device.MultitenantId;
                existingDevice.SerialNumber = device.SerialNumber;
                existingDevice.Brand = device.Brand;
                existingDevice.Model = device.Model;
                existingDevice.DeviceType = device.DeviceType;
                existingDevice.PurchaseDate = device.PurchaseDate;
                existingDevice.ReceiptNumber = device.ReceiptNumber;
                existingDevice.Retailer = device.Retailer;
                existingDevice.Notes = device.Notes;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante l'aggiornamento: {ex.Message}");
            }
        }

        // DELETE: api/Device/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var device = await _context.DeviceRegistry
                    .Where(d => d.Id == id && (d.IsDeleted == false || d.IsDeleted == null))
                    .FirstOrDefaultAsync();

                if (device == null)
                    return NotFound($"Device con ID {id} non trovato");

                device.IsDeleted = true;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante la cancellazione: {ex.Message}");
            }
        }

        // DELETE: api/Device/{id}/permanent (Hard Delete - solo se necessario)
        [HttpDelete("{id}/permanent")]
        public async Task<IActionResult> DeleteDevicePermanent(int id)
        {
            try
            {
                var device = await _context.DeviceRegistry.FindAsync(id);

                if (device == null)
                {
                    return NotFound($"Device con ID {id} non trovato");
                }

                _context.DeviceRegistry.Remove(device);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante la cancellazione permanente: {ex.Message}");
            }
        }

        // PUT: api/Device/{id}/restore
        [HttpPut("{id}/restore")]
        public async Task<IActionResult> RestoreDevice(int id)
        {
            try
            {
                var device = await _context.DeviceRegistry
                    .FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted == true);

                if (device == null)
                {
                    return NotFound($"Device cancellato con ID {id} non trovato");
                }

                device.IsDeleted = false;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante il ripristino: {ex.Message}");
            }
        }

        // SEARCH: api/Device/search
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string query,
            [FromQuery] Guid? multitenantId,
            [FromQuery] string? deviceType,
            [FromQuery] Guid? deviceId)
        {
            if (string.IsNullOrWhiteSpace(query) && !deviceId.HasValue)
                return BadRequest("Il parametro di ricerca (query) o deviceId è obbligatorio.");

            try
            {
                var results = await _context.DeviceRegistry
                    .Where(d =>
                        (d.IsDeleted == false || d.IsDeleted == null) &&
                        (!multitenantId.HasValue || d.MultitenantId == multitenantId.Value) &&
                        (string.IsNullOrEmpty(deviceType) || d.DeviceType == deviceType) &&
                        (
                            // Se è fornito un deviceId specifico, cerca quello
                            (deviceId.HasValue && d.DeviceId == deviceId.Value) ||
                            // Altrimenti cerca nei campi testuali
                            (!deviceId.HasValue && !string.IsNullOrWhiteSpace(query) && (
                                d.Brand.Contains(query) ||
                                d.Model.Contains(query) ||
                                d.SerialNumber.Contains(query)
                            ))
                        ))
                    .Take(50)
                    .ToListAsync();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante la ricerca: {ex.Message}");
            }
        }
    }
}