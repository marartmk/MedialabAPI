using MediaLabAPI.Data;
using MediaLabAPI.DTOs.Repair;
using MediaLabAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaLabAPI.Services
{
    public class QuickRepairNoteService : IQuickRepairNoteService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<QuickRepairNoteService> _logger;

        public QuickRepairNoteService(AppDbContext context, ILogger<QuickRepairNoteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Crea una nuova nota di riparazione veloce
        /// </summary>
        public async Task<QuickRepairNoteResponseDto> CreateQuickNoteAsync(CreateQuickRepairNoteDto dto)
        {
            try
            {
                // Validazioni
                if (string.IsNullOrWhiteSpace(dto.Brand))
                    throw new ArgumentException("Il brand è obbligatorio");

                if (string.IsNullOrWhiteSpace(dto.Model))
                    throw new ArgumentException("Il modello è obbligatorio");

                if (string.IsNullOrWhiteSpace(dto.Problema))
                    throw new ArgumentException("La descrizione del problema è obbligatoria");

                if (dto.PrezzoPreventivo <= 0)
                    throw new ArgumentException("Il prezzo preventivo deve essere maggiore di 0");

                // Verifica se esiste il device (se fornito)
                if (dto.DeviceId.HasValue)
                {
                    var deviceExists = await _context.DeviceRegistry
                        .AnyAsync(d => !d.IsDeleted && d.DeviceId == dto.DeviceId.Value);

                    if (!deviceExists)
                        throw new ArgumentException("Dispositivo non trovato");
                }

                // Verifica se esiste il customer (se fornito)
                if (dto.CustomerId.HasValue)
                {
                    var customerExists = await _context.C_ANA_Companies
                        .AnyAsync(c => c.Id == dto.CustomerId.Value && c.IsDeleted != true);

                    if (!customerExists)
                        throw new ArgumentException("Cliente non trovato");
                }

                // Genera il NoteCode univoco
                var noteCode = await GenerateNoteCodeAsync(dto.MultitenantId);

                // Crea l'entità
                var note = new QuickRepairNote
                {
                    NoteId = Guid.NewGuid(),
                    NoteCode = noteCode,
                    DeviceId = dto.DeviceId,
                    CustomerId = dto.CustomerId,
                    CompanyId = dto.CompanyId,
                    MultitenantId = dto.MultitenantId,
                    Brand = dto.Brand,
                    Model = dto.Model,
                    RagioneSociale = dto.RagioneSociale,
                    Cognome = dto.Cognome,
                    Nome = dto.Nome,
                    Telefono = dto.Telefono,
                    CodiceRiparazione = dto.CodiceRiparazione,
                    Problema = dto.Problema,
                    PrezzoPreventivo = dto.PrezzoPreventivo,
                    Notes = dto.Notes,
                    Stato = "Aperta",
                    StatoCode = "OPEN",
                    ReceivedAt = dto.ReceivedAt,
                    CreatedAt = DateTime.Now,
                    CreatedBy = dto.CreatedBy,
                    IsDeleted = false
                };

                _context.QuickRepairNotes.Add(note);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Nota veloce creata: {NoteCode} per {Brand} {Model}",
                    noteCode, dto.Brand, dto.Model);

                return new QuickRepairNoteResponseDto
                {
                    Id = note.Id,
                    NoteId = note.NoteId,
                    NoteCode = note.NoteCode,
                    Message = "Nota di riparazione creata con successo",
                    CreatedAt = note.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella creazione della nota veloce");
                throw;
            }
        }

        /// <summary>
        /// Ottiene una nota per ID intero
        /// </summary>
        public async Task<QuickRepairNoteDetailDto?> GetQuickNoteByIdAsync(int id)
        {
            var note = await _context.QuickRepairNotes
                .Include(n => n.Device)
                .Include(n => n.Customer)
                .Where(n => n.Id == id && !n.IsDeleted)
                .FirstOrDefaultAsync();

            return note == null ? null : MapToDetailDto(note);
        }

        /// <summary>
        /// Ottiene una nota per NoteId (Guid)
        /// </summary>
        public async Task<QuickRepairNoteDetailDto?> GetQuickNoteByNoteIdAsync(Guid noteId)
        {
            var note = await _context.QuickRepairNotes
                .Include(n => n.Device)
                .Include(n => n.Customer)
                .Where(n => n.NoteId == noteId && !n.IsDeleted)
                .FirstOrDefaultAsync();

            return note == null ? null : MapToDetailDto(note);
        }

        /// <summary>
        /// Ricerca note con filtri
        /// </summary>
        public async Task<IEnumerable<QuickRepairNoteDetailDto>> SearchQuickNotesAsync(QuickRepairNoteSearchDto searchDto)
        {
            var query = _context.QuickRepairNotes
                .Include(n => n.Device)
                .Include(n => n.Customer)
                .Where(n => !n.IsDeleted)
                .AsQueryable();

            if (searchDto.MultitenantId.HasValue)
                query = query.Where(n => n.MultitenantId == searchDto.MultitenantId.Value);

            if (!string.IsNullOrWhiteSpace(searchDto.StatoCode))
                query = query.Where(n => n.StatoCode == searchDto.StatoCode);

            if (!string.IsNullOrWhiteSpace(searchDto.Stato))
                query = query.Where(n => n.Stato == searchDto.Stato);

            if (searchDto.FromDate.HasValue)
                query = query.Where(n => n.ReceivedAt >= searchDto.FromDate.Value);

            if (searchDto.ToDate.HasValue)
                query = query.Where(n => n.ReceivedAt <= searchDto.ToDate.Value);

            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                var term = searchDto.SearchTerm.ToLower();
                query = query.Where(n =>
                    (n.NoteCode != null && n.NoteCode.ToLower().Contains(term)) ||
                    (n.Cognome != null && n.Cognome.ToLower().Contains(term)) ||
                    (n.Nome != null && n.Nome.ToLower().Contains(term)) ||
                    (n.Telefono != null && n.Telefono.Contains(term)) ||
                    n.Brand.ToLower().Contains(term) ||
                    n.Model.ToLower().Contains(term)
                );
            }

            var notes = await query
                .OrderByDescending(n => n.ReceivedAt)
                .Take(100) // Limita a 100 risultati
                .ToListAsync();

            return notes.Select(MapToDetailDto);
        }

        /// <summary>
        /// Aggiorna una nota esistente
        /// </summary>
        public async Task UpdateQuickNoteAsync(Guid noteId, UpdateQuickRepairNoteDto dto)
        {
            var note = await _context.QuickRepairNotes
                .FirstOrDefaultAsync(n => n.NoteId == noteId && !n.IsDeleted);

            if (note == null)
                throw new ArgumentException("Nota non trovata");

            // Aggiorna solo i campi forniti
            if (!string.IsNullOrWhiteSpace(dto.Brand))
                note.Brand = dto.Brand;

            if (!string.IsNullOrWhiteSpace(dto.Model))
                note.Model = dto.Model;

            if (dto.RagioneSociale != null)
                note.RagioneSociale = dto.RagioneSociale;

            if (dto.Cognome != null)
                note.Cognome = dto.Cognome;

            if (dto.Nome != null)
                note.Nome = dto.Nome;

            if (dto.Telefono != null)
                note.Telefono = dto.Telefono;

            if (dto.CodiceRiparazione != null)
                note.CodiceRiparazione = dto.CodiceRiparazione;

            if (!string.IsNullOrWhiteSpace(dto.Problema))
                note.Problema = dto.Problema;

            if (dto.PrezzoPreventivo.HasValue && dto.PrezzoPreventivo.Value > 0)
                note.PrezzoPreventivo = dto.PrezzoPreventivo.Value;

            if (dto.Notes != null)
                note.Notes = dto.Notes;

            if (!string.IsNullOrWhiteSpace(dto.Stato))
                note.Stato = dto.Stato;

            if (!string.IsNullOrWhiteSpace(dto.StatoCode))
                note.StatoCode = dto.StatoCode;

            note.UpdatedAt = DateTime.Now;
            note.UpdatedBy = dto.UpdatedBy;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Nota {NoteCode} aggiornata", note.NoteCode);
        }

        /// <summary>
        /// Elimina una nota (soft delete)
        /// </summary>
        public async Task DeleteQuickNoteAsync(Guid noteId)
        {
            var note = await _context.QuickRepairNotes
                .FirstOrDefaultAsync(n => n.NoteId == noteId && !n.IsDeleted);

            if (note == null)
                throw new ArgumentException("Nota non trovata");

            note.IsDeleted = true;
            note.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Nota {NoteCode} eliminata", note.NoteCode);
        }

        // ============== HELPER METHODS ==============

        /// <summary>
        /// Genera un codice univoco per la nota (es: QN-2025-00001)
        /// </summary>
        private async Task<string> GenerateNoteCodeAsync(Guid multitenantId)
        {
            var year = DateTime.Now.Year;
            var prefix = $"QN-{year}-";

            // Trova l'ultimo codice dell'anno per questo tenant
            var lastNote = await _context.QuickRepairNotes
                .Where(n => n.MultitenantId == multitenantId &&
                           n.NoteCode != null &&
                           n.NoteCode.StartsWith(prefix))
                .OrderByDescending(n => n.NoteCode)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastNote != null && !string.IsNullOrEmpty(lastNote.NoteCode))
            {
                // Estrai il numero dall'ultimo codice
                var lastNumberStr = lastNote.NoteCode.Substring(prefix.Length);
                if (int.TryParse(lastNumberStr, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{prefix}{nextNumber:D5}"; // Formatta con 5 cifre (es: 00001)
        }

        /// <summary>
        /// Mappa l'entità al DTO dettaglio
        /// </summary>
        private QuickRepairNoteDetailDto MapToDetailDto(QuickRepairNote note)
        {
            var dto = new QuickRepairNoteDetailDto
            {
                Id = note.Id,
                NoteId = note.NoteId,
                NoteCode = note.NoteCode,
                DeviceId = note.DeviceId,
                CustomerId = note.CustomerId,
                CompanyId = note.CompanyId,
                MultitenantId = note.MultitenantId,
                Brand = note.Brand,
                Model = note.Model,
                RagioneSociale = note.RagioneSociale,
                Cognome = note.Cognome,
                Nome = note.Nome,
                Telefono = note.Telefono,
                CodiceRiparazione = note.CodiceRiparazione,
                Problema = note.Problema,
                PrezzoPreventivo = note.PrezzoPreventivo,
                Notes = note.Notes,
                Stato = note.Stato,
                StatoCode = note.StatoCode,
                ReceivedAt = note.ReceivedAt,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                CreatedBy = note.CreatedBy,
                UpdatedBy = note.UpdatedBy,
                IsDeleted = note.IsDeleted
            };

            // Aggiungi info dispositivo se presente
            if (note.Device != null)
            {
                dto.DeviceInfo = new DeviceInfo
                {
                    SerialNumber = note.Device.SerialNumber,
                    DeviceType = note.Device.DeviceType
                };
            }

            // Aggiungi info cliente se presente
            if (note.Customer != null)
            {
                dto.CustomerInfo = new CustomerInfo
                {
                    Email = note.Customer.Email ?? string.Empty,
                    Indirizzo = note.Customer.Indirizzo ?? string.Empty,
                    Citta = note.Customer.Citta ?? string.Empty
                };
            }

            return dto;
        }
    }
}