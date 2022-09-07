using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.CustomDefinitions;

public delegate bool IsActiveFightingStyleDelegate(RulesetCharacterHero character);

public sealed class FightingStyleDefinitionCustomizable : FightingStyleDefinition, ICustomFightingStyle
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
    FightingStyleDefinitionCustomizable,
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
