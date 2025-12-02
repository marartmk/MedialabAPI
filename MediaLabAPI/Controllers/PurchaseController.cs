using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediaLabAPI.Data;
using MediaLabAPI.Models;
using MediaLabAPI.DTOs.Purchase;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MediaLabAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PurchaseController(AppDbContext context)
        {
            _context = context;
        }

        // ==================== CREATE ====================
        /// <summary>
        /// Crea un nuovo acquisto di apparato
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CreatePurchaseResponseDto>> CreatePurchase([FromBody] CreatePurchaseRequestDto request)
        {
            try
            {
                // Generazione automatica del codice acquisto
                var purchaseCode = await GeneratePurchaseCodeAsync(request.MultitenantId);

                var purchase = new DevicePurchase
                {
                    PurchaseId = Guid.NewGuid(),
                    PurchaseCode = purchaseCode,
                    PurchaseType = request.PurchaseType,
                    DeviceCondition = request.DeviceCondition,
                    DeviceId = request.DeviceId,
                    DeviceRegistryId = request.DeviceRegistryId,
                    AccessoryId = request.AccessoryId,
                    Brand = request.Brand,
                    Model = request.Model,
                    SerialNumber = request.SerialNumber,
                    IMEI = request.IMEI,
                    SupplierId = request.SupplierId,
                    CompanyId = request.CompanyId,
                    MultitenantId = request.MultitenantId,
                    PurchasePrice = request.PurchasePrice,
                    ShippingCost = request.ShippingCost,
                    OtherCosts = request.OtherCosts,
                    VatRate = request.VatRate,
                    TotalAmount = request.TotalAmount,
                    PaymentType = request.PaymentType,
                    PaymentStatus = request.PaymentStatus,
                    PaidAmount = request.PaidAmount,
                    RemainingAmount = request.RemainingAmount,
                    InstallmentsCount = request.InstallmentsCount,
                    InstallmentAmount = request.InstallmentAmount,
                    PurchaseStatus = request.PurchaseStatus,
                    PurchaseStatusCode = request.PurchaseStatusCode,
                    SupplierInvoiceId = request.SupplierInvoiceId,
                    SupplierInvoiceNumber = request.SupplierInvoiceNumber,
                    SupplierInvoiceDate = request.SupplierInvoiceDate,
                    OrderNumber = request.OrderNumber,
                    OrderDate = request.OrderDate,
                    DDTNumber = request.DDTNumber,
                    DDTDate = request.DDTDate,
                    BuyerCode = request.BuyerCode,
                    BuyerName = request.BuyerName,
                    Notes = request.Notes,
                    IncludedAccessories = request.IncludedAccessories,
                    QualityCheckStatus = request.QualityCheckStatus,
                    QualityCheckNotes = request.QualityCheckNotes,
                    QualityCheckDate = request.QualityCheckDate,
                    QualityCheckedBy = request.QualityCheckedBy,
                    HasSupplierWarranty = request.HasSupplierWarranty,
                    SupplierWarrantyMonths = request.SupplierWarrantyMonths,
                    SupplierWarrantyExpiryDate = request.SupplierWarrantyExpiryDate,
                    PurchaseDate = request.PurchaseDate ?? DateTime.UtcNow,
                    ReceivedDate = request.ReceivedDate,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy,
                    IsDeleted = false
                };

                _context.DevicePurchases.Add(purchase);
                await _context.SaveChangesAsync();

                var response = new CreatePurchaseResponseDto
                {
                    Id = purchase.Id,
                    PurchaseId = purchase.PurchaseId,
                    PurchaseCode = purchase.PurchaseCode,
                    CreatedAt = purchase.CreatedAt,
                    Brand = purchase.Brand,
                    Model = purchase.Model,
                    DeviceCondition = purchase.DeviceCondition,
                    TotalAmount = purchase.TotalAmount,
                    PurchaseStatus = purchase.PurchaseStatus,
                    PaymentStatus = purchase.PaymentStatus
                };

                return CreatedAtAction(nameof(GetPurchaseById), new { id = purchase.PurchaseId }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Errore durante la creazione dell'acquisto", error = ex.Message });
            }
        }

        // ==================== READ ====================
        /// <summary>
        /// Ottiene un acquisto per ID (GUID)
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PurchaseDetailDto>> GetPurchaseById(Guid id)
        {
            try
            {
                var purchase = await _context.DevicePurchases
                    .Include(p => p.Supplier)
                    .Include(p => p.Company)
                    .Include(p => p.Payments.Where(pay => !pay.IsDeleted))
                    .Where(p => p.PurchaseId == id && !p.IsDeleted)
                    .FirstOrDefaultAsync();

                if (purchase == null)
                {
                    return NotFound(new { message = "Acquisto non trovato" });
                }

                var dto = MapToPurchaseDetailDto(purchase);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Errore durante il recupero dell'acquisto", error = ex.Message });
            }
        }

        /// <summary>
        /// Cerca e filtra gli acquisti
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<object>> SearchPurchases([FromBody] PurchaseSearchRequestDto request)
        {
            try
            {
                var query = _context.DevicePurchases
                    .Include(p => p.Supplier)
                    .Include(p => p.Company)
                    .Where(p => !p.IsDeleted)
                    .AsQueryable();

                // Filtri
                if (request.MultitenantId.HasValue)
                    query = query.Where(p => p.MultitenantId == request.MultitenantId.Value);

                if (request.CompanyId.HasValue)
                    query = query.Where(p => p.CompanyId == request.CompanyId.Value);

                if (!string.IsNullOrWhiteSpace(request.SearchQuery))
                {
                    var searchLower = request.SearchQuery.ToLower();
                    query = query.Where(p =>
                        p.PurchaseCode!.ToLower().Contains(searchLower) ||
                        p.Brand.ToLower().Contains(searchLower) ||
                        p.Model.ToLower().Contains(searchLower) ||
                        (p.Supplier != null && p.Supplier.RagioneSociale.ToLower().Contains(searchLower)));
                }

                if (!string.IsNullOrWhiteSpace(request.PurchaseCode))
                    query = query.Where(p => p.PurchaseCode == request.PurchaseCode);

                if (request.PurchaseGuid.HasValue)
                    query = query.Where(p => p.PurchaseId == request.PurchaseGuid.Value);

                if (!string.IsNullOrWhiteSpace(request.PurchaseType))
                    query = query.Where(p => p.PurchaseType == request.PurchaseType);

                if (!string.IsNullOrWhiteSpace(request.DeviceCondition))
                    query = query.Where(p => p.DeviceCondition == request.DeviceCondition);

                if (request.SupplierId.HasValue)
                    query = query.Where(p => p.SupplierId == request.SupplierId.Value);

                if (request.DeviceId.HasValue)
                    query = query.Where(p => p.DeviceId == request.DeviceId.Value);

                if (!string.IsNullOrWhiteSpace(request.PurchaseStatus))
                    query = query.Where(p => p.PurchaseStatus == request.PurchaseStatus);

                if (!string.IsNullOrWhiteSpace(request.PurchaseStatusCode))
                    query = query.Where(p => p.PurchaseStatusCode == request.PurchaseStatusCode);

                if (!string.IsNullOrWhiteSpace(request.PaymentStatus))
                    query = query.Where(p => p.PaymentStatus == request.PaymentStatus);

                if (!string.IsNullOrWhiteSpace(request.PaymentType))
                    query = query.Where(p => p.PaymentType == request.PaymentType);

                if (!string.IsNullOrWhiteSpace(request.BuyerCode))
                    query = query.Where(p => p.BuyerCode == request.BuyerCode);

                if (!string.IsNullOrWhiteSpace(request.QualityCheckStatus))
                    query = query.Where(p => p.QualityCheckStatus == request.QualityCheckStatus);

                if (!string.IsNullOrWhiteSpace(request.Brand))
                    query = query.Where(p => p.Brand.Contains(request.Brand));

                if (!string.IsNullOrWhiteSpace(request.Model))
                    query = query.Where(p => p.Model.Contains(request.Model));

                if (!string.IsNullOrWhiteSpace(request.SerialNumber))
                    query = query.Where(p => p.SerialNumber == request.SerialNumber);

                if (!string.IsNullOrWhiteSpace(request.IMEI))
                    query = query.Where(p => p.IMEI == request.IMEI);

                // Filtri temporali
                if (request.FromDate.HasValue)
                    query = query.Where(p => p.PurchaseDate >= request.FromDate.Value);

                if (request.ToDate.HasValue)
                    query = query.Where(p => p.PurchaseDate <= request.ToDate.Value);

                if (request.CreatedFrom.HasValue)
                    query = query.Where(p => p.CreatedAt >= request.CreatedFrom.Value);

                if (request.CreatedTo.HasValue)
                    query = query.Where(p => p.CreatedAt <= request.CreatedTo.Value);

                if (request.ReceivedFrom.HasValue)
                    query = query.Where(p => p.ReceivedDate >= request.ReceivedFrom.Value);

                if (request.ReceivedTo.HasValue)
                    query = query.Where(p => p.ReceivedDate <= request.ReceivedTo.Value);

                // Filtri prezzo
                if (request.MinPrice.HasValue)
                    query = query.Where(p => p.PurchasePrice >= request.MinPrice.Value);

                if (request.MaxPrice.HasValue)
                    query = query.Where(p => p.PurchasePrice <= request.MaxPrice.Value);

                if (request.MinAmount.HasValue)
                    query = query.Where(p => p.TotalAmount >= request.MinAmount.Value);

                if (request.MaxAmount.HasValue)
                    query = query.Where(p => p.TotalAmount <= request.MaxAmount.Value);

                // Filtri documenti
                if (request.HasSupplierInvoice.HasValue)
                    query = query.Where(p => request.HasSupplierInvoice.Value ? p.SupplierInvoiceNumber != null : p.SupplierInvoiceNumber == null);

                if (!string.IsNullOrWhiteSpace(request.SupplierInvoiceNumber))
                    query = query.Where(p => p.SupplierInvoiceNumber == request.SupplierInvoiceNumber);

                if (request.HasDDT.HasValue)
                    query = query.Where(p => request.HasDDT.Value ? p.DDTNumber != null : p.DDTNumber == null);

                if (!string.IsNullOrWhiteSpace(request.OrderNumber))
                    query = query.Where(p => p.OrderNumber == request.OrderNumber);

                if (request.HasSupplierWarranty.HasValue)
                    query = query.Where(p => p.HasSupplierWarranty == request.HasSupplierWarranty.Value);

                // Ordinamento
                query = request.SortBy?.ToLower() switch
                {
                    "purchasecode" => request.SortDescending ? query.OrderByDescending(p => p.PurchaseCode) : query.OrderBy(p => p.PurchaseCode),
                    "brand" => request.SortDescending ? query.OrderByDescending(p => p.Brand) : query.OrderBy(p => p.Brand),
                    "model" => request.SortDescending ? query.OrderByDescending(p => p.Model) : query.OrderBy(p => p.Model),
                    "purchaseprice" => request.SortDescending ? query.OrderByDescending(p => p.PurchasePrice) : query.OrderBy(p => p.PurchasePrice),
                    "totalamount" => request.SortDescending ? query.OrderByDescending(p => p.TotalAmount) : query.OrderBy(p => p.TotalAmount),
                    "purchasedate" => request.SortDescending ? query.OrderByDescending(p => p.PurchaseDate) : query.OrderBy(p => p.PurchaseDate),
                    "receiveddate" => request.SortDescending ? query.OrderByDescending(p => p.ReceivedDate) : query.OrderBy(p => p.ReceivedDate),
                    _ => request.SortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt)
                };

                // Paginazione
                var totalCount = await query.CountAsync();
                var pageSize = request.GetValidPageSize();
                var page = request.GetValidPage();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var purchases = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var results = purchases.Select(p => new
                {
                    p.Id,
                    p.PurchaseId,
                    p.PurchaseCode,
                    p.PurchaseType,
                    p.DeviceCondition,
                    p.Brand,
                    p.Model,
                    p.SerialNumber,
                    p.IMEI,
                    p.SupplierId,
                    SupplierName = p.Supplier?.RagioneSociale,
                    p.PurchasePrice,
                    p.TotalAmount,
                    p.PaymentStatus,
                    p.PaidAmount,
                    p.RemainingAmount,
                    p.PurchaseStatus,
                    p.PurchaseStatusCode,
                    p.QualityCheckStatus,
                    p.PurchaseDate,
                    p.ReceivedDate,
                    p.CreatedAt,
                    p.Notes
                }).ToList();

                return Ok(new
                {
                    data = results,
                    pagination = new
                    {
                        currentPage = page,
                        pageSize,
                        totalPages,
                        totalCount
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Errore durante la ricerca degli acquisti", error = ex.Message });
            }
        }

        // ==================== UPDATE ====================
        /// <summary>
        /// Aggiorna un acquisto esistente
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PurchaseDetailDto>> UpdatePurchase(Guid id, [FromBody] UpdatePurchaseRequestDto request)
        {
            try
            {
                var purchase = await _context.DevicePurchases
                    .Where(p => p.PurchaseId == id && !p.IsDeleted)
                    .FirstOrDefaultAsync();

                if (purchase == null)
                {
                    return NotFound(new { message = "Acquisto non trovato" });
                }

                // Aggiorna solo i campi forniti
                if (request.PurchaseType != null) purchase.PurchaseType = request.PurchaseType;
                if (request.DeviceCondition != null) purchase.DeviceCondition = request.DeviceCondition;
                if (request.DeviceId.HasValue) purchase.DeviceId = request.DeviceId;
                if (request.DeviceRegistryId.HasValue) purchase.DeviceRegistryId = request.DeviceRegistryId;
                if (request.AccessoryId.HasValue) purchase.AccessoryId = request.AccessoryId;
                if (request.Brand != null) purchase.Brand = request.Brand;
                if (request.Model != null) purchase.Model = request.Model;
                if (request.SerialNumber != null) purchase.SerialNumber = request.SerialNumber;
                if (request.IMEI != null) purchase.IMEI = request.IMEI;
                if (request.SupplierId.HasValue) purchase.SupplierId = request.SupplierId.Value;
                if (request.CompanyId.HasValue) purchase.CompanyId = request.CompanyId.Value;
                if (request.PurchasePrice.HasValue) purchase.PurchasePrice = request.PurchasePrice.Value;
                if (request.ShippingCost.HasValue) purchase.ShippingCost = request.ShippingCost;
                if (request.OtherCosts.HasValue) purchase.OtherCosts = request.OtherCosts;
                if (request.VatRate.HasValue) purchase.VatRate = request.VatRate.Value;
                if (request.TotalAmount.HasValue) purchase.TotalAmount = request.TotalAmount.Value;
                if (request.PaymentType != null) purchase.PaymentType = request.PaymentType;
                if (request.PaymentStatus != null) purchase.PaymentStatus = request.PaymentStatus;
                if (request.PaidAmount.HasValue) purchase.PaidAmount = request.PaidAmount.Value;
                if (request.RemainingAmount.HasValue) purchase.RemainingAmount = request.RemainingAmount.Value;
                if (request.InstallmentsCount.HasValue) purchase.InstallmentsCount = request.InstallmentsCount;
                if (request.InstallmentAmount.HasValue) purchase.InstallmentAmount = request.InstallmentAmount;
                if (request.PurchaseStatus != null) purchase.PurchaseStatus = request.PurchaseStatus;
                if (request.PurchaseStatusCode != null) purchase.PurchaseStatusCode = request.PurchaseStatusCode;
                if (request.SupplierInvoiceId.HasValue) purchase.SupplierInvoiceId = request.SupplierInvoiceId;
                if (request.SupplierInvoiceNumber != null) purchase.SupplierInvoiceNumber = request.SupplierInvoiceNumber;
                if (request.SupplierInvoiceDate.HasValue) purchase.SupplierInvoiceDate = request.SupplierInvoiceDate;
                if (request.OrderNumber != null) purchase.OrderNumber = request.OrderNumber;
                if (request.OrderDate.HasValue) purchase.OrderDate = request.OrderDate;
                if (request.DDTNumber != null) purchase.DDTNumber = request.DDTNumber;
                if (request.DDTDate.HasValue) purchase.DDTDate = request.DDTDate;
                if (request.BuyerCode != null) purchase.BuyerCode = request.BuyerCode;
                if (request.BuyerName != null) purchase.BuyerName = request.BuyerName;
                if (request.Notes != null) purchase.Notes = request.Notes;
                if (request.IncludedAccessories != null) purchase.IncludedAccessories = request.IncludedAccessories;
                if (request.QualityCheckStatus != null) purchase.QualityCheckStatus = request.QualityCheckStatus;
                if (request.QualityCheckNotes != null) purchase.QualityCheckNotes = request.QualityCheckNotes;
                if (request.QualityCheckDate.HasValue) purchase.QualityCheckDate = request.QualityCheckDate;
                if (request.QualityCheckedBy != null) purchase.QualityCheckedBy = request.QualityCheckedBy;
                if (request.HasSupplierWarranty.HasValue) purchase.HasSupplierWarranty = request.HasSupplierWarranty.Value;
                if (request.SupplierWarrantyMonths.HasValue) purchase.SupplierWarrantyMonths = request.SupplierWarrantyMonths;
                if (request.SupplierWarrantyExpiryDate.HasValue) purchase.SupplierWarrantyExpiryDate = request.SupplierWarrantyExpiryDate;
                if (request.PurchaseDate.HasValue) purchase.PurchaseDate = request.PurchaseDate;
                if (request.ReceivedDate.HasValue) purchase.ReceivedDate = request.ReceivedDate;

                purchase.UpdatedAt = DateTime.UtcNow;
                purchase.UpdatedBy = request.UpdatedBy;

                await _context.SaveChangesAsync();

                // Ricarica con le relazioni
                var updatedPurchase = await _context.DevicePurchases
                    .Include(p => p.Supplier)
                    .Include(p => p.Company)
                    .Include(p => p.Payments.Where(pay => !pay.IsDeleted))
                    .FirstOrDefaultAsync(p => p.PurchaseId == id);

                var dto = MapToPurchaseDetailDto(updatedPurchase!);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Errore durante l'aggiornamento dell'acquisto", error = ex.Message });
            }
        }

        /// <summary>
        /// Aggiorna lo stato di un acquisto
        /// </summary>
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdatePurchaseStatus(Guid id, [FromBody] UpdatePurchaseStatusDto request)
        {
            try
            {
                var purchase = await _context.DevicePurchases
                    .Where(p => p.PurchaseId == id && !p.IsDeleted)
                    .FirstOrDefaultAsync();

                if (purchase == null)
                {
                    return NotFound(new { message = "Acquisto non trovato" });
                }

                purchase.PurchaseStatus = request.PurchaseStatus;
                purchase.PurchaseStatusCode = request.PurchaseStatusCode;
                purchase.UpdatedAt = DateTime.UtcNow;
                purchase.UpdatedBy = request.UpdatedBy;

                if (!string.IsNullOrWhiteSpace(request.Notes))
                {
                    purchase.Notes = string.IsNullOrWhiteSpace(purchase.Notes)
                        ? request.Notes
                        : $"{purchase.Notes}\n{DateTime.UtcNow:yyyy-MM-dd HH:mm}: {request.Notes}";
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Stato aggiornato con successo", purchaseStatus = purchase.PurchaseStatus, purchaseStatusCode = purchase.PurchaseStatusCode });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Errore durante l'aggiornamento dello stato", error = ex.Message });
            }
        }

        // ==================== DELETE (SOFT) ====================
        /// <summary>
        /// Cancellazione soft di un acquisto
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePurchase(Guid id, [FromQuery] string? deletedBy = null)
        {
            try
            {
                var purchase = await _context.DevicePurchases
                    .Where(p => p.PurchaseId == id && !p.IsDeleted)
                    .FirstOrDefaultAsync();

                if (purchase == null)
                {
                    return NotFound(new { message = "Acquisto non trovato" });
                }

                // Soft delete
                purchase.IsDeleted = true;
                purchase.DeletedAt = DateTime.UtcNow;
                purchase.DeletedBy = deletedBy;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Acquisto cancellato con successo" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Errore durante la cancellazione dell'acquisto", error = ex.Message });
            }
        }

        /// <summary>
        /// Ripristina un acquisto cancellato
        /// </summary>
        [HttpPost("{id:guid}/restore")]
        public async Task<IActionResult> RestorePurchase(Guid id)
        {
            try
            {
                var purchase = await _context.DevicePurchases
                    .Where(p => p.PurchaseId == id && p.IsDeleted)
                    .FirstOrDefaultAsync();

                if (purchase == null)
                {
                    return NotFound(new { message = "Acquisto cancellato non trovato" });
                }

                purchase.IsDeleted = false;
                purchase.DeletedAt = null;
                purchase.DeletedBy = null;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Acquisto ripristinato con successo" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Errore durante il ripristino dell'acquisto", error = ex.Message });
            }
        }

        // ==================== PAGAMENTI ====================
        /// <summary>
        /// Aggiunge un pagamento a un acquisto
        /// </summary>
        [HttpPost("{id:guid}/payments")]
        public async Task<ActionResult<AddPurchasePaymentResponseDto>> AddPayment(Guid id, [FromBody] PurchasePaymentDto payment)
        {
            try
            {
                var purchase = await _context.DevicePurchases
                    .Where(p => p.PurchaseId == id && !p.IsDeleted)
                    .FirstOrDefaultAsync();

                if (purchase == null)
                {
                    return NotFound(new { message = "Acquisto non trovato" });
                }

                var newPayment = new PurchasePayment
                {
                    PaymentId = Guid.NewGuid(),
                    PurchaseId = purchase.PurchaseId,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod,
                    TransactionReference = payment.TransactionReference,
                    PaymentDate = payment.PaymentDate ?? DateTime.UtcNow,
                    Notes = payment.Notes,
                    PaidBy = payment.PaidBy,
                    CreatedAt = DateTime.UtcNow
                };

                _context.PurchasePayments.Add(newPayment);

                // Aggiorna gli importi
                purchase.PaidAmount += payment.Amount;
                purchase.RemainingAmount = purchase.TotalAmount - purchase.PaidAmount;

                // Aggiorna lo stato del pagamento
                if (purchase.PaidAmount >= purchase.TotalAmount)
                {
                    purchase.PaymentStatus = "Pagato";
                }
                else if (purchase.PaidAmount > 0)
                {
                    purchase.PaymentStatus = "Parzialmente Pagato";
                }
                else
                {
                    purchase.PaymentStatus = "Da Pagare";
                }

                purchase.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var response = new AddPurchasePaymentResponseDto
                {
                    PaymentId = newPayment.PaymentId,
                    PurchaseId = purchase.PurchaseId,
                    Amount = newPayment.Amount,
                    PaymentMethod = newPayment.PaymentMethod,
                    PaymentDate = newPayment.PaymentDate,
                    NewPaidAmount = purchase.PaidAmount,
                    NewRemainingAmount = purchase.RemainingAmount,
                    NewPaymentStatus = purchase.PaymentStatus
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Errore durante l'aggiunta del pagamento", error = ex.Message });
            }
        }

        /// <summary>
        /// Ottiene tutti i pagamenti di un acquisto
        /// </summary>
        [HttpGet("{id:guid}/payments")]
        public async Task<ActionResult<List<PurchasePaymentDetailDto>>> GetPurchasePayments(Guid id)
        {
            try
            {
                var payments = await _context.PurchasePayments
                    .Where(p => p.PurchaseId == id && !p.IsDeleted)
                    .OrderByDescending(p => p.PaymentDate)
                    .ToListAsync();

                var dtos = payments.Select(p => new PurchasePaymentDetailDto
                {
                    Id = p.Id,
                    PaymentId = p.PaymentId,
                    PurchaseId = p.PurchaseId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    TransactionReference = p.TransactionReference,
                    PaymentDate = p.PaymentDate,
                    Notes = p.Notes,
                    PaidBy = p.PaidBy,
                    CreatedAt = p.CreatedAt,
                    IsDeleted = p.IsDeleted
                }).ToList();

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Errore durante il recupero dei pagamenti", error = ex.Message });
            }
        }

        /// <summary>
        /// Cancellazione soft di un pagamento
        /// </summary>
        [HttpDelete("{purchaseId:guid}/payments/{paymentId:guid}")]
        public async Task<IActionResult> DeletePayment(Guid purchaseId, Guid paymentId)
        {
            try
            {
                var payment = await _context.PurchasePayments
                    .Where(p => p.PurchaseId == purchaseId && p.PaymentId == paymentId && !p.IsDeleted)
                    .FirstOrDefaultAsync();

                if (payment == null)
                {
                    return NotFound(new { message = "Pagamento non trovato" });
                }

                var purchase = await _context.DevicePurchases
                    .Where(p => p.PurchaseId == purchaseId && !p.IsDeleted)
                    .FirstOrDefaultAsync();

                if (purchase == null)
                {
                    return NotFound(new { message = "Acquisto non trovato" });
                }

                // Soft delete del pagamento
                payment.IsDeleted = true;

                // Ricalcola gli importi
                purchase.PaidAmount -= payment.Amount;
                purchase.RemainingAmount = purchase.TotalAmount - purchase.PaidAmount;

                // Aggiorna lo stato del pagamento
                if (purchase.PaidAmount >= purchase.TotalAmount)
                {
                    purchase.PaymentStatus = "Pagato";
                }
                else if (purchase.PaidAmount > 0)
                {
                    purchase.PaymentStatus = "Parzialmente Pagato";
                }
                else
                {
                    purchase.PaymentStatus = "Da Pagare";
                }

                purchase.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Pagamento cancellato con successo" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Errore durante la cancellazione del pagamento", error = ex.Message });
            }
        }

        // ==================== HELPER METHODS ====================
        private async Task<string> GeneratePurchaseCodeAsync(Guid multitenantId)
        {
            var year = DateTime.UtcNow.Year.ToString().Substring(2);
            var lastPurchase = await _context.DevicePurchases
                .Where(p => p.MultitenantId == multitenantId && p.PurchaseCode!.StartsWith($"ACQ{year}"))
                .OrderByDescending(p => p.PurchaseCode)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastPurchase != null && !string.IsNullOrEmpty(lastPurchase.PurchaseCode))
            {
                var lastNumber = lastPurchase.PurchaseCode.Substring(5);
                if (int.TryParse(lastNumber, out int num))
                {
                    nextNumber = num + 1;
                }
            }

            return $"ACQ{year}{nextNumber:D5}";
        }

        private PurchaseDetailDto MapToPurchaseDetailDto(DevicePurchase purchase)
        {
            return new PurchaseDetailDto
            {
                Id = purchase.Id,
                PurchaseId = purchase.PurchaseId,
                PurchaseCode = purchase.PurchaseCode,
                PurchaseType = purchase.PurchaseType,
                DeviceCondition = purchase.DeviceCondition,
                DeviceId = purchase.DeviceId,
                DeviceRegistryId = purchase.DeviceRegistryId,
                AccessoryId = purchase.AccessoryId,
                Brand = purchase.Brand,
                Model = purchase.Model,
                SerialNumber = purchase.SerialNumber,
                IMEI = purchase.IMEI,
                SupplierId = purchase.SupplierId,
                SupplierName = purchase.Supplier?.RagioneSociale,
                CompanyId = purchase.CompanyId,
                CompanyName = purchase.Company?.RagioneSociale,
                MultitenantId = purchase.MultitenantId,
                PurchasePrice = purchase.PurchasePrice,
                ShippingCost = purchase.ShippingCost,
                OtherCosts = purchase.OtherCosts,
                VatRate = purchase.VatRate,
                TotalAmount = purchase.TotalAmount,
                PaymentType = purchase.PaymentType,
                PaymentStatus = purchase.PaymentStatus,
                PaidAmount = purchase.PaidAmount,
                RemainingAmount = purchase.RemainingAmount,
                InstallmentsCount = purchase.InstallmentsCount,
                InstallmentAmount = purchase.InstallmentAmount,
                PurchaseStatus = purchase.PurchaseStatus,
                PurchaseStatusCode = purchase.PurchaseStatusCode,
                SupplierInvoiceId = purchase.SupplierInvoiceId,
                SupplierInvoiceNumber = purchase.SupplierInvoiceNumber,
                SupplierInvoiceDate = purchase.SupplierInvoiceDate,
                OrderNumber = purchase.OrderNumber,
                OrderDate = purchase.OrderDate,
                DDTNumber = purchase.DDTNumber,
                DDTDate = purchase.DDTDate,
                BuyerCode = purchase.BuyerCode,
                BuyerName = purchase.BuyerName,
                Notes = purchase.Notes,
                IncludedAccessories = purchase.IncludedAccessories,
                QualityCheckStatus = purchase.QualityCheckStatus,
                QualityCheckNotes = purchase.QualityCheckNotes,
                QualityCheckDate = purchase.QualityCheckDate,
                QualityCheckedBy = purchase.QualityCheckedBy,
                HasSupplierWarranty = purchase.HasSupplierWarranty,
                SupplierWarrantyMonths = purchase.SupplierWarrantyMonths,
                SupplierWarrantyExpiryDate = purchase.SupplierWarrantyExpiryDate,
                CreatedAt = purchase.CreatedAt,
                PurchaseDate = purchase.PurchaseDate,
                ReceivedDate = purchase.ReceivedDate,
                UpdatedAt = purchase.UpdatedAt,
                CreatedBy = purchase.CreatedBy,
                UpdatedBy = purchase.UpdatedBy,
                Payments = purchase.Payments?.Where(p => !p.IsDeleted).Select(p => new PurchasePaymentDetailDto
                {
                    Id = p.Id,
                    PaymentId = p.PaymentId,
                    PurchaseId = p.PurchaseId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    TransactionReference = p.TransactionReference,
                    PaymentDate = p.PaymentDate,
                    Notes = p.Notes,
                    PaidBy = p.PaidBy,
                    CreatedAt = p.CreatedAt,
                    IsDeleted = p.IsDeleted
                }).ToList()
            };
        }
    }
}