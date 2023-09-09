using Desic.EntityFrameworkCore.Entities;
using Desic.EntityFrameworkCore.Entities.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Desic.EntityFrameworkCore.Models
{
    public class DesicContext : DbContext
    {
        public DesicContext(DbContextOptions<DesicContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("app");
            modelBuilder.ApplyConfiguration(new UserConfiguration(Database));
        }

        public static async Task InitializeAsync(DesicContext db)
        {
            await db.Database.MigrateAsync();
            if (db.Users.Any()) return;

            db.Users.AddRange(GenerateSeedDataUsers());
            await db.SaveChangesAsync();
        }

        private const int SeedDataCount = 10;

        private static IEnumerable<User> GenerateSeedDataUsers(int count = SeedDataCount)
        {
            var result = new List<User>();
            var random = new Random();
            for (var i = 0; i < count; ++i)
            {
                const int maxSeconds = 725328000;
                var randomSeconds = random.Next(maxSeconds);
                var createdOn = new DateTime(2000, 1, 1).AddSeconds(randomSeconds);
                result.Add(new User
                {
                    Id = Guid.NewGuid(),
                    SequentialId = i + 1,
                    CreatedOn = createdOn,
                    CreatedBy = "system",
                    ModifiedOn = random.Next(2) == 0 ? createdOn : createdOn.AddSeconds(random.Next(maxSeconds - randomSeconds)),
                    ModifiedBy = "system",
                    Username = $"user{i + 1}",
                });
            }
            return result;
        }
    }
}