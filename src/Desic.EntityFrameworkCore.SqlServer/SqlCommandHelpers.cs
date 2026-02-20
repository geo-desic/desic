using Desic.Data;
using Microsoft.Data.SqlClient;

namespace Desic.EntityFrameworkCore.SqlServer;

public static class SqlCommandHelpers
{
    public static async Task<T?> ExecuteScalarAsyncAs<T>(this SqlCommand command, CancellationToken cancellationToken = default) where T : struct
    {
        return await DbCommandHelpers<SqlCommand>.ExecuteScalarAsyncReturning<T>(command, cancellationToken);
    }
}
