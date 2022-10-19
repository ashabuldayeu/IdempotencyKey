namespace IdempotencyKey
{
    public interface ICacheIdempotencyStorage
    {
        Task<bool> CreateAsync(IIdempotentRequest request);
    }
}
