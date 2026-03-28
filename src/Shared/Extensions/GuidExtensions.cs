namespace Desic.Shared.Extensions;

public static class GuidExtensions
{
    public static Guid ChangeGuidCharacter(this Guid guid, char value, int characterIndex)
    {
        return guid.ToString("N").ChangeGuidCharacter(value: value, characterIndex: characterIndex);
    }

    public static Guid ToIntBasedGuid(this Guid guid, int value)
    {
        return guid.ToString().ToIntBasedGuid(value);
    }

    // source: https://github.com/bitfoundation/bitplatform/blob/develop/src/Templates/Boilerplate/Bit.Boilerplate/src/Shared/Infrastructure/Extensions/GuidExtensions.cs
    extension(Guid source)
    {
        public static Guid CreateSequentialGuid(bool alterForSqlServer)
        {
            Guid standardV7 = Guid.CreateVersion7();

            if (!alterForSqlServer) return standardV7;

            Span<byte> bytes = stackalloc byte[16];
            standardV7.TryWriteBytes(bytes, bigEndian: true, out _);

            // Version 7 structure (big-endian):
            // bytes[0-5]: 48-bit Unix timestamp in milliseconds
            // bytes[6-7]: 4-bit version (0111) + 12-bit random
            // bytes[8-15]: 2-bit variant (10) + 62-bit random

            Span<byte> sqlBytes = stackalloc byte[16];

            /* SQL SERVER SORTING PRIORITY:
               1st: Bytes 10-15 (Must contain the most significant Time bits)
               2nd: Bytes 8-9
               3rd: Bytes 6-7
               4th-6th: Bytes 0-5 (Least significant)
            */

            // Move the 48-bit Timestamp (6 bytes) to the very end (10-15)
            // This makes the timestamp the PRIMARY sorting criteria for SQL Server
            bytes[..6].CopyTo(sqlBytes.Slice(10, 6));

            // Move Version, Variant, and Random bytes to the beginning (0-9)
            // These will only be compared if the timestamp is identical
            bytes.Slice(6, 10).CopyTo(sqlBytes[..10]);

            // Use bigEndian: true because we manually laid out the bytes
            // for SQL Server's internal storage format.
            return new Guid(sqlBytes, bigEndian: true);
        }
    }
}