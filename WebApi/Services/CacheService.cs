using StackExchange.Redis;
using System.Text.Json;

namespace WebApi.Services
{
    public class CacheService : ICacheService
    {
        IDatabase _cacheDb;

        public CacheService()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _cacheDb = redis.GetDatabase();
        }

        public T GetData<T>(string key)
        {
            string valueStr = _cacheDb.StringGet(key);

            if(string.IsNullOrEmpty(valueStr))
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(valueStr);
        }

        public bool RemoveData(string key)
        {
            bool exists = _cacheDb.KeyExists(key);

            if (!exists) return false;

            return _cacheDb.KeyDelete(key);
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expiration)
        {
            var expiryTime = expiration.DateTime.Subtract(DateTime.Now);

            return _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expiry: expiryTime);
        }
    }
}
