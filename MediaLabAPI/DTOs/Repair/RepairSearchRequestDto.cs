namespace MediaLabAPI.DTOs
{
    // 🔍 DTO per richiesta di ricerca ottimizzata per GridView
    public class RepairSearchRequestDto
    {
        // Ricerca testuale generale (ricerca in cliente, dispositivo, codice riparazione)
        public string? SearchQuery { get; set; }

        // Filtri specifici
        public string? RepairCode { get; set; }               // Ricerca esatta per codice
        public Guid? RepairGuid { get; set; }                 // Ricerca per GUID specifico
        public Guid? CustomerId { get; set; }                 // Filtro cliente specifico
        public string? StatusCode { get; set; }               // Filtro stato (es: "RECEIVED", "STARTED", etc)
        public string? TechnicianCode { get; set; }           // Filtro tecnico

        // Filtri temporali
        public DateTime? FromDate { get; set; }               // Data creazione da
        public DateTime? ToDate { get; set; }                 // Data creazione a

        // Filtri dispositivo
        public string? DeviceBrand { get; set; }              // Marca dispositivo
        public string? DeviceModel { get; set; }              // Modello dispositivo
        public string? SerialNumber { get; set; }             // Numero seriale

        // Contesto tenant
        public Guid? MultitenantId { get; set; }              // Filtro tenant

        // Paginazione
        public int Page { get; set; } = 1;                    // Pagina (default: 1)
        public int PageSize { get; set; } = 20;               // Elementi per pagina (default: 20, max: 100)

        // Ordinamento
        public string SortBy { get; set; } = "CreatedAt";     // Campo ordinamento
        public bool SortDescending { get; set; } = true;      // Ordine discendente (più recenti prima)

        // Validazione PageSize
        public int GetValidPageSize() => Math.Min(Math.Max(PageSize, 1), 100);
        public int GetValidPage() => Math.Max(Page, 1);
    }
}