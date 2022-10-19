using IdempotencyKey;
using IdempotencyKey.PersistentStorage;
using IdempotencyKey.Redis;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// hardoced for tests only
ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
builder.Services.AddDbContext<IdempotencyDbContext>(o => o.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=IdempotencyDb;Trusted_Connection=True;"));
builder.Services.AddSingleton(redis);
builder.Services.AddSingleton(redis.GetDatabase());
builder.Services.AddScoped<ICacheIdempotencyStorage, RedisIdempotencyStorage>();
builder.Services.AddScoped<IPersistentStorage, PersistentStorage>();
builder.Services.AddScoped<IdempotencyMiddleware>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<IdempotencyMiddleware>();

app.MapControllers();

app.Run();
