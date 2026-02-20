namespace Desic.EntityFrameworkCore.Models;

internal class DbSetSeedingResult
{
    public long Deletes { get; set; } = 0;
    public long Inserts { get; set; } = 0L;
    public long ReferenceCount { get; set; } = 0;
    public long Updates { get; set; } = 0L;
}
