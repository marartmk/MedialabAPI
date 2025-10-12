using MediaLabAPI.Data;
using MediaLabAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace MediaLabAPI.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly AppDbContext _db;

        public ApiKeyService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<string?> GetKeyAsync(string serviceName)
        {
            var key = await _db.ApiKeys
                .Where(k => k.ServiceName == serviceName && k.IsActive)
                .OrderByDescending(k => k.CreatedAt)
                .FirstOrDefaultAsync();

            return key?.ApiKeyValue;
        }
    }
}
