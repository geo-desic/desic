using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Desic.Infrastructure.Data.Common.Extensions;

internal static class ModelBuilderExtensions
{
    public static void SetUtcValueConverterForAllDateTimeProperties(this ModelBuilder modelBuilder)
    {
        var utcConverter = new ValueConverter<DateTime, DateTime>(
            v => v, // when saving: no change
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // when loading: specify date as utc
        );

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(utcConverter);
                }
            }
        }
    }
}
