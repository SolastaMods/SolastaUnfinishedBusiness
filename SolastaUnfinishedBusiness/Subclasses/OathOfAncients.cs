using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Models;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Models.SpellsContext;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class OathOfAncients : AbstractSubclass
{
    internal OathOfAncients()
    {
        const string NAME = "OathOfAncients";

        //
        // LEVEL 03
        //

        //Based on Oath of the Ancients prepared spells though changed Planet Growth to Spirit Guardians.
        var autoPreparedSpellsOathAncients = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{NAME}")
            .SetGuiPresentation("DomainSpells", Category.Feature)
            .SetPreparedSpellGroups(
                BuildSpellGroup(3, EnsnaringStrike, AnimalFriendship),
                BuildSpellGroup(5, MoonBeam, MistyStep),
                BuildSpellGroup(9, ProtectionFromEnergy, SpiritGuardians),
                BuildSpellGroup(13, IceStorm, Stoneskin))
            .SetSpellcastingClass(CharacterClassDefinitions.Paladin)
            .AddToDB();

        var conditionNaturesWrath = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionRestrainedByEntangle, $"Condition{NAME}NaturesWrath")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionRestrainedByEntangle)
            .SetConditionParticleReference(Entangle.effectDescription.EffectParticleParameters
                .conditionParticleReference)
            .AddToDB();

        //Free single target entangle on Channel Divinity use
        var powerNaturesWrath = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}NaturesWrath")
            .SetGuiPresentation(Category.Feature, Entangle)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 10)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 8, TargetType.Individuals)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .CanSaveToCancel(TurnOccurenceType.EndOfTurn)
                            .SetConditionForm(
                                conditionNaturesWrath,
                                ConditionForm.ConditionOperation.Add,
                                false,
                                false)
                            .Build())
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Strength,
                        true,
                        EffectDifficultyClassComputation.FixedValue,
                        AttributeDefinitions.Wisdom,
                        16)
                    .Build())
            .AddToDB();

        //AoE Turned on failed Wisdom saving throw for Fey and the 0 Fiends in game.
        var powerTurnFaithless = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}TurnFaithless")
            .SetGuiPresentation(Category.Feature, PowerWindShelteringBreeze)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetParticleEffectParameters(PowerWindShelteringBreeze.EffectDescription.effectParticleParameters)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetDurationData(DurationType.Round, 5)
                    .SetRestrictedCreatureFamilies(CharacterFamilyDefinitions.Fey, CharacterFamilyDefinitions.Fiend,
                        CharacterFamilyDefinitions.Elemental)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Wisdom,
                        true,
                        EffectDifficultyClassComputation.FixedValue,
                        AttributeDefinitions.Wisdom,
                        16)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .CanSaveToCancel(TurnOccurenceType.EndOfTurn)
                            .SetConditionForm(
                                ConditionDefinitions.ConditionTurned,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        //
        // LEVEL 7
        //

        //Grants resistance against some elemental damage though I would like to change to resistance to spells eventually
        var conditionAuraWarding = ConditionDefinitionBuilder
            .Create($"Condition{NAME}AuraWarding")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionAuraOfProtection)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddFeatures(DamageAffinityAcidResistance)
            .AddFeatures(DamageAffinityColdResistance)
            .AddFeatures(DamageAffinityFireResistance)
            .AddFeatures(DamageAffinityLightningResistance)
            .AddFeatures(DamageAffinityThunderResistance)
            .AddFeatures(DamageAffinityPoisonResistance)
            .AddToDB();

        // only reports condition on char panel
        Global.CharacterLabelEnabledConditions.Add(conditionAuraWarding);

        var powerAuraWarding = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}AuraWarding")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.PermanentUnlessIncapacitated)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 3)
                    .SetDurationData(DurationType.Permanent, 0, TurnOccurenceType.StartOfTurn)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionAuraWarding,
                                ConditionForm.ConditionOperation.Add,
                                true,
                                true)
                            .Build())
                    .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Subclass, PatronTree)
            .AddFeaturesAtLevel(3,
                autoPreparedSpellsOathAncients,
                powerNaturesWrath,
                powerTurnFaithless)
            .AddFeaturesAtLevel(7, powerAuraWarding)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => FeatureDefinitionSubclassChoices
        .SubclassChoicePaladinSacredOaths;
}
