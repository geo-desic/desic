namespace Desic.Core.Helpers;

public static class NullableHelpers
{
    public static bool NullablyEquivalentTo(this object? source, object? target)
    {
        return (source == null && target == null) || (source != null && target != null);
    }
}
