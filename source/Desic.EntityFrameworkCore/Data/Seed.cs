using Desic.EntityFrameworkCore.Models;

namespace Desic.EntityFrameworkCore.Data;

internal static class Seed
{
    public static async Task ApplyAsync(DesicContext db, CancellationToken cancellationToken)
    {
        if (!db.EntityTypes.Any())
        {
            db.EntityTypes.AddRange(EntityTypes.Generate());
            await db.SaveChangesAsync(cancellationToken);
            db.ChangeTracker.Clear(); // EF foreign key issues occur if this is not done after adding the entity types to the db
        }

        var saveChanges = false;

        if (!db.Tags.Any())
        {
            saveChanges = true;
            db.Tags.AddRange(Tags.Generate());
        }

        if (!db.Users.Any())
        {
            saveChanges = true;
            db.Users.AddRange(Test.Users.Generate());
        }

        if (saveChanges)
        {
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
