using System;

namespace MediaLabAPI.DTOs.Purchase
{
    /// <summary>
    /// DTO per la ricerca e filtro degli acquisti
    /// </summary>
    public class PurchaseSearchRequestDto
    {
        // ==================== RICERCA GENERALE ====================
        /// <summary>
        /// Ricerca testuale generale (fornitore, dispositivo, codice acquisto)
        /// </summary>
        public string? SearchQuery { get; set; }

        // ==================== FILTRI SPECIFICI ====================
        /// <summary>
        /// Ricerca per codice acquisto
        /// </summary>
        public string? PurchaseCode { get; set; }

        /// <summary>
        /// Ricerca per GUID acquisto
        /// </summary>
        public Guid? PurchaseGuid { get; set; }

        /// <summary>
        /// Filtro per tipo acquisto: "Apparato", "Accessorio"
        /// </summary>
        public string? PurchaseType { get; set; }

        /// <summary>
        /// Filtro per condizione: "Nuovo", "Usato"
        /// </summary>
        public string? DeviceCondition { get; set; }

        /// <summary>
        /// Filtro per fornitore specifico
        /// </summary>
        public Guid? SupplierId { get; set; }

        /// <summary>
        /// Filtro per dispositivo specifico
        /// </summary>
        public Guid? DeviceId { get; set; }

        /// <summary>
        /// Filtro per stato acquisto: "Bozza", "Ordinato", "Ricevuto", "Annullato"
        /// </summary>
        public string? PurchaseStatus { get; set; }

        /// <summary>
        /// Filtro per codice stato
        /// </summary>
        public string? PurchaseStatusCode { get; set; }

        /// <summary>
        /// Filtro per stato pagamento: "Pagato", "Parziale", "Da Pagare"
        /// </summary>
        public string? PaymentStatus { get; set; }

        /// <summary>
        /// Filtro per tipo pagamento
        /// </summary>
        public string? PaymentType { get; set; }

        /// <summary>
        /// Filtro per acquirente/responsabile
        /// </summary>
        public string? BuyerCode { get; set; }

        /// <summary>
        /// Filtro per stato controllo qualità
        /// </summary>
        public string? QualityCheckStatus { get; set; }

        // ==================== FILTRI TEMPORALI ====================
        /// <summary>
        /// Data acquisto da
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Data acquisto a
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

        /// <summary>
        /// Data ricezione da
        /// </summary>
        public DateTime? ReceivedFrom { get; set; }

        /// <summary>
        /// Data ricezione a
        /// </summary>
        public DateTime? ReceivedTo { get; set; }

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
        /// Filtro per acquisti con fattura fornitore
        /// </summary>
        public bool? HasSupplierInvoice { get; set; }

        /// <summary>
        /// Numero fattura fornitore
        /// </summary>
        public string? SupplierInvoiceNumber { get; set; }

        /// <summary>
        /// Filtro per acquisti con DDT
        /// </summary>
        public bool? HasDDT { get; set; }

        /// <summary>
        /// Numero ordine
        /// </summary>
        public string? OrderNumber { get; set; }

        // ==================== FILTRI GARANZIA ====================
        /// <summary>
        /// Filtro per acquisti con garanzia fornitore
        /// </summary>
        public bool? HasSupplierWarranty { get; set; }

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
