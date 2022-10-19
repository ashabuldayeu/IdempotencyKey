using StackExchange.Redis;

namespace IdempotencyKey.Redis
{
    public class RedisIdempotencyStorage : ICacheIdempotencyStorage
    {
        private readonly IDatabase _database;

        public RedisIdempotencyStorage(IDatabase database)
        {
            this._database = database;
        }
        /// <summary>
        /// Returns false if key already exists, otherwise true
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> CreateAsync(IIdempotentRequest request)
        {
            // seems like this is "atomic" operation and increment one of the best way to check if request alredy exists in system
            long storedValue = await _database.StringIncrementAsync(new RedisKey(request.GetIdempotentKey()), 1);

            return storedValue == 1;
        }
    }
}