using Desic.EntityFrameworkCore.Entities.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.EntityFrameworkCore.Entities
{
    internal class TagConfiguration(DatabaseFacade db) : IEntityTypeConfiguration<Tag>
    {
        private readonly DatabaseFacade _db = db;

        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            var columnOrder = builder.ConfigureModifiableEntity(_db);
            builder.Property(x => x.Name).IsRequired().HasColumnOrder(columnOrder++);
            builder.HasIndex(x => x.Name);
        }
    }
}
