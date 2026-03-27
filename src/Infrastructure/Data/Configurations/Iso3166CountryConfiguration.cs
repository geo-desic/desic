using Desic.Domain.Iso3166Countries;
using Desic.Infrastructure.Data.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.Infrastructure.Data.Configurations;

internal class Iso3166CountryConfiguration(DatabaseFacade databaseFacade) : IEntityTypeConfiguration<Iso3166Country>
{
    private readonly DatabaseFacade _databaseFacade = databaseFacade ?? throw new ArgumentNullException(nameof(databaseFacade));

    public void Configure(EntityTypeBuilder<Iso3166Country> builder)
    {
        var schema = _databaseFacade.IsSqlite() ? null : ApplicationDbContext.RefSchema;
        builder.ToTable(nameof(ApplicationDbContext.Iso3166Countries), schema);
        var columnOrder = builder.ConfigureSeedableSoftDeletableEntity(_databaseFacade);
        builder.Property(x => x.IsoId).IsRequired().HasColumnOrder(++columnOrder);
        builder.Property(x => x.Alpha2).IsRequired().HasMaxLength(Iso3166Country.LengthAlpha2).HasColumnOrder(++columnOrder);
        builder.Property(x => x.Alpha3).IsRequired().HasMaxLength(Iso3166Country.LengthAlpha3).HasColumnOrder(++columnOrder);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(Iso3166Country.MaxLengthName).HasColumnOrder(++columnOrder);
        builder.HasIndex(x => x.IsoId).IsUnique();
        builder.HasIndex(x => x.Alpha2); // both alpha2 and alpha3 codes can potentially be re-used so their indexes are not defined as unique
        builder.HasIndex(x => x.Alpha3);
        builder.HasIndex(x => x.Name);
    }
}
