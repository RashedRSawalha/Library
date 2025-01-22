using System;
using System.Threading.Tasks;

namespace Shared.Redis
{
    public interface IRedisCache
    {
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<T?> GetAsync<T>(string key, string apiUrl = "");
        Task RemoveAsync(string key);
    }
}
