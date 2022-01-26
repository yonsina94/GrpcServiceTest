using Microsoft.EntityFrameworkCore;
using GrpcServiceTest.Database.Entities;
using GrpcServiceTest.Database.Entities.Base;

namespace GrpcServiceTest.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<PersonEntity> Persons { get; set; }

        /* protected override void OnModelCreating(ModelBuilder builder)
         {
             builder.Entity<PersonEntity>()
                 .Property(p=>p.Name).IsRequired().HasMaxLength(80);

             builder.Entity<PersonEntity>()
                 .Property(p => p.LastName).IsRequired().HasMaxLength(80);

             builder.Entity<PersonEntity>()
                 .Property(p => p.Email).IsRequired().HasMaxLength(120);

             builder.Entity<PersonEntity>()
                 .Property(p => p.Age).IsRequired();
         } */

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(
           bool acceptAllChangesOnSuccess,
           CancellationToken cancellationToken = default(CancellationToken)
        )
        {
            OnBeforeSaving();
            return (await base.SaveChangesAsync(acceptAllChangesOnSuccess,
                          cancellationToken));
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            var utcNow = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                // for entities that inherit from BaseEntity,
                // set UpdatedOn / CreatedOn appropriately
                if (entry.Entity is BaseEntity trackable)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            // set the updated date to "now"
                            trackable.UpdatedAt = utcNow;

                            // mark property as "don't touch"
                            // we don't want to update on a Modify operation
                            entry.Property("CreatedAt").IsModified = false;
                            break;

                        case EntityState.Added:
                            // set both updated and created date to "now"
                            trackable.CreatedAt = utcNow;
                            trackable.UpdatedAt = utcNow;
                            break;
                    }
                }
            }
        }
    }
}
