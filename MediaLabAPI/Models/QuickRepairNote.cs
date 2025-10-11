using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaLabAPI.Models
{
    /// <summary>
    /// Modello per le note di riparazione veloci
    /// </summary>
    [Table("QuickRepairNotes")]
    public class QuickRepairNote
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// ID univoco della nota (GUID)
        /// </summary>
        [Required]
        public Guid NoteId { get; set; }

        /// <summary>
        /// Codice univoco della nota (generato automaticamente)
        /// </summary>
        [MaxLength(50)]
        public string? NoteCode { get; set; }

        /// <summary>
        /// ID del dispositivo (nullable - valorizzato solo se selezionato dalla ricerca)
        /// </summary>
        public Guid? DeviceId { get; set; }

        /// <summary>
        /// ID del cliente (nullable - valorizzato solo se selezionato dalla ricerca)
        /// </summary>
        public Guid? CustomerId { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid MultitenantId { get; set; }

        /// <summary>
        /// Brand del dispositivo (da form o da device selezionato)
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        /// <summary>
        /// Modello del dispositivo (da form o da device selezionato)
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// Ragione sociale (da customer o form)
        /// </summary>
        [MaxLength(200)]
        public string? RagioneSociale { get; set; }

        /// <summary>
        /// Cognome (da customer o form)
        /// </summary>
        [MaxLength(100)]
        public string? Cognome { get; set; }

        /// <summary>
        /// Nome (da customer o form)
        /// </summary>
        [MaxLength(100)]
        public string? Nome { get; set; }

        /// <summary>
        /// Telefono del cliente
        /// </summary>
        [MaxLength(50)]
        public string? Telefono { get; set; }

        /// <summary>
        /// Codice di sblocco dispositivo
        /// </summary>
        [MaxLength(50)]
        public string? CodiceRiparazione { get; set; }

        /// <summary>
        /// Descrizione del problema/intervento
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string Problema { get; set; } = string.Empty;

        /// <summary>
        /// Prezzo preventivo IVA inclusa
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrezzoPreventivo { get; set; }

        /// <summary>
        /// Note aggiuntive
        /// </summary>
        [MaxLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Stato della nota (es: "Aperta", "In Lavorazione", "Completata", "Consegnata")
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Stato { get; set; } = "Aperta";

        /// <summary>
        /// Codice stato per gestione programmatica
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string StatoCode { get; set; } = "OPEN";

        /// <summary>
        /// Data e ora di ricezione
        /// </summary>
        [Required]
        public DateTime ReceivedAt { get; set; }

        /// <summary>
        /// Data di creazione
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Data ultimo aggiornamento
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Utente che ha creato la nota
        /// </summary>
        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Utente che ha aggiornato la nota
        /// </summary>
        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Flag eliminazione logica
        /// </summary>
        [Required]
        public bool IsDeleted { get; set; } = false;

        // Relazioni (navigation properties)
        [ForeignKey("DeviceId")]
        public virtual DeviceRegistry? Device { get; set; }

        [ForeignKey("CustomerId")]
        public virtual C_ANA_Company? Customer { get; set; }

        [ForeignKey("CompanyId")]
        public virtual C_ANA_Company? Company { get; set; }
    }
}