using Desic.EntityFrameworkCore.Models;

namespace Desic.EntityFrameworkCore.Data
{
    internal static class Seed
    {
        public static async Task ApplyAsync(DesicContext db, CancellationToken cancellationToken)
        {
            var hasDml = false;

            if (!db.EntityTypes.Any())
            {
                hasDml = true;
                db.EntityTypes.AddRange(Data.EntityTypes.Generate());
            }

            if (!db.Tags.Any())
            {
                hasDml = true;
                db.Tags.AddRange(Data.Tags.Generate());
            }

            if (!db.Users.Any())
            {
                hasDml = true;
                db.Users.AddRange(Data.Test.Users.Generate());
            }

            if (hasDml)
            {
                await db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
