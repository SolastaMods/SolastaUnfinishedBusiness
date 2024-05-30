using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public class PatronArchfey : AbstractSubclass
{
    private const string Name = "Archfey";

    public PatronArchfey()
    {
        // LEVEL 01

        // Expanded Spells

        var spellListArchfey = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListWizard, $"SpellList{Name}")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(1, FaerieFire, Sleep)
            .SetSpellsAtLevel(2, CalmEmotions)
            .SetSpellsAtLevel(3)
            .SetSpellsAtLevel(4, DominateBeast, GreaterInvisibility)
            .SetSpellsAtLevel(5, DominatePerson)
            .FinalizeSpells(true, 9)
            .AddToDB();

        var magicAffinityExpandedSpells = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}ExpandedSpells")
            .SetGuiPresentation("MagicAffinityPatronExpandedSpells", Category.Feature)
            .SetExtendedSpellList(spellListArchfey)
            .AddToDB();

        // Fey Presence

        var powerFeyPresence = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}FeyPresence")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .Build())
            .AddToDB();

        var powerFeyPresenceCharmed = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}FeyPresenceCharmed")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.Action, powerFeyPresence)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionCharmed,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var powerFeyPresenceFrightened = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}FeyPresenceFrightened")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.Action, powerFeyPresence)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionFrightened,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        PowerBundle.RegisterPowerBundle(powerFeyPresence, false,
            powerFeyPresenceCharmed, powerFeyPresenceFrightened);

        var featureSetFeyPresence = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}FeyPresence")
            .SetGuiPresentation($"Power{Name}FeyPresence", Category.Feature)
            .SetFeatureSet(powerFeyPresence, powerFeyPresenceCharmed, powerFeyPresenceFrightened)
            .AddToDB();

        // LEVEL 06

        // Misty Escape

        var powerMistyEscape = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MistyEscape")
            .SetGuiPresentation(Category.Feature, MistyStep)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                            .Build(),
                        EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionInvisible,
                            ConditionForm.ConditionOperation.Add, true, true))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerMelekTeleport)
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        // LEVEL 10

        // Beguiling Defenses

        var conditionAffinityBeguilingDefenses = FeatureDefinitionConditionAffinityBuilder
            .Create($"ConditionAffinity{Name}BeguilingDefenses")
            .SetGuiPresentation(Category.Feature)
            .SetConditionType(ConditionDefinitions.ConditionCharmed)
            .SetConditionAffinityType(ConditionAffinityType.Immunity)
            .AddToDB();

        // LEVEL 14

        // Dark Delirium

        var powerDarkDelirium = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DarkDelirium")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .Build())
            .AddToDB();

        var powerDarkDeliriumCharmed = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}DarkDeliriumCharmed")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.Action, powerDarkDelirium)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionCharmed,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var powerDarkDeliriumFrightened = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}DarkDeliriumFrightened")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.Action, powerDarkDelirium)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionFrightened,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        PowerBundle.RegisterPowerBundle(powerDarkDelirium, false,
            powerDarkDeliriumCharmed, powerDarkDeliriumFrightened);

        var featureSetDarkDelirium = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}DarkDelirium")
            .SetGuiPresentation($"Power{Name}DarkDelirium", Category.Feature)
            .SetFeatureSet(powerDarkDelirium, powerDarkDeliriumCharmed, powerDarkDeliriumFrightened)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"Patron{Name}")
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.RangerShadowTamer)
            .AddFeaturesAtLevel(1, magicAffinityExpandedSpells, featureSetFeyPresence)
            .AddFeaturesAtLevel(6, powerMistyEscape)
            .AddFeaturesAtLevel(10, conditionAffinityBeguilingDefenses)
            .AddFeaturesAtLevel(14, featureSetDarkDelirium)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Warlock;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }
}
