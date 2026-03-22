using Desic.Domain.Users;
using Desic.Infrastructure.Data.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.Infrastructure.Data.Configurations;

internal class UserConfiguration(DatabaseFacade databaseFacade) : IEntityTypeConfiguration<User>
{
    private readonly DatabaseFacade _databaseFacade = databaseFacade ?? throw new ArgumentNullException(nameof(databaseFacade));

    public void Configure(EntityTypeBuilder<User> builder)
    {
        var columnOrder = builder.ConfigureSoftDeletableEntity(_databaseFacade);
        builder.Property(x => x.Username).IsRequired().HasMaxLength(User.MaxLengthUsername).HasColumnOrder(++columnOrder);
        builder.Property(x => x.IsActive).HasDefaultValue(true).IsRequired().HasColumnOrder(++columnOrder);
        builder.HasIndex(x => x.Username).IsUnique();
    }
}
