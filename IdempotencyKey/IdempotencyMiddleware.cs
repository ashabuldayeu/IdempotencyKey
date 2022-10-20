using Microsoft.AspNetCore.Http;
using System.Text;

namespace IdempotencyKey
{
    public class IdempotencyMiddleware : IMiddleware
    {
        private readonly ICacheIdempotencyStorage _cacheProvider;
        private readonly IPersistentStorage _persistentStorage;

        public IdempotencyMiddleware(ICacheIdempotencyStorage dataProvider, IPersistentStorage persistentStorage)
        {
            _cacheProvider = dataProvider;
            _persistentStorage = persistentStorage;
        }
        // TODO : REFACTOR THIS METHOD
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string idempotencyHeader = context.Request.Headers["X-Idempotency-Key"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(idempotencyHeader))
            {
                await next(context);
                return;
            }

            Guid idempotencyHeaderGuid;
            if (!Guid.TryParse(idempotencyHeader, out idempotencyHeaderGuid))
            {
                // response just for sample
                context.Response.ContentType = "text/plain";
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("X-Idempotency-Key should be a valid UUID4");
            }
            else
            {
                IdempotentRequest request = new IdempotentRequest(idempotencyHeaderGuid, $"{context.Request.Method}_{context.Request.Path}");
                // if flag == created it mean that there were no such keys already stored
                bool isCreated = await _cacheProvider.CreateAsync(request);

                if (isCreated)
                {
                    // save income request to persistent storage
                    await _persistentStorage.AddIdempotentRequestAsync(request);

                    // not need to "using" this stream under control of context.response
                    // additional stream to allow save response body across pipeline
                    Stream responseStream = context.Response.Body;
                    using MemoryStream tempBodyStream = new MemoryStream();

                    context.Response.Body = tempBodyStream;

                    await next(context);

                    tempBodyStream.Position = 0;

                    // save request respnse for futher requests
                    await _persistentStorage.SaveResponseAsync(request.Key, await RequestResponse.CreateFromHttpResponseAsync(context.Response));

                    await tempBodyStream.CopyToAsync(responseStream);

                    context.Response.Body = responseStream;
                }
                else
                {
                    var savedResponse = await _persistentStorage.GetExistedResponseAsync(request.Key);
                    // if requests were in race condition and one marked as duplicate but first didn't finish yet we cannot return saved response
                    // so return Accepted code
                    if(savedResponse == null)
                    {
                        context.Response.ContentType = "text/plain";
                        context.Response.StatusCode = 201;
                        await context.Response.WriteAsync("Request already sent.");
                        return;
                    }
                    context.Response.ContentType = savedResponse.ContentType;
                    context.Response.StatusCode = savedResponse.StatusCode;
                    using MemoryStream savedBodyStream = new MemoryStream(Encoding.UTF8.GetBytes(savedResponse.Body));
                    await savedBodyStream.CopyToAsync(context.Response.Body);
                }
            }
        }

    }
}
