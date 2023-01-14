using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class PatronRiftWalker : AbstractSubclass
{
    internal PatronRiftWalker()
    {
        var spellListRiftWalker = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListWizard, "SpellListRiftWalker")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(1, Jump, Longstrider)
            .SetSpellsAtLevel(2, Blur, PassWithoutTrace)
            .SetSpellsAtLevel(3, Haste, Slow)
            .SetSpellsAtLevel(4, FreedomOfMovement, GreaterInvisibility)
            .SetSpellsAtLevel(5, MindTwist, DispelEvilAndGood)
            .FinalizeSpells(true, 9)
            .AddToDB();

        var magicAffinityRiftWalkerExpandedSpells = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityRiftWalkerExpandedSpells")
            .SetGuiPresentation("MagicAffinityPatronExpandedSpells", Category.Feature)
            .SetExtendedSpellList(spellListRiftWalker)
            .AddToDB();

        var powerRiftWalkerRiftWalk = FeatureDefinitionPowerBuilder
            .Create("PowerRiftWalkerRiftWalk")
            .SetGuiPresentation(Category.Feature, MistyStep)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(MistyStep.EffectDescription)
            .SetUniqueInstance()
            .AddToDB();

        var powerRiftWalkerBlink = FeatureDefinitionPowerBuilder
            .Create("PowerRiftWalkerBlink")
            .SetGuiPresentation(Category.Feature, PowerShadowcasterShadowDodge)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(Banishment.EffectDescription)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetNoSavingThrow()
                .Build())
            .SetCustomSubFeatures(new PreventRemoveConcentrationRiftWalker())
            .AddToDB();

        var conditionAffinityRiftWalkerRestrainedImmunity = FeatureDefinitionConditionAffinityBuilder
            .Create(ConditionAffinityRestrainedmmunity, "ConditionAffinityRiftWalkerRestrainedImmunity")
            .SetGuiPresentation(Category.Condition)
            .AddToDB();

        var powerRiftWalkerRiftStrike = FeatureDefinitionPowerBuilder
            .Create("PowerRiftWalkerRiftStrike")
            .SetGuiPresentation(Category.Feature, Banishment)
            .SetUsesProficiencyBonus(ActivationTime.Reaction)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(Banishment.EffectDescription)
                .SetDurationData(DurationType.Round, 1)
                .SetNoSavingThrow()
                .Build())
            .SetCustomSubFeatures(new PreventRemoveConcentrationRiftWalker())
            .SetReactionContext(ReactionTriggerContext.HitByMelee)
            .AddToDB();

        var powerRiftWalkerRiftControl = FeatureDefinitionPowerBuilder
            .Create("PowerRiftWalkerRiftControl")
            .SetGuiPresentation(Category.Feature, DimensionDoor)
            .SetOverriddenPower(powerRiftWalkerRiftWalk)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(DimensionDoor.EffectDescription)
            .SetCustomSubFeatures(new PreventRemoveConcentrationRiftWalker())
            .AddToDB();

        var damageAffinityRiftWalkerFadeIntoTheVoid = FeatureDefinitionDamageAffinityBuilder
            .Create(DamageAffinityHalfOrcRelentlessEndurance, "DamageAffinityRiftWalkerFadeIntoTheVoid")
            .SetGuiPresentation(Category.Feature, Blur)
            .AddToDB();

        var bonusCantripRiftWalkWardingBond = FeatureDefinitionBonusCantripsBuilder
            .Create("BonusCantripRiftWalkWardingBond")
            .SetGuiPresentation(Category.Feature)
            .SetBonusCantrips(
                SpellDefinitionBuilder
                    .Create(WardingBond, "AtWillWardingBond")
                    .SetSpellLevel(0)
                    .AddToDB())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("PatronRiftWalker")
            .SetGuiPresentation(Category.Subclass,
                CharacterSubclassDefinitions.PathMagebane)
            .AddFeaturesAtLevel(1,
                magicAffinityRiftWalkerExpandedSpells,
                powerRiftWalkerRiftWalk,
                powerRiftWalkerBlink)
            .AddFeaturesAtLevel(6,
                conditionAffinityRiftWalkerRestrainedImmunity,
                powerRiftWalkerRiftStrike)
            .AddFeaturesAtLevel(10,
                powerRiftWalkerRiftControl,
                damageAffinityRiftWalkerFadeIntoTheVoid)
            .AddFeaturesAtLevel(14,
                bonusCantripRiftWalkWardingBond)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;

    private sealed class PreventRemoveConcentrationRiftWalker : IPreventRemoveConcentrationWithPowerUse
    {
    }
}
