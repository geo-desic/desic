namespace Desic.Shared.Extensions;

public static class StringExtensions
{
    internal static Guid ToIntBasedGuid(this string guid, int value)
    {
        if (value < 0) value = -value;
        var valueString = value.ToString();
        var resultString = guid[..^valueString.Length] + valueString;
        return new Guid(resultString);
    }
}
