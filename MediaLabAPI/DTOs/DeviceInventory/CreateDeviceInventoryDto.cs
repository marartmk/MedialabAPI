using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.DeviceInventory;

public class CreateDeviceInventoryDto
{
    [Required(ErrorMessage = "Il codice è obbligatorio")]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Il tipo di dispositivo è obbligatorio")]
    [RegularExpression("^(smartphone|tablet)$", ErrorMessage = "Il tipo deve essere 'smartphone' o 'tablet'")]
    public string DeviceType { get; set; } = "smartphone";

    [Required(ErrorMessage = "La marca è obbligatoria")]
    [MaxLength(100)]
    public string Brand { get; set; } = string.Empty;

    [Required(ErrorMessage = "Il modello è obbligatorio")]
    [MaxLength(100)]
    public string Model { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'IMEI è obbligatorio")]
    [MaxLength(50)]
    public string IMEI { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? ESN { get; set; }

    [MaxLength(100)]
    public string? SerialNumber { get; set; }

    [Required(ErrorMessage = "Il colore è obbligatorio")]
    [MaxLength(50)]
    public string Color { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(new|used|refurbished)$", ErrorMessage = "Condizione non valida")]
    public string DeviceCondition { get; set; } = "new";

    public bool IsCourtesyDevice { get; set; } = false;

    [Required]
    [RegularExpression("^(available|loaned|sold|unavailable)$", ErrorMessage = "Stato non valido")]
    public string DeviceStatus { get; set; } = "available";

    [Required(ErrorMessage = "Il fornitore è obbligatorio")]
    [MaxLength(50)]
    public string SupplierId { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Il prezzo di acquisto deve essere positivo")]
    public decimal PurchasePrice { get; set; } = 0;

    [Range(0, double.MaxValue, ErrorMessage = "Il prezzo di vendita deve essere positivo")]
    public decimal SellingPrice { get; set; } = 0;

    public DateTime? PurchaseDate { get; set; }

    [MaxLength(100)]
    public string? Location { get; set; }

    public string? Notes { get; set; }

    [Required]
    public Guid MultitenantId { get; set; }
}