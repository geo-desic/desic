using Desic.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desic.EntityFrameworkCore.Entities
{
    internal class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        private readonly DatabaseFacade _db;

        public TagConfiguration(DatabaseFacade db)
        {
            _db = db;
        }

        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasKey(x => x.Id);
            //builder.HasIndex(x => x.SequentialId, "UX_Tags_SequentialId").IsUnique();
            builder.HasIndex(x => x.Name, "UX_Tags_Name");
            builder.Property(x => x.CreatedById).IsRequired();
            builder.Property(x => x.CreatedByTypeId).IsRequired();
            builder.Property(x => x.CreatedOn).HasDefaultValueSql(_db.DateTimeUtc()).IsRequired();
            builder.Property(x => x.ModifiedById).IsRequired();
            builder.Property(x => x.ModifiedByTypeId).IsRequired();
            builder.Property(x => x.ModifiedOn).HasDefaultValueSql(_db.DateTimeUtc()).IsRequired();
            builder.Property(x => x.Name).IsRequired();
        }
    }
}
