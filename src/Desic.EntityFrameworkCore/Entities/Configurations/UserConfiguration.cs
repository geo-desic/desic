using Desic.Data.Entities;
using Desic.EntityFrameworkCore.Entities.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.EntityFrameworkCore.Entities.Configurations;

internal class UserConfiguration(DatabaseFacade databaseFacade) : IEntityTypeConfiguration<User>
{
    private readonly DatabaseFacade _databaseFacade = databaseFacade ?? throw new ArgumentNullException(nameof(databaseFacade));

    public void Configure(EntityTypeBuilder<User> builder)
    {
        var columnOrder = builder.ConfigureSoftDeletableEntity(_databaseFacade);
        builder.Property(x => x.Username).IsRequired().HasMaxLength(100).HasColumnOrder(columnOrder++);
        builder.Property(x => x.IsActive).HasDefaultValue(true).IsRequired().HasColumnOrder(columnOrder++);
        builder.HasIndex(x => x.Username).IsUnique();
    }
}
