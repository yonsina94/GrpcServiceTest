using Microsoft.EntityFrameworkCore;
using GrpcServiceTest.Database.Entities;

namespace GrpcServiceTest.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<PersonEntity> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<PersonEntity>()
                .Property(p=>p.Name).IsRequired().HasMaxLength(80);

            builder.Entity<PersonEntity>()
                .Property(p => p.LastName).IsRequired().HasMaxLength(80);

            builder.Entity<PersonEntity>()
                .Property(p => p.Email).IsRequired().HasMaxLength(120);

            builder.Entity<PersonEntity>()
                .Property(p => p.Age).IsRequired();
        }
    }
}
