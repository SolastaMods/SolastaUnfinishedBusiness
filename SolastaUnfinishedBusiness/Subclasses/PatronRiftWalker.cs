using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public class PatronRiftWalker : AbstractSubclass
{
    private const string Name = "RiftWalker";

    public PatronRiftWalker()
    {
        // LEVEL 01

        // Expanded Spells

        var spellListRiftWalker = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListWizard, $"SpellList{Name}")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(1, Jump, Longstrider)
            .SetSpellsAtLevel(2, Blur, PassWithoutTrace)
            .SetSpellsAtLevel(3, Haste, Slow)
            .SetSpellsAtLevel(4, FreedomOfMovement, GreaterInvisibility)
            .SetSpellsAtLevel(5, MindTwist, DispelEvilAndGood)
            .FinalizeSpells(true, 9)
            .AddToDB();

        var magicAffinityExpandedSpells = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}ExpandedSpells")
            .SetGuiPresentation("MagicAffinityPatronExpandedSpells", Category.Feature)
            .SetExtendedSpellList(spellListRiftWalker)
            .AddToDB();

        // Blink

        var powerBlink = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Blink")
            .SetGuiPresentation(Category.Feature, PowerShadowcasterShadowDodge)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest, 1, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Banishment)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetNoSavingThrow()
                    .Build())
            .AddCustomSubFeatures(new PreventRemoveConcentrationRiftWalker())
            .AddToDB();

        // Rift Step

        var powerRiftStep = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}RiftStep")
            .SetGuiPresentation(Category.Feature, MistyStep)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(MistyStep)
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        // LEVEL 06

        // Restrained Immunity

        var conditionAffinityRestrainedImmunity = FeatureDefinitionConditionAffinityBuilder
            .Create(ConditionAffinityRestrainedmmunity, $"ConditionAffinity{Name}RestrainedImmunity")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // Rift Strike

        var powerRiftWalkerRiftStrike = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}RiftStrike")
            .SetGuiPresentation(Category.Feature, Banishment)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.ShortRest)
            .SetReactionContext(ReactionTriggerContext.HitByMelee)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Banishment)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetNoSavingThrow()
                    .Build())
            .AddCustomSubFeatures(new PreventRemoveConcentrationRiftWalker())
            .AddToDB();

        // LEVEL 10

        // Fade into the Void

        var damageAffinityRiftWalkerFadeIntoTheVoid = FeatureDefinitionDamageAffinityBuilder
            .Create(DamageAffinityHalfOrcRelentlessEndurance, $"DamageAffinity{Name}FadeIntoTheVoid")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // Rift Portal

        var powerRiftPortal = FeatureDefinitionPowerBuilder
            .Create(powerRiftStep, $"Power{Name}RiftPortal")
            .SetGuiPresentation(Category.Feature, DimensionDoor)
            .SetOverriddenPower(powerRiftStep)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(DimensionDoor)
                    .Build())
            .AddCustomSubFeatures(new PreventRemoveConcentrationRiftWalker())
            .AddToDB();

        // LEVEL 14

        // Rift Cloak

        var conditionWardedByRiftWalkWardingBond = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionWardedByWardingBond, $"Condition{Name}RiftCloak")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .AddToDB();

        conditionWardedByRiftWalkWardingBond.Features.RemoveAll(x =>
            x is FeatureDefinitionAttributeModifier or FeatureDefinitionSavingThrowAffinity);

        var powerRiftCloak = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}RiftCloak")
            .SetGuiPresentation(Category.Feature, WardingBond)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(WardingBond)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionWardedByRiftWalkWardingBond))
                    .Build())
            .AddCustomSubFeatures(new PreventRemoveConcentrationRiftWalker())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"Patron{Name}")
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PatronRiftWalker, 256))
            .AddFeaturesAtLevel(1, magicAffinityExpandedSpells, powerBlink, powerRiftStep)
            .AddFeaturesAtLevel(6, conditionAffinityRestrainedImmunity, powerRiftWalkerRiftStrike)
            .AddFeaturesAtLevel(10, damageAffinityRiftWalkerFadeIntoTheVoid, powerRiftPortal)
            .AddFeaturesAtLevel(14, powerRiftCloak)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Warlock;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class PreventRemoveConcentrationRiftWalker : IPreventRemoveConcentrationOnPowerUse;
}
