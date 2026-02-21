using Desic.Data.Entities;
using Desic.EntityFrameworkCore.Entities.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.EntityFrameworkCore.Entities.Configurations;

internal class TagConfiguration(DatabaseFacade databaseFacade) : IEntityTypeConfiguration<Tag>
{
    private readonly DatabaseFacade _databaseFacade = databaseFacade ?? throw new ArgumentNullException(nameof(databaseFacade));

    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        var columnOrder = builder.ConfigureSoftDeletableEntity(_databaseFacade);
        builder.Property(x => x.Name).IsRequired().HasColumnOrder(columnOrder++);
        builder.HasIndex(x => x.Name).IsUnique(false);
    }
}
