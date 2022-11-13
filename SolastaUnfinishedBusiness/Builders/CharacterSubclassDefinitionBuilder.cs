using System;
using System.Linq;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class CharacterSubclassDefinitionBuilder
    : DefinitionBuilder<CharacterSubclassDefinition, CharacterSubclassDefinitionBuilder>
{
    internal CharacterSubclassDefinitionBuilder AddFeaturesAtLevel(int level, params FeatureDefinition[] features)
    {
        Definition.FeatureUnlocks.AddRange(features.Select(f => new FeatureUnlockByLevel(f, level)));
        return this;
    }

    #region Constructors

    protected CharacterSubclassDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected CharacterSubclassDefinitionBuilder(CharacterSubclassDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
