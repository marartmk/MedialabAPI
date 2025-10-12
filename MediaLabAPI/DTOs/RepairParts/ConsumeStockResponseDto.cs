using System.Collections.Generic;

namespace MediaLabAPI.DTOs.RepairParts
{
    /// <summary>
    /// DTO per la risposta dello scarico magazzino
    /// </summary>
    public class ConsumeStockResponseDto
    {
        public bool Success { get; set; }
        public int ConsumedItems { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }
    }
}