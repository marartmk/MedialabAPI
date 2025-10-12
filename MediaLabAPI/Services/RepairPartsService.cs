using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaLabAPI.Data;
using MediaLabAPI.DTOs.RepairParts;
using MediaLabAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaLabAPI.Services
{
    public class RepairPartsService : IRepairPartsService
    {
        private readonly AppDbContext _context;

        public RepairPartsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RepairPartDto>> GetRepairPartsAsync(Guid repairId)
        {
            var parts = await _context.RepairParts
                .Include(p => p.WarehouseItem)
                .Where(p => p.RepairId == repairId)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();

            return parts.Select(p => MapToDto(p)).ToList();
        }

        public async Task<RepairPartDto?> GetRepairPartByIdAsync(int id)
        {
            var part = await _context.RepairParts
                .Include(p => p.WarehouseItem)
                .FirstOrDefaultAsync(p => p.Id == id);

            return part != null ? MapToDto(part) : null;
        }

        public async Task<RepairPartDto> AddRepairPartAsync(Guid repairId, CreateRepairPartDto dto, string? createdBy = null)
        {
            // Verifica che la riparazione esista
            var repair = await _context.DeviceRepairs
                .FirstOrDefaultAsync(r => r.RepairId == repairId);

            if (repair == null)
                throw new KeyNotFoundException($"Riparazione con ID {repairId} non trovata");

            // Ottieni l'articolo dal magazzino
            var warehouseItem = await _context.WarehouseItems
                .FirstOrDefaultAsync(w => w.Id == dto.WarehouseItemId && !w.IsDeleted);

            if (warehouseItem == null)
                throw new KeyNotFoundException($"Articolo di magazzino con ID {dto.WarehouseItemId} non trovato");

            // Verifica disponibilità
            if (warehouseItem.Quantity < dto.Quantity)
                throw new InvalidOperationException(
                    $"Quantità insufficiente in magazzino. Disponibili: {warehouseItem.Quantity}, Richiesti: {dto.Quantity}");

            // Verifica se il ricambio è già stato aggiunto a questa riparazione
            var existingPart = await _context.RepairParts
                .FirstOrDefaultAsync(p => p.RepairId == repairId && p.WarehouseItemId == dto.WarehouseItemId);

            if (existingPart != null)
            {
                // Aggiorna la quantità invece di creare un duplicato
                existingPart.Quantity += dto.Quantity;
                existingPart.LineTotal = existingPart.Quantity * existingPart.UnitPrice;
                existingPart.Notes = dto.Notes ?? existingPart.Notes;

                await _context.SaveChangesAsync();
                return MapToDto(existingPart, warehouseItem.Quantity);
            }

            // Crea nuovo ricambio
            var repairPart = new RepairPart
            {
                RepairId = repairId,
                WarehouseItemId = warehouseItem.Id,
                ItemId = warehouseItem.ItemId,
                Code = warehouseItem.Code,
                Name = warehouseItem.Name,
                Brand = warehouseItem.Brand,
                Model = warehouseItem.Model,
                Quantity = dto.Quantity,
                UnitPrice = warehouseItem.UnitPrice,
                LineTotal = dto.Quantity * warehouseItem.UnitPrice,
                CompanyId = repair.CompanyId,
                MultitenantId = repair.MultitenantId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy,
                Notes = dto.Notes
            };

            _context.RepairParts.Add(repairPart);
            await _context.SaveChangesAsync();

            return MapToDto(repairPart, warehouseItem.Quantity);
        }

        public async Task<List<RepairPartDto>> AddRepairPartsBatchAsync(Guid repairId, List<CreateRepairPartDto> dtos, string? createdBy = null)
        {
            var result = new List<RepairPartDto>();

            foreach (var dto in dtos)
            {
                var part = await AddRepairPartAsync(repairId, dto, createdBy);
                result.Add(part);
            }

            return result;
        }

        public async Task UpdateRepairPartAsync(Guid repairId, int partId, UpdateRepairPartDto dto)
        {
            var part = await _context.RepairParts
                .Include(p => p.WarehouseItem)
                .FirstOrDefaultAsync(p => p.Id == partId && p.RepairId == repairId);

            if (part == null)
                throw new KeyNotFoundException($"Ricambio con ID {partId} non trovato per questa riparazione");

            // Verifica disponibilità in magazzino se aumenti la quantità
            if (dto.Quantity > part.Quantity && part.WarehouseItem != null)
            {
                var additionalQty = dto.Quantity - part.Quantity;
                if (part.WarehouseItem.Quantity < additionalQty)
                    throw new InvalidOperationException(
                        $"Quantità insufficiente in magazzino. Disponibili: {part.WarehouseItem.Quantity}");
            }

            // Aggiorna
            part.Quantity = dto.Quantity;

            if (dto.UnitPrice.HasValue)
                part.UnitPrice = dto.UnitPrice.Value;

            part.LineTotal = part.Quantity * part.UnitPrice;
            part.Notes = dto.Notes ?? part.Notes;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteRepairPartAsync(Guid repairId, int partId)
        {
            var part = await _context.RepairParts
                .FirstOrDefaultAsync(p => p.Id == partId && p.RepairId == repairId);

            if (part == null)
                throw new KeyNotFoundException($"Ricambio con ID {partId} non trovato per questa riparazione");

            _context.RepairParts.Remove(part);
            await _context.SaveChangesAsync();
        }

        public async Task<RepairPartsTotalDto> GetRepairPartsTotalAsync(Guid repairId)
        {
            var parts = await GetRepairPartsAsync(repairId);

            return new RepairPartsTotalDto
            {
                TotalAmount = parts.Sum(p => p.LineTotal),
                ItemCount = parts.Count,
                Items = parts
            };
        }

        public async Task<ConsumeStockResponseDto> ConsumeStockAsync(Guid repairId, string? processedBy = null)
        {
            var parts = await _context.RepairParts
                .Include(p => p.WarehouseItem)
                .Where(p => p.RepairId == repairId)
                .ToListAsync();

            if (!parts.Any())
            {
                return new ConsumeStockResponseDto
                {
                    Success = false,
                    ConsumedItems = 0,
                    Message = "Nessun ricambio da scaricare per questa riparazione",
                    Errors = new List<string> { "Lista ricambi vuota" }
                };
            }

            var errors = new List<string>();
            var consumedCount = 0;

            foreach (var part in parts)
            {
                if (part.WarehouseItem == null)
                {
                    errors.Add($"Articolo magazzino ID {part.WarehouseItemId} non trovato");
                    continue;
                }

                // Verifica disponibilità
                if (part.WarehouseItem.Quantity < part.Quantity)
                {
                    errors.Add($"Quantità insufficiente per {part.Name}. Disponibili: {part.WarehouseItem.Quantity}, Richiesti: {part.Quantity}");
                    continue;
                }

                // Scarica dal magazzino
                part.WarehouseItem.Quantity -= part.Quantity;
                part.WarehouseItem.TotalValue = part.WarehouseItem.Quantity * part.WarehouseItem.UnitPrice;
                part.WarehouseItem.UpdatedAt = DateTime.UtcNow;
                part.WarehouseItem.UpdatedBy = processedBy;

                consumedCount++;
            }

            await _context.SaveChangesAsync();

            return new ConsumeStockResponseDto
            {
                Success = errors.Count == 0,
                ConsumedItems = consumedCount,
                Message = errors.Count == 0
                    ? $"Scaricati {consumedCount} ricambi dal magazzino con successo"
                    : $"Scaricati {consumedCount} ricambi, ma con {errors.Count} errori",
                Errors = errors.Count > 0 ? errors : null
            };
        }

        // Helper: Mapping da Model a DTO
        private RepairPartDto MapToDto(RepairPart part, int? availableStock = null)
        {
            return new RepairPartDto
            {
                Id = part.Id,
                RepairId = part.RepairId,
                WarehouseItemId = part.WarehouseItemId,
                ItemId = part.ItemId,
                Code = part.Code,
                Name = part.Name,
                Brand = part.Brand,
                Model = part.Model,
                Quantity = part.Quantity,
                UnitPrice = part.UnitPrice,
                LineTotal = part.LineTotal,
                AvailableStock = availableStock ?? part.WarehouseItem?.Quantity ?? 0,
                CreatedAt = part.CreatedAt,
                CreatedBy = part.CreatedBy,
                Notes = part.Notes
            };
        }
    }
}