using System;
using System.Linq;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FightingStyleDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Paladin;

internal class OathOfRetribution : AbstractSubclass
{
    private static readonly Guid SubclassNamespace = new("f5efd735-ff95-4256-ad17-dde585aeb5f3");
    private readonly CharacterSubclassDefinition Subclass;

    internal OathOfRetribution()
    {
        var paladinOathOfRetributionAutoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("PaladinOathOfRetributionAutoPreparedSpells", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Paladin)
            .SetPreparedSpellGroups(
                AutoPreparedSpellsGroupBuilder.BuildSpellGroup(3, SpellDefinitions.Bane, SpellDefinitions.HuntersMark),
                AutoPreparedSpellsGroupBuilder.BuildSpellGroup(5, SpellDefinitions.HoldPerson, SpellDefinitions.MistyStep),
                AutoPreparedSpellsGroupBuilder.BuildSpellGroup(6, SpellDefinitions.Haste, SpellDefinitions.ProtectionFromEnergy))
            .AddToDB();

        var conditionFrightenedZealousAccusation = ConditionDefinitionBuilder
            .Create("ConditionFrightenedZealousAccusation", SubclassNamespace)
            .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
            .SetAllowMultipleInstances(false)
            .SetSpecialInterruptions(
                RuleDefinitions.ConditionInterruption.Damaged,
                RuleDefinitions.ConditionInterruption.DamagedByFriendly
                )
            .Configure(RuleDefinitions.DurationType.Minute, 1, false,
                FeatureDefinitionCombatAffinitys.CombatAffinityFrightened,
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained)
            .AddToDB();
        
        
        
        
        Subclass = CharacterSubclassDefinitionBuilder
            .Create("PaladinOathOfRetribution", SubclassNamespace)
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.DomainBattle.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(paladinOathOfRetributionAutoPreparedSpells, 3)
            .AddToDB();
    }

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }
}
