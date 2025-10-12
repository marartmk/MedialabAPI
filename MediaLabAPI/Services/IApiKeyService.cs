using System.Threading.Tasks;

namespace MediaLabAPI.Services
{
    public interface IApiKeyService
    {
        Task<string?> GetKeyAsync(string serviceName);
    }
}
