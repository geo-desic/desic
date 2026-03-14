namespace Desic.Api.Common;

public static class Headers
{
    public static void AddResponseHeaderEntityId(this HttpContext httpContext, Guid entityId)
    {
        httpContext.Response.Headers.Append(Keys.EntityId, entityId.ToString());
    }

    public static class Keys
    {
        public const string EntityId = "Entity-Id";
        public const string Prefer = nameof(Prefer);
    }

    public static class Values
    {
        public const string PreferRepresentation = "return=representation";

        public static bool IsPreferRepresentation(string? preferHeaderValue)
        {
            return PreferRepresentation.Equals(preferHeaderValue, StringComparison.OrdinalIgnoreCase);
        }
    }
}
