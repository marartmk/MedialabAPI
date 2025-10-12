using System.Collections.Generic;

namespace MediaLabAPI.DTOs.RepairParts
{
    /// <summary>
    /// DTO per il riepilogo totale dei ricambi di una riparazione
    /// </summary>
    public class RepairPartsTotalDto
    {
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
        public List<RepairPartDto> Items { get; set; } = new();
    }
}