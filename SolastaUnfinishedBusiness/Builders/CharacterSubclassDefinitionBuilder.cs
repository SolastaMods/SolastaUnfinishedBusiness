using System;
using System.Linq;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class CharacterSubclassDefinitionBuilder
    : DefinitionBuilder<CharacterSubclassDefinition, CharacterSubclassDefinitionBuilder>
{
    public CharacterSubclassDefinitionBuilder AddFeatureAtLevel(FeatureDefinition feature, int level)
    {
        Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(feature, level));
        Definition.FeatureUnlocks.Sort(Sorting.Compare);
        return this;
    }

    public CharacterSubclassDefinitionBuilder AddFeaturesAtLevel(int level, params FeatureDefinition[] features)
    {
        Definition.FeatureUnlocks.AddRange(features.Select(f => new FeatureUnlockByLevel(f, level)));
        Definition.FeatureUnlocks.Sort(Sorting.Compare);
        return this;
    }

    #region Constructors

    protected CharacterSubclassDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected CharacterSubclassDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected CharacterSubclassDefinitionBuilder(CharacterSubclassDefinition original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected CharacterSubclassDefinitionBuilder(CharacterSubclassDefinition original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
