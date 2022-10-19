namespace IdempotencyKey.PersistentStorage
{
    public class IdempotentAction : IdempotentRequest, IIdempotentRequest
    {
        protected IdempotentAction() : base()
        {

        }
        public IdempotentAction(Guid key, string clientName) : base(key, clientName)
        {
        }

        public object Response { get; set; }
    }
}
