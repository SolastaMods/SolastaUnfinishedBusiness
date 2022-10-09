using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionBonusCantripsBuilder : FeatureDefinitionBuilder<
    FeatureDefinitionBonusCantrips, FeatureDefinitionBonusCantripsBuilder>
{
    internal FeatureDefinitionBonusCantripsBuilder SetBonusCantrips(params SpellDefinition[] spellDefinitions)
    {
        Definition.BonusCantrips.SetRange(spellDefinitions);
        Definition.BonusCantrips.Sort(Sorting.Compare);
        return this;
    }

    #region Constructors

    internal FeatureDefinitionBonusCantripsBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal FeatureDefinitionBonusCantripsBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    internal FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    internal FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
