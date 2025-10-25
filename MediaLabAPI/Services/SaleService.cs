using MediaLabAPI.Data;
using MediaLabAPI.Models;
using MediaLabAPI.DTOs.Sale;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MediaLabAPI.Services
{
    /// <summary>
    /// Servizio per la gestione delle vendite
    /// </summary>
    public class SaleService : ISaleService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SaleService> _logger;

        public SaleService(AppDbContext context, ILogger<SaleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region CRUD Operations

        public async Task<CreateSaleResponseDto> CreateSaleAsync(CreateSaleRequestDto request)
        {
            try
            {
                // Validazioni
                if (string.IsNullOrEmpty(request.SaleType))
                    throw new ArgumentException("Il tipo di vendita è obbligatorio");

                if (request.TotalAmount <= 0)
                    throw new ArgumentException("L'importo totale deve essere maggiore di zero");

                // Genera codice vendita univoco
                var saleCode = await GenerateSaleCodeAsync();

                // Calcola remaining amount se non specificato
                var remainingAmount = request.RemainingAmount > 0
                    ? request.RemainingAmount
                    : request.TotalAmount - request.PaidAmount;

                // Crea l'entità DeviceSale
                var sale = new DeviceSale
                {
                    SaleId = Guid.NewGuid(),
                    SaleCode = saleCode,
                    SaleType = request.SaleType,
                    DeviceId = request.DeviceId,
                    DeviceRegistryId = request.DeviceRegistryId,
                    AccessoryId = request.AccessoryId,
                    Brand = request.Brand,
                    Model = request.Model,
                    SerialNumber = request.SerialNumber,
                    IMEI = request.IMEI,
                    CustomerId = request.CustomerId,
                    CompanyId = request.CompanyId,
                    MultitenantId = request.MultitenantId,
                    SalePrice = request.SalePrice,
                    OriginalPrice = request.OriginalPrice,
                    Discount = request.Discount,
                    VatRate = request.VatRate,
                    TotalAmount = request.TotalAmount,
                    PaymentType = request.PaymentType,
                    PaymentStatus = request.PaymentStatus,
                    PaidAmount = request.PaidAmount,
                    RemainingAmount = remainingAmount,
                    InstallmentsCount = request.InstallmentsCount,
                    InstallmentAmount = request.InstallmentAmount,
                    SaleStatus = request.SaleStatus,
                    SaleStatusCode = request.SaleStatusCode,
                    InvoiceId = request.InvoiceId,
                    InvoiceNumber = request.InvoiceNumber,
                    InvoiceDate = request.InvoiceDate,
                    ReceiptId = request.ReceiptId,
                    ReceiptNumber = request.ReceiptNumber,
                    ReceiptDate = request.ReceiptDate,
                    SellerCode = request.SellerCode,
                    SellerName = request.SellerName,
                    Notes = request.Notes,
                    IncludedAccessories = request.IncludedAccessories,
                    HasWarranty = request.HasWarranty,
                    WarrantyMonths = request.WarrantyMonths,
                    WarrantyExpiryDate = request.WarrantyExpiryDate,
                    SaleDate = request.SaleDate ?? DateTime.UtcNow,
                    DeliveryDate = request.DeliveryDate,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy,
                    IsDeleted = false
                };

                _context.DeviceSales.Add(sale);
                await _context.SaveChangesAsync();

                var cnn = _context.Database.GetDbConnection();
                var affected = await _context.SaveChangesAsync();
                _logger.LogWarning("DB={Db} on {Srv} | affected={A} | NewId={Id}", cnn.Database, cnn.DataSource, affected, sale.Id);


                _logger.LogInformation("Vendita creata con successo: {SaleCode}", saleCode);

                return new CreateSaleResponseDto
                {
                    Id = sale.Id,
                    SaleId = sale.SaleId,
                    SaleCode = sale.SaleCode,
                    Message = "Vendita creata con successo",
                    CreatedAt = sale.CreatedAt,
                    Brand = sale.Brand,
                    Model = sale.Model,
                    TotalAmount = sale.TotalAmount,
                    SaleStatus = sale.SaleStatus,
                    PaymentStatus = sale.PaymentStatus
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione della vendita");
                throw;
            }
        }

        public async Task<SaleDetailDto?> GetSaleByIdAsync(int id)
        {
            var sale = await _context.DeviceSales
                .Include(s => s.Payments.Where(p => !p.IsDeleted))
                .Where(s => s.Id == id && !s.IsDeleted)
                .FirstOrDefaultAsync();

            return sale != null ? await MapToDetailDtoAsync(sale) : null;
        }

        public async Task<SaleDetailDto?> GetSaleBySaleIdAsync(Guid saleId)
        {
            var sale = await _context.DeviceSales
                .Include(s => s.Payments.Where(p => !p.IsDeleted))
                .Where(s => s.SaleId == saleId && !s.IsDeleted)
                .FirstOrDefaultAsync();

            return sale != null ? await MapToDetailDtoAsync(sale) : null;
        }

        public async Task<SaleDetailDto?> GetSaleBySaleCodeAsync(string saleCode)
        {
            var sale = await _context.DeviceSales
                .Include(s => s.Payments.Where(p => !p.IsDeleted))
                .Where(s => s.SaleCode == saleCode && !s.IsDeleted)
                .FirstOrDefaultAsync();

            return sale != null ? await MapToDetailDtoAsync(sale) : null;
        }

        public async Task<IEnumerable<SaleDetailDto>> SearchSalesAsync(SaleSearchRequestDto searchRequest)
        {
            var query = _context.DeviceSales
                .Include(s => s.Payments.Where(p => !p.IsDeleted))
                .Where(s => !s.IsDeleted);

            // Ricerca generale
            if (!string.IsNullOrWhiteSpace(searchRequest.SearchQuery))
            {
                var searchLower = searchRequest.SearchQuery.ToLower();
                query = query.Where(s =>
                    s.SaleCode.ToLower().Contains(searchLower) ||
                    s.Brand.ToLower().Contains(searchLower) ||
                    s.Model.ToLower().Contains(searchLower) ||
                    (s.SerialNumber != null && s.SerialNumber.ToLower().Contains(searchLower)) ||
                    (s.IMEI != null && s.IMEI.ToLower().Contains(searchLower))
                );
            }

            // Filtri specifici
            if (!string.IsNullOrWhiteSpace(searchRequest.SaleCode))
                query = query.Where(s => s.SaleCode.Contains(searchRequest.SaleCode));

            if (searchRequest.SaleGuid.HasValue)
                query = query.Where(s => s.SaleId == searchRequest.SaleGuid.Value);

            if (!string.IsNullOrWhiteSpace(searchRequest.SaleType))
                query = query.Where(s => s.SaleType == searchRequest.SaleType);

            if (searchRequest.MultitenantId.HasValue)
                query = query.Where(s => s.MultitenantId == searchRequest.MultitenantId.Value);

            if (searchRequest.CompanyId.HasValue)
                query = query.Where(s => s.CompanyId == searchRequest.CompanyId.Value);

            if (searchRequest.CustomerId.HasValue)
                query = query.Where(s => s.CustomerId == searchRequest.CustomerId.Value);

            if (searchRequest.DeviceId.HasValue)
                query = query.Where(s => s.DeviceId == searchRequest.DeviceId.Value);

            if (!string.IsNullOrWhiteSpace(searchRequest.SaleStatus))
                query = query.Where(s => s.SaleStatus == searchRequest.SaleStatus);

            if (!string.IsNullOrWhiteSpace(searchRequest.SaleStatusCode))
                query = query.Where(s => s.SaleStatusCode == searchRequest.SaleStatusCode);

            if (!string.IsNullOrWhiteSpace(searchRequest.PaymentStatus))
                query = query.Where(s => s.PaymentStatus == searchRequest.PaymentStatus);

            if (!string.IsNullOrWhiteSpace(searchRequest.Brand))
                query = query.Where(s => s.Brand.Contains(searchRequest.Brand));

            if (!string.IsNullOrWhiteSpace(searchRequest.Model))
                query = query.Where(s => s.Model.Contains(searchRequest.Model));

            if (!string.IsNullOrWhiteSpace(searchRequest.IMEI))
                query = query.Where(s => s.IMEI != null && s.IMEI.Contains(searchRequest.IMEI));

            if (!string.IsNullOrWhiteSpace(searchRequest.SerialNumber))
                query = query.Where(s => s.SerialNumber != null && s.SerialNumber.Contains(searchRequest.SerialNumber));

            if (searchRequest.FromDate.HasValue)
                query = query.Where(s => s.SaleDate >= searchRequest.FromDate.Value);

            if (searchRequest.ToDate.HasValue)
                query = query.Where(s => s.SaleDate <= searchRequest.ToDate.Value);

            if (searchRequest.MinAmount.HasValue)
                query = query.Where(s => s.TotalAmount >= searchRequest.MinAmount.Value);

            if (searchRequest.MaxAmount.HasValue)
                query = query.Where(s => s.TotalAmount <= searchRequest.MaxAmount.Value);

            if (searchRequest.HasWarranty.HasValue)
                query = query.Where(s => s.HasWarranty == searchRequest.HasWarranty.Value);

            // Ordinamento
            query = searchRequest.SortBy?.ToLower() switch
            {
                "salecode" => searchRequest.SortDescending ? query.OrderByDescending(s => s.SaleCode) : query.OrderBy(s => s.SaleCode),
                "saledate" => searchRequest.SortDescending ? query.OrderByDescending(s => s.SaleDate) : query.OrderBy(s => s.SaleDate),
                "totalamount" => searchRequest.SortDescending ? query.OrderByDescending(s => s.TotalAmount) : query.OrderBy(s => s.TotalAmount),
                "brand" => searchRequest.SortDescending ? query.OrderByDescending(s => s.Brand) : query.OrderBy(s => s.Brand),
                "model" => searchRequest.SortDescending ? query.OrderByDescending(s => s.Model) : query.OrderBy(s => s.Model),
                _ => searchRequest.SortDescending ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt)
            };

            var sales = await query.ToListAsync();
            return await Task.WhenAll(sales.Select(MapToDetailDtoAsync));
        }

        public async Task<IEnumerable<SaleDetailDto>> GetSalesAsync(Guid? multitenantId, string? status, string? paymentStatus)
        {
            var query = _context.DeviceSales
                .Include(s => s.Payments.Where(p => !p.IsDeleted))
                .Where(s => !s.IsDeleted);

            if (multitenantId.HasValue)
                query = query.Where(s => s.MultitenantId == multitenantId.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(s => s.SaleStatusCode == status);

            if (!string.IsNullOrWhiteSpace(paymentStatus))
                query = query.Where(s => s.PaymentStatus == paymentStatus);

            var sales = await query
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return await Task.WhenAll(sales.Select(MapToDetailDtoAsync));
        }

        public async Task<IEnumerable<SaleDetailDto>> GetSalesByCustomerAsync(Guid customerId)
        {
            var sales = await _context.DeviceSales
                .Include(s => s.Payments.Where(p => !p.IsDeleted))
                .Where(s => s.CustomerId == customerId && !s.IsDeleted)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return await Task.WhenAll(sales.Select(MapToDetailDtoAsync));
        }

        public async Task<IEnumerable<SaleDetailDto>> GetSalesByDeviceAsync(Guid deviceId)
        {
            var sales = await _context.DeviceSales
                .Include(s => s.Payments.Where(p => !p.IsDeleted))
                .Where(s => s.DeviceId == deviceId && !s.IsDeleted)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return await Task.WhenAll(sales.Select(MapToDetailDtoAsync));
        }

        public async Task UpdateSaleAsync(Guid saleId, UpdateSaleRequestDto request)
        {
            var sale = await _context.DeviceSales
                .FirstOrDefaultAsync(s => s.SaleId == saleId && !s.IsDeleted);

            if (sale == null)
                throw new KeyNotFoundException($"Vendita con ID {saleId} non trovata");

            // Aggiorna solo i campi modificabili
            if (!string.IsNullOrEmpty(request.SaleType))
                sale.SaleType = request.SaleType;

            if (request.SalePrice.HasValue)
                sale.SalePrice = request.SalePrice.Value;

            if (request.OriginalPrice.HasValue)
                sale.OriginalPrice = request.OriginalPrice.Value;

            if (request.Discount.HasValue)
                sale.Discount = request.Discount.Value;

            if (request.VatRate.HasValue)
                sale.VatRate = request.VatRate.Value;

            if (request.TotalAmount.HasValue)
                sale.TotalAmount = request.TotalAmount.Value;

            if (!string.IsNullOrEmpty(request.PaymentType))
                sale.PaymentType = request.PaymentType;

            if (!string.IsNullOrEmpty(request.PaymentStatus))
                sale.PaymentStatus = request.PaymentStatus;

            if (request.PaidAmount.HasValue)
                sale.PaidAmount = request.PaidAmount.Value;

            if (request.RemainingAmount.HasValue)
                sale.RemainingAmount = request.RemainingAmount.Value;

            if (request.InstallmentsCount.HasValue)
                sale.InstallmentsCount = request.InstallmentsCount;

            if (request.InstallmentAmount.HasValue)
                sale.InstallmentAmount = request.InstallmentAmount.Value;

            if (!string.IsNullOrEmpty(request.Notes))
                sale.Notes = request.Notes;

            if (!string.IsNullOrEmpty(request.IncludedAccessories))
                sale.IncludedAccessories = request.IncludedAccessories;

            if (request.HasWarranty.HasValue)
                sale.HasWarranty = request.HasWarranty.Value;

            if (request.WarrantyMonths.HasValue)
                sale.WarrantyMonths = request.WarrantyMonths;

            if (request.WarrantyExpiryDate.HasValue)
                sale.WarrantyExpiryDate = request.WarrantyExpiryDate;

            if (request.SaleDate.HasValue)
                sale.SaleDate = request.SaleDate.Value;

            if (request.DeliveryDate.HasValue)
                sale.DeliveryDate = request.DeliveryDate;

            if (!string.IsNullOrEmpty(request.SellerCode))
                sale.SellerCode = request.SellerCode;

            if (!string.IsNullOrEmpty(request.SellerName))
                sale.SellerName = request.SellerName;

            sale.UpdatedAt = DateTime.UtcNow;
            sale.UpdatedBy = request.UpdatedBy;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Vendita {SaleCode} aggiornata con successo", sale.SaleCode);
        }

        public async Task UpdateSaleStatusAsync(Guid saleId, UpdateSaleStatusDto request)
        {
            var sale = await _context.DeviceSales
                .FirstOrDefaultAsync(s => s.SaleId == saleId && !s.IsDeleted);

            if (sale == null)
                throw new KeyNotFoundException($"Vendita con ID {saleId} non trovata");

            sale.SaleStatus = request.SaleStatus;
            sale.SaleStatusCode = request.SaleStatusCode;
            sale.UpdatedAt = DateTime.UtcNow;
            sale.UpdatedBy = request.UpdatedBy;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Stato vendita {SaleCode} aggiornato a {Status}", sale.SaleCode, request.SaleStatus);
        }

        public async Task DeleteSaleAsync(Guid saleId)
        {
            var sale = await _context.DeviceSales
                .FirstOrDefaultAsync(s => s.SaleId == saleId && !s.IsDeleted);

            if (sale == null)
                throw new KeyNotFoundException($"Vendita con ID {saleId} non trovata");

            sale.IsDeleted = true;
            sale.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Vendita {SaleCode} eliminata (soft delete)", sale.SaleCode);
        }

        #endregion

        #region Payment Management

        public async Task<AddSalePaymentResponseDto> AddPaymentAsync(Guid saleId, SalePaymentDto paymentDto)
        {
            var sale = await _context.DeviceSales
                .Include(s => s.Payments)
                .FirstOrDefaultAsync(s => s.SaleId == saleId && !s.IsDeleted);

            if (sale == null)
                throw new KeyNotFoundException($"Vendita con ID {saleId} non trovata");

            if (paymentDto.Amount <= 0)
                throw new ArgumentException("L'importo del pagamento deve essere maggiore di zero");

            if (sale.RemainingAmount < paymentDto.Amount)
                throw new ArgumentException($"L'importo del pagamento ({paymentDto.Amount}) supera il residuo da pagare ({sale.RemainingAmount})");

            var payment = new SalePayment
            {
                PaymentId = Guid.NewGuid(),
                SaleId = saleId,
                Amount = paymentDto.Amount,
                PaymentMethod = paymentDto.PaymentMethod,
                TransactionReference = paymentDto.TransactionReference,
                PaymentDate = paymentDto.PaymentDate ?? DateTime.UtcNow,
                Notes = paymentDto.Notes,
                ReceivedBy = paymentDto.ReceivedBy,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.SalePayments.Add(payment);

            // Aggiorna i totali della vendita
            sale.PaidAmount += paymentDto.Amount;
            sale.RemainingAmount -= paymentDto.Amount;

            // Aggiorna lo stato del pagamento
            if (sale.RemainingAmount <= 0)
            {
                sale.PaymentStatus = "Pagato";
                sale.RemainingAmount = 0; // Assicura che sia esattamente 0
            }
            else if (sale.PaidAmount > 0)
            {
                sale.PaymentStatus = "Parzialmente Pagato";
            }

            sale.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Pagamento di {Amount} aggiunto alla vendita {SaleCode}", paymentDto.Amount, sale.SaleCode);

            return new AddSalePaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                SaleId = saleId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                PaymentDate = payment.PaymentDate,
                NewPaidAmount = sale.PaidAmount,
                NewRemainingAmount = sale.RemainingAmount,
                NewPaymentStatus = sale.PaymentStatus,
                Message = "Pagamento registrato con successo"
            };
        }

        public async Task<IEnumerable<SalePaymentDetailDto>> GetSalePaymentsAsync(Guid saleId)
        {
            var payments = await _context.SalePayments
                .Where(p => p.SaleId == saleId && !p.IsDeleted)
                .OrderBy(p => p.PaymentDate)
                .ToListAsync();

            return payments.Select(p => new SalePaymentDetailDto
            {
                Id = p.Id,
                PaymentId = p.PaymentId,
                SaleId = p.SaleId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                TransactionReference = p.TransactionReference,
                PaymentDate = p.PaymentDate,
                Notes = p.Notes,
                ReceivedBy = p.ReceivedBy,
                CreatedAt = p.CreatedAt,
                IsDeleted = p.IsDeleted
            });
        }

        public async Task DeletePaymentAsync(Guid paymentId)
        {
            var payment = await _context.SalePayments
                .Include(p => p.Sale)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId && !p.IsDeleted);

            if (payment == null)
                throw new KeyNotFoundException($"Pagamento con ID {paymentId} non trovato");

            var sale = payment.Sale;

            // Aggiorna i totali della vendita
            sale.PaidAmount -= payment.Amount;
            sale.RemainingAmount += payment.Amount;

            // Aggiorna lo stato del pagamento
            if (sale.PaidAmount <= 0)
            {
                sale.PaymentStatus = "Da Pagare";
                sale.PaidAmount = 0; // Assicura che sia esattamente 0
            }
            else if (sale.RemainingAmount > 0)
            {
                sale.PaymentStatus = "Parzialmente Pagato";
            }

            // Soft delete del pagamento
            payment.IsDeleted = true;

            sale.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Pagamento {PaymentId} eliminato dalla vendita {SaleCode}", paymentId, sale.SaleCode);
        }

        #endregion

        #region Utilities

        public async Task<string> GenerateSaleCodeAsync()
        {
            var year = DateTime.UtcNow.Year;
            var lastSale = await _context.DeviceSales
                .Where(s => s.SaleCode.StartsWith($"SALE-{year}-"))
                .OrderByDescending(s => s.Id)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastSale != null)
            {
                var parts = lastSale.SaleCode.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"SALE-{year}-{nextNumber:D5}";
        }

        public decimal CalculateTotalAmount(decimal salePrice, decimal vatRate, decimal? discount = null)
        {
            var discountedPrice = discount.HasValue && discount.Value > 0
                ? salePrice - discount.Value
                : salePrice;

            var vatAmount = discountedPrice * (vatRate / 100);
            return discountedPrice + vatAmount;
        }

        public decimal CalculateInstallmentAmount(decimal totalAmount, int installmentsCount)
        {
            if (installmentsCount <= 0)
                throw new ArgumentException("Il numero di rate deve essere maggiore di zero");

            return Math.Round(totalAmount / installmentsCount, 2);
        }

        public async Task<object> GetSalesStatisticsAsync(Guid? multitenantId, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.DeviceSales.Where(s => !s.IsDeleted);

            if (multitenantId.HasValue)
                query = query.Where(s => s.MultitenantId == multitenantId.Value);

            if (fromDate.HasValue)
                query = query.Where(s => s.SaleDate.HasValue && s.SaleDate.Value >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(s => s.SaleDate.HasValue && s.SaleDate.Value <= toDate.Value);

            var sales = await query.ToListAsync();

            // Statistiche generali
            var totalSales = sales.Count;
            var totalRevenue = sales.Sum(s => s.TotalAmount);
            var totalPaid = sales.Sum(s => s.PaidAmount);
            var totalRemaining = sales.Sum(s => s.RemainingAmount);
            var averageOrderValue = totalSales > 0 ? totalRevenue / totalSales : 0;
            var averageDiscount = sales.Where(s => s.Discount.HasValue).Any()
                ? sales.Where(s => s.Discount.HasValue).Average(s => s.Discount!.Value)
                : 0;

            // Statistiche per stato vendita
            var completedSales = sales.Count(s => s.SaleStatusCode == "COMPLETED");
            var draftSales = sales.Count(s => s.SaleStatusCode == "DRAFT");
            var cancelledSales = sales.Count(s => s.SaleStatusCode == "CANCELLED");

            // Statistiche per stato pagamento
            var fullyPaidSales = sales.Count(s => s.PaymentStatus == "Pagato");
            var partiallyPaidSales = sales.Count(s => s.PaymentStatus == "Parzialmente Pagato");
            var unpaidSales = sales.Count(s => s.PaymentStatus == "Da Pagare");

            // Statistiche per tipo vendita
            var salesByType = sales.GroupBy(s => s.SaleType)
                .Select(g => new
                {
                    SaleType = g.Key,
                    Count = g.Count(),
                    Total = g.Sum(s => s.TotalAmount)
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            // Statistiche per brand (top 10)
            var salesByBrand = sales.GroupBy(s => s.Brand)
                .Select(g => new
                {
                    Brand = g.Key,
                    Count = g.Count(),
                    Total = g.Sum(s => s.TotalAmount)
                })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();

            // Statistiche per tipo di pagamento
            var salesByPaymentType = sales.GroupBy(s => s.PaymentType)
                .Select(g => new
                {
                    PaymentType = g.Key,
                    Count = g.Count(),
                    Total = g.Sum(s => s.TotalAmount)
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            // Statistiche per venditore
            var salesBySeller = sales.Where(s => !string.IsNullOrEmpty(s.SellerCode))
                .GroupBy(s => new { s.SellerCode, s.SellerName })
                .Select(g => new
                {
                    SellerCode = g.Key.SellerCode,
                    SellerName = g.Key.SellerName,
                    Count = g.Count(),
                    Total = g.Sum(s => s.TotalAmount)
                })
                .OrderByDescending(x => x.Total)
                .Take(10)
                .ToList();

            // Statistiche garanzia
            var salesWithWarranty = sales.Count(s => s.HasWarranty);
            var salesWithoutWarranty = sales.Count(s => !s.HasWarranty);

            // Statistiche documenti
            var salesWithInvoice = sales.Count(s => s.InvoiceId.HasValue || !string.IsNullOrEmpty(s.InvoiceNumber));
            var salesWithReceipt = sales.Count(s => s.ReceiptId.HasValue || !string.IsNullOrEmpty(s.ReceiptNumber));

            // Trend temporale (se specificato un range di date)
            var dailyTrend = fromDate.HasValue && toDate.HasValue
                ? sales.Where(s => s.SaleDate.HasValue)
                    .GroupBy(s => s.SaleDate!.Value.Date)
                    .Select(g => new
                    {
                        DateValue = g.Key,
                        Count = g.Count(),
                        Total = g.Sum(s => s.TotalAmount)
                    })
                    .OrderBy(x => x.DateValue)
                    .ToList()
                : null;

            // Crea l'oggetto statistiche
            var stats = new
            {
                // Statistiche generali
                TotalSales = totalSales,
                TotalRevenue = totalRevenue,
                TotalPaid = totalPaid,
                TotalRemaining = totalRemaining,
                AverageOrderValue = averageOrderValue,
                AverageDiscount = averageDiscount,

                // Statistiche per stato vendita
                CompletedSales = completedSales,
                DraftSales = draftSales,
                CancelledSales = cancelledSales,

                // Statistiche per stato pagamento
                FullyPaidSales = fullyPaidSales,
                PartiallyPaidSales = partiallyPaidSales,
                UnpaidSales = unpaidSales,

                // Statistiche per tipo vendita
                SalesByType = salesByType,

                // Statistiche per brand (top 10)
                SalesByBrand = salesByBrand,

                // Statistiche per tipo di pagamento
                SalesByPaymentType = salesByPaymentType,

                // Statistiche per venditore
                SalesBySeller = salesBySeller,

                // Statistiche garanzia
                SalesWithWarranty = salesWithWarranty,
                SalesWithoutWarranty = salesWithoutWarranty,

                // Statistiche documenti
                SalesWithInvoice = salesWithInvoice,
                SalesWithReceipt = salesWithReceipt,

                // Trend temporale
                DailyTrend = dailyTrend
            };

            return stats;
        }

        #endregion

        #region Helper Methods

        private async Task<SaleDetailDto> MapToDetailDtoAsync(DeviceSale sale)
        {
            // Qui potresti aggiungere logica per recuperare nomi di clienti/aziende
            // da altre tabelle se necessario

            return new SaleDetailDto
            {
                Id = sale.Id,
                SaleId = sale.SaleId,
                SaleCode = sale.SaleCode,
                SaleType = sale.SaleType,
                DeviceId = sale.DeviceId,
                DeviceRegistryId = sale.DeviceRegistryId,
                AccessoryId = sale.AccessoryId,
                Brand = sale.Brand,
                Model = sale.Model,
                SerialNumber = sale.SerialNumber,
                IMEI = sale.IMEI,
                CustomerId = sale.CustomerId,
                CustomerName = null, // TODO: recuperare da tabella clienti se necessario
                CompanyId = sale.CompanyId,
                CompanyName = null, // TODO: recuperare da tabella aziende se necessario
                MultitenantId = sale.MultitenantId,
                SalePrice = sale.SalePrice,
                OriginalPrice = sale.OriginalPrice,
                Discount = sale.Discount,
                VatRate = sale.VatRate,
                TotalAmount = sale.TotalAmount,
                PaymentType = sale.PaymentType,
                PaymentStatus = sale.PaymentStatus,
                PaidAmount = sale.PaidAmount,
                RemainingAmount = sale.RemainingAmount,
                InstallmentsCount = sale.InstallmentsCount,
                InstallmentAmount = sale.InstallmentAmount,
                SaleStatus = sale.SaleStatus,
                SaleStatusCode = sale.SaleStatusCode,
                InvoiceId = sale.InvoiceId,
                InvoiceNumber = sale.InvoiceNumber,
                InvoiceDate = sale.InvoiceDate,
                ReceiptId = sale.ReceiptId,
                ReceiptNumber = sale.ReceiptNumber,
                ReceiptDate = sale.ReceiptDate,
                SellerCode = sale.SellerCode,
                SellerName = sale.SellerName,
                Notes = sale.Notes,
                IncludedAccessories = sale.IncludedAccessories,
                HasWarranty = sale.HasWarranty,
                WarrantyMonths = sale.WarrantyMonths,
                WarrantyExpiryDate = sale.WarrantyExpiryDate,
                CreatedAt = sale.CreatedAt,
                SaleDate = sale.SaleDate,
                DeliveryDate = sale.DeliveryDate,
                UpdatedAt = sale.UpdatedAt,
                CreatedBy = sale.CreatedBy,
                UpdatedBy = sale.UpdatedBy,
                Payments = sale.Payments?.Select(p => new SalePaymentDetailDto
                {
                    Id = p.Id,
                    PaymentId = p.PaymentId,
                    SaleId = sale.SaleId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    TransactionReference = p.TransactionReference,
                    PaymentDate = p.PaymentDate,
                    Notes = p.Notes,
                    ReceivedBy = p.ReceivedBy,
                    CreatedAt = p.CreatedAt,
                    IsDeleted = p.IsDeleted
                }).ToList()
            };
        }

        #endregion
    }
}