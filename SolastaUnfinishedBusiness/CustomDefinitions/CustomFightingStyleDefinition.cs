using System;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal delegate bool IsActiveFightingStyleDelegate(RulesetCharacterHero character);

internal sealed class CustomFightingStyleDefinition : FightingStyleDefinition, ICustomFightingStyle
{
    private IsActiveFightingStyleDelegate isActive;

    public bool IsActive(RulesetCharacterHero character)
    {
        return isActive == null || isActive(character);
    }

    internal void SetIsActiveDelegate(IsActiveFightingStyleDelegate del)
    {
        isActive = del;
    }
}

[UsedImplicitly]
internal class CustomizableFightingStyleBuilder : DefinitionBuilder<
    CustomFightingStyleDefinition,
    CustomizableFightingStyleBuilder>
{
    protected CustomizableFightingStyleBuilder(string name, Guid guid) : base(name, guid)
    {
    }

    [NotNull]
    internal CustomizableFightingStyleBuilder SetFeatures(params FeatureDefinition[] features)
    {
        Definition.Features.SetRange(features.OrderBy(f => f.Name));
        Definition.Features.Sort(Sorting.Compare);
        return this;
    }

#if false
    [NotNull]
    internal CustomizableFightingStyleBuilder SetIsActive(IsActiveFightingStyleDelegate del)
    {
        Definition.SetIsActiveDelegate(del);
        return this;
    }
#endif
}
