using StackExchange.Redis;

namespace WalletApi.Services
{
    public interface IRedisService
    {
        Task<List<T>> GetListAsync<T>(string key);

        Task StoreListAsync<T>(string key, List<T> value, TimeSpan? expiry = null);
    }
}
