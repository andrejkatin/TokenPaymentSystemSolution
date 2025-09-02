using Azure.Core.Pipeline;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using StackExchange.Redis;

namespace WalletApi.Services.Impl
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase Redis;
        private readonly AsyncRetryPolicy RetryPolicy;
        private readonly ILogger<RedisService> Logger;

        public RedisService(IConnectionMultiplexer redis, ILogger<RedisService> logger)
        {
            Redis = redis.GetDatabase();
            Logger = logger;
            // Polly Retry Logic:
            // This retry policy is designed to handle Redis downtimes gracefully for up to 45 seconds.
            // The policy will react to RedisConnectionExceptions using an exponential backoff strategy:
            // 1st retry: After 3 seconds.
            // 2nd retry: After 6 seconds.
            // 3rd retry: After 12 seconds.
            // 4th retry: After 24 seconds.
            // This gives a total retry window of 45 seconds (3 + 6 + 12 + 24).
            // It complements the StackExchange.Redis connection settings which have their own retry mechanisms,
            // ensuring robustness in the face of transient Redis outages.
            RetryPolicy = Policy
            .Handle<RedisConnectionException>()
            .WaitAndRetryAsync(4, // retry 4 times
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt + 1)), // exponential backoff
                (exception, timeSpan, retryCount, context) =>
                {
                    Logger.LogWarning("Redis operation failed with message: {message}. Waiting {timeSpan} before next retry. Retry attempt {retryCount}.", exception.Message, timeSpan, retryCount);
                });
        }

        private async Task<RedisValue> GetRedisValueAsync(string key)
        {
            return await RetryPolicy.ExecuteAsync(() => Redis.StringGetAsync(key));
        }

        public async Task<List<T>> GetListAsync<T>(string key)
        {
            RedisValue value = await GetRedisValueAsync(key);

            return value.IsNullOrEmpty ? new List<T>() : JsonConvert.DeserializeObject<List<T>>(value.ToString());
        }

        public async Task StoreListAsync<T>(string key, List<T> value, TimeSpan? expiry = null)
        {
            await Redis.StringSetAsync(key, JsonConvert.SerializeObject(value), expiry);
        }
    }
}
