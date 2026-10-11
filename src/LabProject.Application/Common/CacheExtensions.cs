using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
namespace LabProject.Application.Common
{
    public static class CacheExtensions
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static async Task<T?> GetRecordAsync<T>(
            this IDistributedCache cache,
            string recordId,
            CancellationToken cancellationToken = default)
        {
            var jsonData = await cache.GetStringAsync(recordId, cancellationToken);
            return jsonData is null ? default : JsonSerializer.Deserialize<T>(jsonData, JsonOptions);
        }

        public static async Task SetRecordAsync<T>(
            this IDistributedCache cache,
            string recordId,
            T data,
            TimeSpan? absoluteExpireTime = null,
            CancellationToken cancellationToken = default)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromMinutes(5)
            };

            var jsonData = JsonSerializer.Serialize(data, JsonOptions);
            await cache.SetStringAsync(recordId, jsonData, options, cancellationToken);
        }
    }
}
