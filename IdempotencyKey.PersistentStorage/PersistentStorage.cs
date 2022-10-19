namespace IdempotencyKey.PersistentStorage
{
    public class PersistentStorage : IPersistentStorage
    {
        private readonly IdempotencyDbContext _idempotencyDb;

        public PersistentStorage(IdempotencyDbContext idempotencyDb)
        {
            _idempotencyDb = idempotencyDb;
        }

        public async Task AddIdempotentRequestAsync(IdempotentRequest request)
        {
            _idempotencyDb.Add(request);
            await _idempotencyDb.SaveChangesAsync();
        }

        public async Task<RequestResponse> GetExistedResponseAsync(Guid key)
        {
            var request = await _idempotencyDb.IdempotentRequests.FindAsync(key);
            RequestResponse response = new RequestResponse();
            response.Body = _idempotencyDb.Entry(request).Property<string>("Response").CurrentValue;
            response.ContentLength = _idempotencyDb.Entry(request).Property<long>("ContentLength").CurrentValue;
            response.ContentType = _idempotencyDb.Entry(request).Property<string>("ContentType").CurrentValue;
            response.StatusCode = _idempotencyDb.Entry(request).Property<int>("StatusCode").CurrentValue;

            return response;
        }

        public async Task<IdempotentRequest> GetRequestAsync(Guid key)
        {
            return await _idempotencyDb.IdempotentRequests.FindAsync(key);
        }

        public async Task SaveResponseAsync(Guid key, RequestResponse value)
        {
            var request = await _idempotencyDb.IdempotentRequests.FindAsync(key);

            _idempotencyDb.Entry(request).Property<string>("Response").CurrentValue = value.Body;
            _idempotencyDb.Entry(request).Property<int>("StatusCode").CurrentValue = value.StatusCode;
            _idempotencyDb.Entry(request).Property<long>("ContentLength").CurrentValue = value.ContentLength;
            _idempotencyDb.Entry(request).Property<string>("ContentType").CurrentValue = value.ContentType;

            await _idempotencyDb.SaveChangesAsync();
        }
    }
}
