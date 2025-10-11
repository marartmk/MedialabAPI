using System;
using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.Repair
{
    /// <summary>
    /// DTO per la creazione di una nota di riparazione veloce
    /// </summary>
    public class CreateQuickRepairNoteDto
    {
        /// <summary>
        /// ID del dispositivo (opzionale - solo se selezionato dalla ricerca)
        /// </summary>
        public Guid? DeviceId { get; set; }

        /// <summary>
        /// ID del cliente (opzionale - solo se selezionato dalla ricerca)
        /// </summary>
        public Guid? CustomerId { get; set; }

        [Required(ErrorMessage = "CompanyId è obbligatorio")]
        public Guid CompanyId { get; set; }

        [Required(ErrorMessage = "MultitenantId è obbligatorio")]
        public Guid MultitenantId { get; set; }

        /// <summary>
        /// Brand del dispositivo
        /// </summary>
        [Required(ErrorMessage = "Il brand è obbligatorio")]
        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        /// <summary>
        /// Modello del dispositivo
        /// </summary>
        [Required(ErrorMessage = "Il modello è obbligatorio")]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// Ragione sociale (opzionale)
        /// </summary>
        [MaxLength(200)]
        public string? RagioneSociale { get; set; }

        /// <summary>
        /// Cognome
        /// </summary>
        [MaxLength(100)]
        public string? Cognome { get; set; }

        /// <summary>
        /// Nome
        /// </summary>
        [MaxLength(100)]
        public string? Nome { get; set; }

        /// <summary>
        /// Telefono
        /// </summary>
        [MaxLength(50)]
        public string? Telefono { get; set; }

        /// <summary>
        /// Codice di sblocco
        /// </summary>
        [MaxLength(50)]
        public string? CodiceRiparazione { get; set; }

        /// <summary>
        /// Descrizione del problema
        /// </summary>
        [Required(ErrorMessage = "La descrizione del problema è obbligatoria")]
        [MaxLength(1000)]
        public string Problema { get; set; } = string.Empty;

        /// <summary>
        /// Prezzo preventivo
        /// </summary>
        [Required(ErrorMessage = "Il prezzo preventivo è obbligatorio")]
        [Range(0.01, 999999.99, ErrorMessage = "Il prezzo deve essere maggiore di 0")]
        public decimal PrezzoPreventivo { get; set; }

        /// <summary>
        /// Note aggiuntive
        /// </summary>
        [MaxLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Data e ora di ricezione
        /// </summary>
        [Required]
        public DateTime ReceivedAt { get; set; }

        /// <summary>
        /// Utente che crea la nota
        /// </summary>
        [MaxLength(100)]
        public string? CreatedBy { get; set; }
    }

    /// <summary>
    /// DTO per l'aggiornamento di una nota di riparazione veloce
    /// </summary>
    public class UpdateQuickRepairNoteDto
    {
        public Guid? CustomerId { get; set; }
        public Guid? DeviceId { get; set; }
        [MaxLength(100)]
        public string? Brand { get; set; }
        [MaxLength(100)]
        public string? Model { get; set; }
        [MaxLength(200)]
        public string? RagioneSociale { get; set; }
        [MaxLength(100)]
        public string? Cognome { get; set; }
        [MaxLength(100)]
        public string? Nome { get; set; }
        [MaxLength(50)]
        public string? Telefono { get; set; }
        [MaxLength(50)]
        public string? CodiceRiparazione { get; set; }
        [MaxLength(1000)]
        public string? Problema { get; set; }
        [Range(0.01, 999999.99, ErrorMessage = "Il prezzo deve essere maggiore di 0")]
        public decimal? PrezzoPreventivo { get; set; }
        [MaxLength(1000)]
        public string? Notes { get; set; }
        [MaxLength(50)]
        public string? Stato { get; set; }
        [MaxLength(20)]
        public string? StatoCode { get; set; }
        [MaxLength(100)]
        public string? UpdatedBy { get; set; }
    }

    /// <summary>
    /// DTO di risposta dopo la creazione
    /// </summary>
    public class QuickRepairNoteResponseDto
    {
        public int Id { get; set; }
        public Guid NoteId { get; set; }
        public string? NoteCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO dettaglio nota
    /// </summary>
    public class QuickRepairNoteDetailDto
    {
        public int Id { get; set; }
        public Guid NoteId { get; set; }
        public string? NoteCode { get; set; }
        public Guid? DeviceId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid MultitenantId { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string? RagioneSociale { get; set; }
        public string? Cognome { get; set; }
        public string? Nome { get; set; }
        public string? Telefono { get; set; }
        public string? CodiceRiparazione { get; set; }
        public string Problema { get; set; } = string.Empty;
        public decimal PrezzoPreventivo { get; set; }
        public string? Notes { get; set; }
        public string Stato { get; set; } = string.Empty;
        public string StatoCode { get; set; } = string.Empty;
        public DateTime ReceivedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        // Info dispositivo (se presente)
        public DeviceInfo? DeviceInfo { get; set; }

        // Info cliente (se presente)
        public CustomerInfo? CustomerInfo { get; set; }
    }

    public class DeviceInfo
    {
        public string SerialNumber { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
    }

    public class CustomerInfo
    {
        public string Email { get; set; } = string.Empty;
        public string Indirizzo { get; set; } = string.Empty;
        public string Citta { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO per ricerca/filtro
    /// </summary>
    public class QuickRepairNoteSearchDto
    {
        public Guid? MultitenantId { get; set; }
        public string? StatoCode { get; set; }
        public string? Stato { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SearchTerm { get; set; } // Per cercare per cognome, nome, telefono, note code
    }
}