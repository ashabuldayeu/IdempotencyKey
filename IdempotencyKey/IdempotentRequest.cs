namespace IdempotencyKey
{
    public class IdempotentRequest : IIdempotentRequest
    {
        protected IdempotentRequest() { }
        public IdempotentRequest(Guid key, string clientName)
        {
            Key = key;
            ClientName = clientName;
            TStamp = DateTimeOffset.UtcNow;
        }

        public Guid Key { get; protected set; }

        public string ClientName { get; protected set; }

        public DateTimeOffset TStamp { get; protected set; }

        public string GetIdempotentKey() => $"{Key}_{ClientName}";

    }
}