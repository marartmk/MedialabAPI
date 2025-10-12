using System;

namespace MediaLabAPI.DTOs.RepairParts
{
    /// <summary>
    /// DTO per la risposta - Rappresenta un ricambio usato in una riparazione
    /// </summary>
    public class RepairPartDto
    {
        public int Id { get; set; }
        public Guid RepairId { get; set; }
        public int WarehouseItemId { get; set; }
        public Guid ItemId { get; set; }

        // Snapshot dei dati articolo
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;

        // Quantità e prezzi
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }

        // Info aggiuntive
        public int AvailableStock { get; set; } // Giacenza attuale in magazzino
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? Notes { get; set; }
    }
}