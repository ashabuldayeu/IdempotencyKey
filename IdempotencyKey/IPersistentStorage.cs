namespace IdempotencyKey
{
    public interface IPersistentStorage
    {
        Task AddIdempotentRequestAsync(IdempotentRequest request);

        Task<RequestResponse> GetExistedResponseAsync(Guid key);

        Task<IdempotentRequest> GetRequestAsync(Guid key);

        Task SaveResponseAsync(Guid key, RequestResponse value);
    }
}
