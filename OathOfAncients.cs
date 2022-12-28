using System.Collections;

using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Models.SpellsContext;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.Subclasses;
internal sealed class OathOfAncients : AbstractSubclass
{
    //Somewhat Oath of the Ancients from 5e
    internal OathOfAncients()
    {
        //
        // LEVEL 03
        //

        //Based on Oath of the Ancients prepared spells though changed Planet Growth to Spirit Guardians.
        var autoPreparedSpellsOathAncients = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsOathForest")
            .SetGuiPresentation("DomainSpells", Category.Feature)
            .SetPreparedSpellGroups(
                BuildSpellGroup(3, EnsnaringStrike, AnimalFriendship),
                BuildSpellGroup(5, MoonBeam, MistyStep),
                BuildSpellGroup(9, ProtectionFromEnergy, SpiritGuardians))
            .SetSpellcastingClass(CharacterClassDefinitions.Paladin)
            .AddToDB();

        var conditionNaturesWrath = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionRestrainedByEntangle, "ConditionNaturesWrath")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionRestrainedByEntangle)
            .SetConditionParticleReference(Entangle.effectDescription.EffectParticleParameters.conditionParticleReference)
            .AddToDB();
        //Free single target entangle on Channel Divinity use
        var powerNaturesWrath = FeatureDefinitionPowerBuilder
            .Create("PowerNaturesWrath")
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
                            .HasSavingThrow(EffectSavingThrowType.Negates,TurnOccurenceType.EndOfTurn)
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
            .Create("PowerTurnFaithless")
            .SetGuiPresentation(Category.Feature, PowerWindShelteringBreeze)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetParticleEffectParameters(PowerWindShelteringBreeze.EffectDescription.effectParticleParameters)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetDurationData(DurationType.Round, 5, TurnOccurenceType.EndOfTurn)
                    .SetRestrictedCreatureFamilies(CharacterFamilyDefinitions.Fey, CharacterFamilyDefinitions.Fiend, CharacterFamilyDefinitions.Elemental)
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
            .Create("ConditionAuraWarding")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionAuraOfProtection)
            .AddFeatures(DamageAffinityAcidResistance)
            .AddFeatures(DamageAffinityColdResistance)
            .AddFeatures(DamageAffinityFireResistance)
            .AddFeatures(DamageAffinityLightningResistance)
            .AddFeatures(DamageAffinityThunderResistance)
            .AddFeatures(DamageAffinityPoisonResistance)
            .AddToDB();

        var powerAuraWarding = FeatureDefinitionPowerBuilder
            .Create("PowerAuraWarding")
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
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("OathOfAncients")
            .SetGuiPresentation(Category.Subclass, PatronTree)
            .AddFeaturesAtLevel(3,
            autoPreparedSpellsOathAncients,
            powerNaturesWrath,
            powerTurnFaithless)
            .AddFeaturesAtLevel(7, powerAuraWarding)
            .AddToDB();

    }

    internal override CharacterSubclassDefinition Subclass { get;  }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => FeatureDefinitionSubclassChoices
        .SubclassChoicePaladinSacredOaths;

}
