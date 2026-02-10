using Desic.EntityFrameworkCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.EntityFrameworkCore.Entities.Configurations;

internal class Iso3166CountryConfiguration(DatabaseFacade databaseFacade) : IEntityTypeConfiguration<Iso3166Country>
{
    private readonly DatabaseFacade _databaseFacade = databaseFacade ?? throw new ArgumentNullException(nameof(databaseFacade));

    public void Configure(EntityTypeBuilder<Iso3166Country> builder)
    {
        var columnOrder = 0;
        builder.Property(x => x.IsoId).IsRequired().HasColumnOrder(columnOrder++);
        builder.HasKey(x => x.IsoId);
        builder.Property(x => x.Alpha2).IsRequired().HasColumnOrder(columnOrder++);
        builder.Property(x => x.Alpha3).IsRequired().HasColumnOrder(columnOrder++);
        builder.HasIndex(x => x.Alpha2).IsUnique();
        builder.HasIndex(x => x.Alpha3).IsUnique();
        builder.HasData(Iso3166Countries.Generate()); // this is model managed data that will automatically be created/updated/deleted during ef migrations; do not also include this data in any separate seeding functionality
    }
}
