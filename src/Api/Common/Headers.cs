namespace Desic.Api.Common;

public static class Headers
{
    public static bool PreferRepresentation(string? preferHeaderValue)
    {
        return "return=representation".Equals(preferHeaderValue, StringComparison.OrdinalIgnoreCase);
    }

    public static void AddResponseHeaderEntityId(this HttpContext httpContext, Guid entityId)
    {
        httpContext.Response.Headers.Append("Entity-Id", entityId.ToString());
    }
}
