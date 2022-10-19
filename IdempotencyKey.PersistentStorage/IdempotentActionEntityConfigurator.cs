using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace IdempotencyKey.PersistentStorage
{
    internal class IdempotentActionEntityConfigurator : IEntityTypeConfiguration<IdempotentAction>
    {
        public void Configure(EntityTypeBuilder<IdempotentAction> builder)
        {
            //builder.HasKey(b => b.Key);

            //builder.Property(b => b.Response).HasConversion(
            //    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
            //    v => JsonSerializer.Deserialize<object>(v, new JsonSerializerOptions()));
        }
    }
}
