using Desic.Domain.Processes;
using Desic.Infrastructure.Data.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.Infrastructure.Data.Configurations;

public class ProcessConfiguration(DatabaseFacade databaseFacade) : IEntityTypeConfiguration<Process>
{
    private readonly DatabaseFacade _databaseFacade = databaseFacade ?? throw new ArgumentNullException(nameof(databaseFacade));

    public void Configure(EntityTypeBuilder<Process> builder)
    {
        var columnOrder = builder.ConfigureSoftDeletableEntity(_databaseFacade);
        builder.Property(x => x.StartedOn).IsRequired(false).HasColumnOrder(++columnOrder);
        builder.Property(x => x.CompletedOn).IsRequired(false).HasColumnOrder(++columnOrder);
        builder.Property(x => x.FaileddOn).IsRequired(false).HasColumnOrder(++columnOrder);
        builder.Property(x => x.Message).IsRequired(false).HasMaxLength(Process.MaxLengthMessage).HasColumnOrder(++columnOrder);
        builder.HasIndex(x => x.StartedOn).IsUnique(false);
        builder.HasIndex(x => x.CompletedOn).IsUnique(false);
        builder.HasIndex(x => x.FaileddOn).IsUnique(false);
    }
}