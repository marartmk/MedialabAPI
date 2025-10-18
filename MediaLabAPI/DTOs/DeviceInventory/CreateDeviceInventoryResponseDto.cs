namespace MediaLabAPI.DTOs.DeviceInventory;

public class CreateDeviceInventoryResponseDto
{
    public int Id { get; set; }
    public Guid DeviceId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}