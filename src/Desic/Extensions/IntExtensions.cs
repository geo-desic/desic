namespace Desic.Extensions;

public static class IntExtensions
{
    public static Guid ToGuid(this int value)
    {
        return Guid.Empty.ToIntBasedGuid(value);
    }
}
