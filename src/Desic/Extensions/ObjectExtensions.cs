namespace Desic.Extensions;

public static class ObjectExtensions
{
    public static bool NullablyEquivalentTo(this object? source, object? target)
    {
        return (source == null && target == null) || (source != null && target != null);
    }
}
