using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaLabAPI.Models
{
    [Table("AffiliateGeolocations")]
    public class AffiliateGeolocation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid AffiliateId { get; set; }

        [Column(TypeName = "decimal(10, 8)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(11, 8)")]
        public decimal? Longitude { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [Required]
        public DateTime GeocodedDate { get; set; }

        [MaxLength(50)]
        public string? Quality { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(100)]
        public string? Notes { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [MaxLength(50)]
        public string? GeocodingSource { get; set; }

        // Navigation property
        [ForeignKey("AffiliateId")]
        public virtual C_ANA_Company? Affiliate { get; set; }

        // Computed property per verificare se ha coordinate valide
        [NotMapped]
        public bool HasValidCoordinates => Latitude.HasValue && Longitude.HasValue
            && Latitude.Value != 0 && Longitude.Value != 0;
    }
}