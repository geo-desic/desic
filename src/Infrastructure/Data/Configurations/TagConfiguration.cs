using Desic.Domain.Tags;
using Desic.Infrastructure.Data.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.Infrastructure.Data.Configurations;

internal class TagConfiguration(DatabaseFacade databaseFacade) : IEntityTypeConfiguration<Tag>
{
    private readonly DatabaseFacade _databaseFacade = databaseFacade ?? throw new ArgumentNullException(nameof(databaseFacade));

    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        var schema = _databaseFacade.SupportsSchemas() ? ApplicationDbContext.AppSchema : null;
        builder.ToTable(nameof(ApplicationDbContext.Tags), schema);
        var columnOrder = builder.ConfigureSoftDeletableEntity(_databaseFacade);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(Tag.MaxLengthName).HasColumnOrder(++columnOrder);
        builder.Property(x => x.Value).HasMaxLength(Tag.MaxLengthValue).HasColumnOrder(++columnOrder);
        builder.HasIndex(x => x.Name).IsUnique(false);
        builder.HasIndex(x => x.Value).IsUnique(false);
    }
}
