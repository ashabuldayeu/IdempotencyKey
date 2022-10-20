using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdempotencyKey.PersistentStorage
{
    public class IdempotentRequestEntityConfigurator : IEntityTypeConfiguration<IdempotentRequest>
    {
        public void Configure(EntityTypeBuilder<IdempotentRequest> builder)
        {
            builder.HasKey(b => b.Key);
            
            builder.Property(b => b.ClientName);
            builder.Property(b => b.TStamp);

            builder.Property<string>("Response");
            builder.Property<int>("StatusCode");
            builder.Property<long>("ContentLength");
            builder.Property<string>("ContentType");
        }
    }
}
