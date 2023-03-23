using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionBonusCantripsBuilder
    : DefinitionBuilder<FeatureDefinitionBonusCantrips, FeatureDefinitionBonusCantripsBuilder>
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

    internal FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
