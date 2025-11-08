using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.Booking
{
    /// <summary>
    /// DTO per la conversione di una prenotazione in riparazione
    /// </summary>
    public class ConvertBookingToRepairDto
    {
        [StringLength(1000)]
        public string? AdditionalNotes { get; set; }

        [StringLength(100)]
        public string? ConvertedBy { get; set; }

        // Opzionale: dati aggiuntivi per la riparazione
        public string? UnlockCode { get; set; }
        public string? CourtesyPhone { get; set; }
    }
}