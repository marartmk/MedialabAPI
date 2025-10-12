using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaLabAPI.DTOs.RepairParts;

namespace MediaLabAPI.Services
{
    public interface IRepairPartsService
    {
        /// <summary>
        /// Ottiene tutti i ricambi associati a una riparazione
        /// </summary>
        Task<List<RepairPartDto>> GetRepairPartsAsync(Guid repairId);

        /// <summary>
        /// Ottiene un singolo ricambio per ID
        /// </summary>
        Task<RepairPartDto?> GetRepairPartByIdAsync(int id);

        /// <summary>
        /// Aggiunge un ricambio a una riparazione
        /// </summary>
        Task<RepairPartDto> AddRepairPartAsync(Guid repairId, CreateRepairPartDto dto, string? createdBy = null);

        /// <summary>
        /// Aggiunge più ricambi in batch
        /// </summary>
        Task<List<RepairPartDto>> AddRepairPartsBatchAsync(Guid repairId, List<CreateRepairPartDto> dtos, string? createdBy = null);

        /// <summary>
        /// Aggiorna un ricambio esistente
        /// </summary>
        Task UpdateRepairPartAsync(Guid repairId, int partId, UpdateRepairPartDto dto);

        /// <summary>
        /// Rimuove un ricambio da una riparazione
        /// </summary>
        Task DeleteRepairPartAsync(Guid repairId, int partId);

        /// <summary>
        /// Calcola il totale dei ricambi di una riparazione
        /// </summary>
        Task<RepairPartsTotalDto> GetRepairPartsTotalAsync(Guid repairId);

        /// <summary>
        /// Scarica i ricambi dal magazzino (consume stock)
        /// </summary>
        Task<ConsumeStockResponseDto> ConsumeStockAsync(Guid repairId, string? processedBy = null);
    }
}