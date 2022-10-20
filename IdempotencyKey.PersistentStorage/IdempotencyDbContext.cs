using Microsoft.EntityFrameworkCore;

namespace IdempotencyKey.PersistentStorage
{
    public class IdempotencyDbContext : DbContext
    {
        public IdempotencyDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<IdempotentRequest> IdempotentRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.ApplyConfiguration(new IdempotentRequestEntityConfigurator());
            base.OnModelCreating(modelBuilder);
        }
    }
}
