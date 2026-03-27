using Desic.Domain.Labels;
using Desic.Infrastructure.Data.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.Infrastructure.Data.Configurations;

public class LabelConfiguration(DatabaseFacade databaseFacade) : IEntityTypeConfiguration<Label>
{
    private readonly DatabaseFacade _databaseFacade = databaseFacade ?? throw new ArgumentNullException(nameof(databaseFacade));

    public void Configure(EntityTypeBuilder<Label> builder)
    {
        var schema = _databaseFacade.IsSqlite() ? null : ApplicationDbContext.AppSchema;
        builder.ToTable(nameof(ApplicationDbContext.Labels), schema);
        var columnOrder = builder.ConfigureSoftDeletableEntity(_databaseFacade);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(Label.MaxLengthName).HasColumnOrder(++columnOrder);
        builder.HasIndex(x => x.Name).IsUnique(false);
    }
}
