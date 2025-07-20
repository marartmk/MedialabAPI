namespace MediaLabAPI.DTOs
{
    public class SearchRepairDto
    {
        public string? Query { get; set; }                    // Ricerca generale
        public string? RepairCode { get; set; }               // Ricerca per codice
        public Guid? RepairGuid { get; set; }                 // Ricerca per GUID
        public Guid? CustomerId { get; set; }                 // Filtro cliente
        public string? Status { get; set; }                   // Filtro stato
        public DateTime? FromDate { get; set; }               // Filtro data da
        public DateTime? ToDate { get; set; }                 // Filtro data a
        public Guid? MultitenantId { get; set; }              // Filtro tenant
        public int Page { get; set; } = 1;                    // Paginazione
        public int PageSize { get; set; } = 20;               // Dimensione pagina
    }
}