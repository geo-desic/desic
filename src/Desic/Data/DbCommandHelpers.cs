using System.Data.Common;

namespace Desic.Data;

public static class DbCommandHelpers<T> where T : DbCommand
{
    public static T AddParameterWithValue(T command, string parameterName, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = parameterName;
        parameter.Value = value;
        command.Parameters.Add(parameter);
        return command;
    }

    public static async Task<V?> ExecuteScalarAsyncReturning<V>(T command, CancellationToken cancellationToken = default) where V : struct
    {
        var result = await command.ExecuteScalarAsync(cancellationToken);
        if (result == null || result == DBNull.Value) return null;
        if (result is V v) return v;
        return (V)Convert.ChangeType(result, typeof(V));
    }

    public static async Task<string?> ExecuteScalarAsyncReturningString(T command, CancellationToken cancellationToken = default)
    {
        var result = await command.ExecuteScalarAsync(cancellationToken);
        if (result == null || result == DBNull.Value) return null;
        return result as string;
    }
}
