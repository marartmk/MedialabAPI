using MediaLabAPI.DTOs.Sale;

namespace MediaLabAPI.Services
{
    /// <summary>
    /// Interfaccia per il servizio di gestione vendite
    /// </summary>
    public interface ISaleService
    {
        #region CRUD Operations

        /// <summary>
        /// Crea una nuova vendita
        /// </summary>
        Task<CreateSaleResponseDto> CreateSaleAsync(CreateSaleRequestDto request);

        /// <summary>
        /// Ottiene una vendita per ID numerico
        /// </summary>
        Task<SaleDetailDto?> GetSaleByIdAsync(int id);

        /// <summary>
        /// Ottiene una vendita per SaleId (Guid)
        /// </summary>
        Task<SaleDetailDto?> GetSaleBySaleIdAsync(Guid saleId);

        /// <summary>
        /// Ottiene una vendita per codice vendita
        /// </summary>
        Task<SaleDetailDto?> GetSaleBySaleCodeAsync(string saleCode);

        /// <summary>
        /// Cerca vendite con filtri avanzati
        /// </summary>
        Task<IEnumerable<SaleDetailDto>> SearchSalesAsync(SaleSearchRequestDto searchRequest);

        /// <summary>
        /// Ottiene tutte le vendite per un tenant con filtri opzionali
        /// </summary>
        Task<IEnumerable<SaleDetailDto>> GetSalesAsync(Guid? multitenantId, string? status, string? paymentStatus);

        /// <summary>
        /// Ottiene le vendite per cliente
        /// </summary>
        Task<IEnumerable<SaleDetailDto>> GetSalesByCustomerAsync(Guid customerId);

        /// <summary>
        /// Ottiene le vendite per dispositivo
        /// </summary>
        Task<IEnumerable<SaleDetailDto>> GetSalesByDeviceAsync(Guid deviceId);

        /// <summary>
        /// Aggiorna una vendita
        /// </summary>
        Task UpdateSaleAsync(Guid saleId, UpdateSaleRequestDto request);

        /// <summary>
        /// Aggiorna lo stato di una vendita
        /// </summary>
        Task UpdateSaleStatusAsync(Guid saleId, UpdateSaleStatusDto request);

        /// <summary>
        /// Elimina una vendita (soft delete)
        /// </summary>
        Task DeleteSaleAsync(Guid saleId);

        #endregion

        #region Payment Management

        /// <summary>
        /// Aggiunge un pagamento a una vendita
        /// </summary>
        Task<AddSalePaymentResponseDto> AddPaymentAsync(Guid saleId, SalePaymentDto paymentDto);

        /// <summary>
        /// Ottiene tutti i pagamenti di una vendita
        /// </summary>
        Task<IEnumerable<SalePaymentDetailDto>> GetSalePaymentsAsync(Guid saleId);

        /// <summary>
        /// Elimina un pagamento (soft delete)
        /// </summary>
        Task DeletePaymentAsync(Guid paymentId);

        #endregion

        #region Utilities

        /// <summary>
        /// Genera un nuovo codice vendita univoco
        /// </summary>
        Task<string> GenerateSaleCodeAsync();

        /// <summary>
        /// Calcola il totale con IVA
        /// </summary>
        decimal CalculateTotalAmount(decimal salePrice, decimal vatRate, decimal? discount = null);

        /// <summary>
        /// Calcola l'importo della rata
        /// </summary>
        decimal CalculateInstallmentAmount(decimal totalAmount, int installmentsCount);

        /// <summary>
        /// Ottiene statistiche vendite per un periodo
        /// </summary>
        Task<object> GetSalesStatisticsAsync(Guid? multitenantId, DateTime? fromDate, DateTime? toDate);

        #endregion
    }
}