using System.Buffers.Binary;
using System.Runtime.InteropServices;

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

    extension(Guid source)
    {
        public static Guid CreateSequentialGuid(bool forSqlServer) => forSqlServer ? CreateSequentialGuidForSqlServer() : Guid.CreateVersion7();

        // source: https://github.com/dotnet/dotnet/blob/main/src/efcore/src/EFCore/ValueGeneration/SequentialGuidValueGenerator.cs
        public static Guid CreateSequentialGuidForSqlServer()
        {
            var guid = Guid.NewGuid();
            var ticks = DateTime.UtcNow.Ticks;
            var counter = BitConverter.IsLittleEndian
                ? Interlocked.Increment(ref ticks)
                : BinaryPrimitives.ReverseEndianness(Interlocked.Increment(ref ticks));
            var counterBytes = MemoryMarshal.AsBytes(new ReadOnlySpan<long>(in counter));
            var guidBytes = MemoryMarshal.AsBytes(new Span<Guid>(ref guid));
            guidBytes[08] = counterBytes[1];
            guidBytes[09] = counterBytes[0];
            guidBytes[10] = counterBytes[7];
            guidBytes[11] = counterBytes[6];
            guidBytes[12] = counterBytes[5];
            guidBytes[13] = counterBytes[4];
            guidBytes[14] = counterBytes[3];
            guidBytes[15] = counterBytes[2];
            return guid;
        }
    }
}