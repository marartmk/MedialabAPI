namespace MediaLabAPI.DTOs.Booking
{
    public class BookingSearchRequestDto
    {
        // Ricerca testuale generale
        public string? SearchQuery { get; set; }

        // Filtri specifici
        public string? BookingCode { get; set; }
        public Guid? BookingGuid { get; set; }
        public Guid? CustomerId { get; set; }
        public string? StatusCode { get; set; }
        public string? TechnicianCode { get; set; }

        // Filtri temporali
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? ScheduledDate { get; set; }

        // Filtri dispositivo
        public string? DeviceType { get; set; }
        public string? DeviceBrand { get; set; }
        public string? DeviceModel { get; set; }

        // Filtri cliente
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }

        // Contesto tenant
        public Guid? MultitenantId { get; set; }

        // Paginazione
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        // Ordinamento
        public string SortBy { get; set; } = "ScheduledDate";
        public bool SortDescending { get; set; } = false;

        // Flag conversione
        public bool? IsConverted { get; set; }

        // Validazione
        public int GetValidPageSize() => Math.Min(Math.Max(PageSize, 1), 100);
        public int GetValidPage() => Math.Max(Page, 1);
    }
}