using MediaLabAPI.DTOs;

namespace MediaLabAPI.Services
{
    public interface IDiagnosticsService
    {
        // INCOMING
        Task<IncomingTestDto?> GetIncomingAsync(Guid repairId);
        Task UpsertIncomingAsync(Guid repairId, IncomingTestDto dto);
        Task DeleteIncomingAsync(Guid repairId);

        // EXIT
        Task<ExitTestDto?> GetExitAsync(Guid repairId);
        Task UpsertExitAsync(Guid repairId, ExitTestDto dto);
        Task DeleteExitAsync(Guid repairId);
    }
}
