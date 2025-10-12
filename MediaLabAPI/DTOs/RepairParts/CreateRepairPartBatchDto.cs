using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.RepairParts
{
    /// <summary>
    /// DTO per aggiungere più ricambi in batch
    /// </summary>
    public class CreateRepairPartBatchDto
    {
        [Required(ErrorMessage = "La lista di ricambi è obbligatoria")]
        [MinLength(1, ErrorMessage = "Devi inserire almeno un ricambio")]
        public List<CreateRepairPartDto> Parts { get; set; } = new();
    }
}