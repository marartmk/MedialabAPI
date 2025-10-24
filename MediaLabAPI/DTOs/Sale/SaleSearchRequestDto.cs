namespace MediaLabAPI.DTOs.Sale
{
    /// <summary>
    /// DTO per la ricerca e filtro delle vendite
    /// </summary>
    public class SaleSearchRequestDto
    {
        // ==================== RICERCA GENERALE ====================
        /// <summary>
        /// Ricerca testuale generale (cliente, dispositivo, codice vendita)
        /// </summary>
        public string? SearchQuery { get; set; }

        // ==================== FILTRI SPECIFICI ====================
        /// <summary>
        /// Ricerca per codice vendita
        /// </summary>
        public string? SaleCode { get; set; }

        /// <summary>
        /// Ricerca per GUID vendita
        /// </summary>
        public Guid? SaleGuid { get; set; }

        /// <summary>
        /// Filtro per tipo vendita: "Apparato", "Accessorio"
        /// </summary>
        public string? SaleType { get; set; }

        /// <summary>
        /// Filtro per cliente specifico
        /// </summary>
        public Guid? CustomerId { get; set; }

        /// <summary>
        /// Filtro per dispositivo specifico
        /// </summary>
        public Guid? DeviceId { get; set; }

        /// <summary>
        /// Filtro per stato vendita: "Bozza", "Completata", "Annullata"
        /// </summary>
        public string? SaleStatus { get; set; }

        /// <summary>
        /// Filtro per codice stato
        /// </summary>
        public string? SaleStatusCode { get; set; }

        /// <summary>
        /// Filtro per stato pagamento: "Pagato", "Parziale", "Da Pagare"
        /// </summary>
        public string? PaymentStatus { get; set; }

        /// <summary>
        /// Filtro per tipo pagamento
        /// </summary>
        public string? PaymentType { get; set; }

        /// <summary>
        /// Filtro per venditore
        /// </summary>
        public string? SellerCode { get; set; }

        // ==================== FILTRI TEMPORALI ====================
        /// <summary>
        /// Data vendita da
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Data vendita a
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Data creazione da
        /// </summary>
        public DateTime? CreatedFrom { get; set; }

        /// <summary>
        /// Data creazione a
        /// </summary>
        public DateTime? CreatedTo { get; set; }

        // ==================== FILTRI PRODOTTO ====================
        /// <summary>
        /// Filtro per brand
        /// </summary>
        public string? Brand { get; set; }

        /// <summary>
        /// Filtro per modello
        /// </summary>
        public string? Model { get; set; }

        /// <summary>
        /// Filtro per numero seriale
        /// </summary>
        public string? SerialNumber { get; set; }

        /// <summary>
        /// Filtro per IMEI
        /// </summary>
        public string? IMEI { get; set; }

        // ==================== FILTRI PREZZO ====================
        /// <summary>
        /// Prezzo minimo
        /// </summary>
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// Prezzo massimo
        /// </summary>
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// Importo totale minimo (con IVA)
        /// </summary>
        public decimal? MinAmount { get; set; }

        /// <summary>
        /// Importo totale massimo (con IVA)
        /// </summary>
        public decimal? MaxAmount { get; set; }

        // ==================== FILTRI DOCUMENTI ====================
        /// <summary>
        /// Filtro per vendite con fattura
        /// </summary>
        public bool? HasInvoice { get; set; }

        /// <summary>
        /// Filtro per vendite con ricevuta
        /// </summary>
        public bool? HasReceipt { get; set; }

        /// <summary>
        /// Numero fattura
        /// </summary>
        public string? InvoiceNumber { get; set; }

        // ==================== FILTRI GARANZIA ====================
        /// <summary>
        /// Filtro per vendite con garanzia
        /// </summary>
        public bool? HasWarranty { get; set; }

        // ==================== CONTESTO ====================
        /// <summary>
        /// Filtro per tenant
        /// </summary>
        public Guid? MultitenantId { get; set; }

        /// <summary>
        /// Filtro per azienda
        /// </summary>
        public Guid? CompanyId { get; set; }

        // ==================== PAGINAZIONE ====================
        /// <summary>
        /// Numero pagina (default: 1)
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Elementi per pagina (default: 20, max: 100)
        /// </summary>
        public int PageSize { get; set; } = 20;

        // ==================== ORDINAMENTO ====================
        /// <summary>
        /// Campo per ordinamento (default: CreatedAt)
        /// </summary>
        public string SortBy { get; set; } = "CreatedAt";

        /// <summary>
        /// Ordine discendente (default: true - più recenti prima)
        /// </summary>
        public bool SortDescending { get; set; } = true;

        // Metodi di validazione
        public int GetValidPageSize() => Math.Min(Math.Max(PageSize, 1), 100);
        public int GetValidPage() => Math.Max(Page, 1);
    }
}