namespace IdempotencyKey
{
    public interface IIdempotentRequest
    {
        Guid Key { get; }

        string ClientName { get; }

        DateTimeOffset TStamp { get; }

        string GetIdempotentKey();
    }
}
