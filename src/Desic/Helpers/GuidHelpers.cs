namespace Desic.Helpers;

public static class GuidHelpers
{
    public static Guid ToGuid(this int value)
    {
        return Guid.Empty.ToIntBasedGuid(value);
    }

    public static Guid ToIntBasedGuid(this Guid guid, int value)
    {
        return guid.ToString().ToIntBasedGuid(value);
    }

    public static Guid ToIntBasedGuid(this string guid, int value)
    {
        if (value < 0) value = -value;
        var valueString = value.ToString();
        var resultString = guid[..^valueString.Length] + valueString;
        return new Guid(resultString);
    }
}
