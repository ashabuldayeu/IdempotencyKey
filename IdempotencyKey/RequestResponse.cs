using Microsoft.AspNetCore.Http;

namespace IdempotencyKey
{
    public class RequestResponse
    {
        public RequestResponse()
        {
        }

        public int StatusCode { get; set; }

        public string Body { get; set; }

        public long ContentLength { get; set; }

        public string ContentType { get; set; }

        public static async Task<RequestResponse> CreateFromHttpResponseAsync(HttpResponse response)
        {
            var createdResponse = new RequestResponse()
            {
                Body = await ReadResponseBodyAsync(response),
                ContentLength = response.ContentLength.GetValueOrDefault(),
                ContentType = response.ContentType,
                StatusCode = response.StatusCode
            };

            return createdResponse;
        }

        private static async Task<string> ReadResponseBodyAsync(HttpResponse response)
        {
            string respnseContent = "";
            // we do not want to dispose response body
            StreamReader streamReader = new StreamReader(response.Body);
            respnseContent = await streamReader.ReadToEndAsync();
            response.Body.Position = 0;
            return respnseContent;
        }
    }
}
