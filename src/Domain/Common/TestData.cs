using Desic.Shared.Extensions;

namespace Desic.Domain.Common;

public static class TestData
{
    internal const int GuidFlagCharacterIndex = 11;
    internal const char GuidFlagCharacterValue = 'f';

    public static Guid ToTestData(this Guid guid) => guid.ChangeGuidCharacter(value: GuidFlagCharacterValue, characterIndex: GuidFlagCharacterIndex);
}
