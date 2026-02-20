using System.Data.Common;

namespace Desic.Data;

public static class DbCommandExtensions
{
    public static T AddParameterWithValue<T>(this T command, string parameterName, object value) where T : DbCommand
    {
        return DbCommandHelpers<T>.AddParameterWithValue(command, parameterName, value);
    }

    public static async Task<string?> ExecuteScalarAsyncReturningString<T>(this T command, CancellationToken cancellationToken = default) where T : DbCommand
    {
        return await DbCommandHelpers<T>.ExecuteScalarAsyncReturningString(command, cancellationToken);
    }
}
