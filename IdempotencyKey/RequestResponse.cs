namespace IdempotencyKey
{
    public class RequestResponse
    {
        public int StatusCode { get; set; }

        public string Body { get; set; }

        public long ContentLength { get; set; }
        public string ContentType { get; set; }
    }
}
