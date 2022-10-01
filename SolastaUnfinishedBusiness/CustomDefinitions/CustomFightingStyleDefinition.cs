using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

public delegate bool IsActiveFightingStyleDelegate(RulesetCharacterHero character);

public sealed class CustomFightingStyleDefinition : FightingStyleDefinition, ICustomFightingStyle
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

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class CustomizableFightingStyleBuilder : FightingStyleDefinitionBuilder<
    CustomFightingStyleDefinition,
    CustomizableFightingStyleBuilder>
{
    protected CustomizableFightingStyleBuilder(string name, string guid) : base(name, guid)
    {
    }

    protected CustomizableFightingStyleBuilder(string name, Guid guid) : base(name, guid)
    {
    }

    [NotNull]
    public CustomizableFightingStyleBuilder SetIsActive(IsActiveFightingStyleDelegate del)
    {
        Definition.SetIsActiveDelegate(del);
        return this;
    }
}
