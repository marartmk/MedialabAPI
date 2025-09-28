namespace MediaLabAPI.DTOs
{
    public class AffiliateGeolocationDto
    {
        public int Id { get; set; }
        public Guid AffiliateId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? Address { get; set; }
        public DateTime GeocodedDate { get; set; }
        public string? Quality { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public string? GeocodingSource { get; set; }
        public bool HasValidCoordinates { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateAffiliateGeolocationDto
    {
        public Guid AffiliateId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? Address { get; set; }
        public string? Quality { get; set; }
        public string? Notes { get; set; }
        public string? GeocodingSource { get; set; } = "Manual";
    }

    public class UpdateAffiliateGeolocationDto
    {
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? Address { get; set; }
        public string? Quality { get; set; }
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }
        public string? GeocodingSource { get; set; }
    }

    public class BatchGeolocationDto
    {
        public Guid AffiliateId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Quality { get; set; } = "UNKNOWN";
        public string? Notes { get; set; }
    }

    public class BatchGeolocationRequestDto
    {
        public List<BatchGeolocationDto> Affiliates { get; set; } = new List<BatchGeolocationDto>();
    }

    public class BatchGeolocationResultDto
    {
        public int TotalProcessed { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public int CreatedCount { get; set; }
        public int UpdatedCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class BatchGeolocationCheckDto
    {
        public List<Guid> AffiliateIds { get; set; } = new List<Guid>();
    }

    public class BatchGeolocationCheckResultDto
    {
        public int TotalChecked { get; set; }
        public int AlreadyGeolocated { get; set; }
        public int NeedGeolocation { get; set; }
        public List<AffiliateGeolocationStatusDto> AffiliatesStatus { get; set; } = new List<AffiliateGeolocationStatusDto>();
    }

    public class AffiliateGeolocationStatusDto
    {
        public Guid AffiliateId { get; set; }
        public bool IsGeolocated { get; set; }
        public string Quality { get; set; } = "UNKNOWN";
        public bool NeedsUpdate { get; set; }
        public DateTime? LastGeocodedDate { get; set; }
    }
}