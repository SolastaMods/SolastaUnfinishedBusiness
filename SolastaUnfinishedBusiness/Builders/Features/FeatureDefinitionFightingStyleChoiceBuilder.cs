using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionFightingStyleChoiceBuilder
    : DefinitionBuilder<FeatureDefinitionFightingStyleChoice, FeatureDefinitionFightingStyleChoiceBuilder>
{
#if false
    internal FeatureDefinitionFightingStyleChoiceBuilder SetFightingStyles(
        params string[] fightingStyles)
    {
        Definition.FightingStyles.SetRange(fightingStyles);
        return this;
    }
#endif

    #region Constructors

    protected FeatureDefinitionFightingStyleChoiceBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionFightingStyleChoiceBuilder(FeatureDefinitionFightingStyleChoice original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
