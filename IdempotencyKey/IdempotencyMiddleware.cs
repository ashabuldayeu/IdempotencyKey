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
            }
            else
            {
                Guid idempotencyHeaderGuid;
                if (!Guid.TryParse(idempotencyHeader, out idempotencyHeaderGuid))
                {
                    context.Response.ContentType = "text/plain";
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("X-Idempotency-Key should be a valid UUID4");
                }
                else
                {
                    var request = new IdempotentRequest(idempotencyHeaderGuid, $"{context.Request.Method}_{context.Request.Path}");
                    bool isCreated = await _cacheProvider.CreateAsync(request);

                    if (isCreated)
                    {
                        try
                        {
                            await _persistentStorage.AddIdempotentRequestAsync(request);
                            Stream str = context.Response.Body;
                            using MemoryStream memoryStream = new MemoryStream();
                            context.Response.Body = memoryStream;
                            await next(context);
                            memoryStream.Position = 0;
                            using StreamReader streamReader = new StreamReader(memoryStream);
                            var cont = await streamReader.ReadToEndAsync();
                            await _persistentStorage.SaveResponseAsync(request.Key, new RequestResponse()
                            {
                                Body = cont,
                                ContentLength = context.Response.ContentLength.GetValueOrDefault(),
                                ContentType = context.Response.ContentType,
                                StatusCode = context.Response.StatusCode
                            });
                            memoryStream.Position = 0;

                            await memoryStream.CopyToAsync(str);

                            context.Response.Body = str;
                        }
                        catch (Exception e)
                        {

                            throw;
                        }
                    }
                    else
                    {
                        var savedResponse = await _persistentStorage.GetExistedResponseAsync(request.Key);
                        context.Response.ContentType = savedResponse.ContentType;
                        context.Response.StatusCode = savedResponse.StatusCode;
                        using MemoryStream sr = new MemoryStream(Encoding.UTF8.GetBytes(savedResponse.Body));
                        await sr.CopyToAsync(context.Response.Body);
                    }
                }
            }
        }
    }
}
