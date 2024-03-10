using Desic.EntityFrameworkCore.Entities.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.EntityFrameworkCore.Entities.Configurations
{
    internal class UserConfiguration(DatabaseFacade db) : IEntityTypeConfiguration<User>
    {
        private readonly DatabaseFacade _db = db;

        public void Configure(EntityTypeBuilder<User> builder)
        {
            var columnOrder = builder.ConfigureModifiableEntity(_db);
            builder.Property(x => x.Username).IsRequired().HasColumnOrder(columnOrder++);
            builder.Property(x => x.IsActive).HasDefaultValue(true).IsRequired().HasColumnOrder(columnOrder++);
            builder.Property(x => x.IsHidden).HasDefaultValue(false).IsRequired().HasColumnOrder(columnOrder++);
            builder.HasIndex(x => x.Username).IsUnique();
        }
    }
}
