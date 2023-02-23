#if false
using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class
    CharacterBackgroundDefinitionBuilder : DefinitionBuilder<CharacterBackgroundDefinition,
        CharacterBackgroundDefinitionBuilder>
{
    //
    // TODO: add builder methods as they get required
    //

    internal CharacterBackgroundDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal CharacterBackgroundDefinitionBuilder(CharacterBackgroundDefinition original, string name,
        Guid namespaceGuid) : base(original,
        name, namespaceGuid)
    {
    }
}
#endif
