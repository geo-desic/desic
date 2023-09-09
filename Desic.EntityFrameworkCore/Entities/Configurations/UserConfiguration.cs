using Desic.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.EntityFrameworkCore.Entities.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        private readonly DatabaseFacade _db;

        public UserConfiguration(DatabaseFacade db)
        {
            _db = db;
        }

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.SequentialId, "UX_Users_SequentialId").IsUnique();
            builder.Property(x => x.CreatedOn).HasDefaultValueSql(_db.DateTimeUtc());
            builder.Property(x => x.ModifiedOn).HasDefaultValueSql(_db.DateTimeUtc());
            builder.Property(x => x.Hidden).HasDefaultValue(false);
            builder.HasIndex(x => x.Username, "UX_Users_Username").IsUnique();
        }
    }
}
