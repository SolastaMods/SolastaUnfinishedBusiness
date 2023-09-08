#if false
using System;
using System.Linq;
using JetBrains.Annotations;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class DeityDefinitionBuilder : DefinitionBuilder<DeityDefinition, DeityDefinitionBuilder>
{
    internal DeityDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal DeityDefinitionBuilder(DeityDefinition original, string name, Guid namespaceGuid) : base(original,
        name, namespaceGuid)
    {
    }

    internal DeityDefinitionBuilder SetAlignment(string alignment)
    {
        Definition.alignment = alignment;
        return this;
    }

    internal DeityDefinitionBuilder SetSubClasses(params CharacterSubclassDefinition[] characterSubclassDefinitions)
    {
        Definition.subclasses.AddRange(characterSubclassDefinitions.Select(x => x.Name));
        return this;
    }
}
#endif
