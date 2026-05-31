using DispatchR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Infrastructure.Data;

// see DbContextOptionsBuilder.UseSeeding at https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontextoptionsbuilder.useseeding?view=efcore-10.0
public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseApplicationDbContextSeeding(this DbContextOptionsBuilder options, IServiceProvider serviceProvider)
    {
        // this is needed because EF tooling does not (yet?) support async
        // see https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontextoptionsbuilder.useasyncseeding?view=efcore-10.0#microsoft-entityframeworkcore-dbcontextoptionsbuilder-useasyncseeding(system-func((microsoft-entityframeworkcore-dbcontext-system-boolean-system-threading-cancellationtoken-system-threading-tasks-task))):~:text=It%20is%20recomended%20to%20also%20call%20UseSeeding(Action%3CDbContext%2CBoolean%3E)%20with%20the%20same%20logic.
        options.UseSeeding((context, seed) =>
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            mediator.Send(new SeedApplicationDatabaseRequest(), cancellationToken: CancellationToken.None).GetAwaiter().GetResult();
        });
        options.UseAsyncSeeding(async (context, seed, cancellationToken) =>
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new SeedApplicationDatabaseRequest(), cancellationToken);
        });
        return options;
    }
}
