using System;
using System.Linq;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FightingStyleDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Fighter;

internal class RoyalKnight : AbstractSubclass
{
    private static readonly Guid SubclassNamespace = new("f5efd735-ff95-4256-ad17-dde585aeb4e2");
    private readonly CharacterSubclassDefinition Subclass;

    internal RoyalKnight()
    {
        var royalEnvoyAbilityCheckAffinity = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create(AbilityCheckAffinityChampionRemarkableAthlete, "RoyalEnvoyAbilityCheckAffinity",
                "b16f8b68-0dab-49e5-b1a2-6fdfd8836849")
            .SetAffinityGroups(new FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup
            {
                abilityScoreName = AttributeDefinitions.Charisma,
                affinity = RuleDefinitions.CharacterAbilityCheckAffinity.HalfProficiencyWhenNotProficient
            })
            .AddToDB();

        var royalEnvoyFeatureSet = FeatureDefinitionFeatureSetBuilder
            .Create("RoyalEnvoyFeature", "c8299685-d806-4e20-aff0-ca3dd4000e05")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(royalEnvoyAbilityCheckAffinity,
                FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfSolasta)
            .AddToDB();

        // TODO: use EffectDescriptionBuilder
        var effectDescription = FeatureDefinitionPowers.PowerDomainLifePreserveLife.EffectDescription.Copy();
        effectDescription.EffectForms[0].HealingForm.HealingCap = RuleDefinitions.HealingCap.MaximumHitPoints;
        effectDescription.EffectForms[0].HealingForm.DiceNumber = 4;
        effectDescription.targetFilteringTag = RuleDefinitions.TargetFilteringTag.No;

        var rallyingCryPower = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerDomainLifePreserveLife, "RallyingCryPower",
                "cabe94a7-7e51-4231-ae6d-e8e6e3954611")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.HealingWord.GuiPresentation.SpriteReference)
            .SetShortTitle("Feature/&RallyingCryPowerTitleShort")
            .SetOverriddenPower(FeatureDefinitionPowers.PowerFighterSecondWind)
            .SetActivationTime(RuleDefinitions.ActivationTime.BonusAction)
            .SetRechargeRate(RuleDefinitions.RechargeRate.ShortRest)
            .SetAbilityScore(AttributeDefinitions.Charisma)
            .SetUsesAbilityScoreName(AttributeDefinitions.Charisma)
            .SetEffectDescription(effectDescription)
            .AddToDB();

        // TODO: use EffectDescriptionBuilder
        var inspiringSurgeEffectDescription = FeatureDefinitionPowers.PowerDomainLifePreserveLife.EffectDescription
            .Copy()
            .SetTargetType(RuleDefinitions.TargetType.Individuals)
            .SetTargetParameter(1)
            .SetTargetParameter2(2)
            .SetTargetSide(RuleDefinitions.Side.Ally)
            .SetCanBePlacedOnCharacter(true)
            .SetTargetFilteringMethod(RuleDefinitions.TargetFilteringMethod.CharacterOnly)
            .SetTargetFilteringTag(RuleDefinitions.TargetFilteringTag.No)
            .SetDurationType(RuleDefinitions.DurationType.Round)
            .SetRequiresVisibilityForPosition(true)
            .SetRangeType(RuleDefinitions.RangeType.Distance)
            .SetRangeParameter(20);

        inspiringSurgeEffectDescription.EffectForms.SetRange(
            FeatureDefinitionPowers.PowerFighterActionSurge.EffectDescription.EffectForms
                .Select(x =>
                {
                    var ef = new EffectForm();

                    ef.Copy(x);

                    return ef;
                }));

        var inspiringSurgePower = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerDomainLifePreserveLife, "InspiringSurgePower",
                "c2930ad2-dd02-4ff3-bad8-46d93e328fbd")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Heroism.GuiPresentation.SpriteReference)
            .SetActivationTime(RuleDefinitions.ActivationTime.BonusAction)
            .SetRechargeRate(RuleDefinitions.RechargeRate.LongRest)
            .SetEffectDescription(inspiringSurgeEffectDescription)
            .SetShortTitle("Feature/&InspiringSurgePowerTitleShort")
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("FighterRoyalKnight", SubclassNamespace)
            .SetGuiPresentation(Category.Subclass, Protection.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(rallyingCryPower, 3)
            .AddFeatureAtLevel(royalEnvoyFeatureSet, 7)
            .AddFeatureAtLevel(inspiringSurgePower, 10)
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
