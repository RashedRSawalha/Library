using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shared.Redis
{
    public class RedisCache : IRedisCache
    {
        private readonly IDistributedCache _distributedCache;
        private readonly HttpClient _httpClient;

        public RedisCache(IDistributedCache distributedCache, HttpClient httpClient)
        {
            _distributedCache = distributedCache;
            _httpClient = httpClient;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(10) // Default 10 minutes
            };

            await _distributedCache.SetStringAsync(key, json, options);
        }

        public async Task<T> GetAsync<T>(string key, string apiUrl = "")
        {
            var cachedValue = await _distributedCache.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cachedValue))
            {
                // Deserialize and return the cached value
                return JsonSerializer.Deserialize<T>(cachedValue)!;
            }
            else if (!string.IsNullOrEmpty(apiUrl))
            {
                // If not found in cache and API URL is provided, fetch from external API
                return await GetFromExternalAPI<T>(key, apiUrl);
            }

            return default!;
        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

        private async Task<T> GetFromExternalAPI<T>(string key, string apiUrl)
        {
            try
            {
                // Send a GET request to the external API
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // Read the response content
                string responseContent = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true};
                var result = JsonSerializer.Deserialize<T>(responseContent, options);

                // Cache the fetched data
                await SetAsync(key, result, TimeSpan.FromMinutes(10));

                return result!;
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException($"Error fetching from external API: {ex.Message}");
            }
        }
    }
}
