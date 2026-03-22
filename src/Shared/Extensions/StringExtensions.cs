namespace Desic.Shared.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Gets the leftmost <paramref name="length"/> characters from the input string.
    /// </summary>
    /// <param name="source">The input string.</param>
    /// <param name="length">The number of characters to retrieve from the left.</param>
    /// <returns>The entire string if <paramref name="length"/> is longer than the length of the input string, otherwise the leftmost <paramref name="length"/> characters from the input string.</returns>
    public static string? Left(this string? source, int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        if (source == null) return null;
        if (length >= source.Length) return source;
        return source[..length];
    }

    /// <summary>
    /// Gets the rightmost <paramref name="length"/> characters from the input string.
    /// </summary>
    /// <param name="source">The input string.</param>
    /// <param name="length">The number of characters to retrieve from the right.</param>
    /// <returns>The entire string if <paramref name="length"/> is longer than the length of the input string, otherwise the rightmost <paramref name="length"/> characters from the input string.</returns>
    public static string? Right(this string? source, int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        if (source == null) return null;
        if (length >= source.Length) return source;
        return source[^length..];
    }

    internal static Guid ToIntBasedGuid(this string guid, int value)
    {
        if (value < 0) value = -value;
        var valueString = value.ToString();
        var resultString = guid[..^valueString.Length] + valueString;
        return new Guid(resultString);
    }
}
