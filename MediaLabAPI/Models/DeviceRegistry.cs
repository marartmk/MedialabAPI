using System;
using System.Collections.Generic;

namespace MediaLabAPI.Models;

public partial class DeviceRegistry
{
    public int Id { get; set; }

    public Guid DeviceId { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? CompanyId { get; set; }

    public Guid? MultitenantId { get; set; }

    public string SerialNumber { get; set; }

    public string Brand { get; set; }

    public string Model { get; set; }

    public string DeviceType { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public string? ReceiptNumber { get; set; }

    public string? Retailer { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }
}
