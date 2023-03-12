using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
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
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Oath")
            .SetPreparedSpellGroups(
                BuildSpellGroup(3, EnsnaringStrike, AnimalFriendship),
                BuildSpellGroup(5, MoonBeam, MistyStep),
                BuildSpellGroup(9, ProtectionFromEnergy, SpiritGuardians),
                BuildSpellGroup(13, IceStorm, Stoneskin))
            .SetSpellcastingClass(CharacterClassDefinitions.Paladin)
            .AddToDB();

        var conditionNaturesWrath = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionRestrainedByEntangle, $"Condition{NAME}NaturesWrath")
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

        var conditionAuraWardingResistance = ConditionDefinitionBuilder
            .Create($"Condition{NAME}AuraWardingResistance")
            .SetGuiPresentationNoContent(true)
            .AddFeatures(DamageAffinityAcidResistance,
                DamageAffinityBludgeoningResistance,
                DamageAffinityColdResistance,
                DamageAffinityFireResistance,
                DamageAffinityForceDamageResistance,
                DamageAffinityLightningResistance,
                DamageAffinityNecroticResistance,
                DamageAffinityPiercingResistance,
                DamageAffinityPoisonResistance,
                DamageAffinityPsychicResistance,
                DamageAffinityRadiantResistance,
                DamageAffinitySlashingResistance,
                DamageAffinityThunderResistance)
            .AddSpecialInterruptions(ConditionInterruption.Damaged)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var featureAuraWarding = FeatureDefinitionBuilder
            .Create($"Feature{NAME}AuraWarding")
            .SetCustomSubFeatures(new AuraWardingModifyMagic(conditionAuraWardingResistance))
            .SetGuiPresentationNoContent(true)
            .AddToDB();

        var conditionAuraWarding = ConditionDefinitionBuilder
            .Create($"Condition{NAME}AuraWarding")
            .SetGuiPresentation($"Power{NAME}AuraWarding", Category.Feature, ConditionDefinitions.ConditionBlessed)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(featureAuraWarding)
            .AddToDB();

        var powerAuraWarding = FeatureDefinitionPowerBuilder
            .Create(PowerPaladinAuraOfProtection, $"Power{NAME}AuraWarding")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // keep it simple and ensure it'll follow any changes from Aura of Protection
        powerAuraWarding.EffectDescription.EffectForms[0] = EffectFormBuilder
            .Create()
            .SetConditionForm(conditionAuraWarding, ConditionForm.ConditionOperation.Add)
            .Build();

        //
        // Level 15
        //

        var damageAffinityUndyingSentinel = FeatureDefinitionDamageAffinityBuilder
            .Create(DamageAffinityHalfOrcRelentlessEndurance, $"DamageAffinity{NAME}UndyingSentinel")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite("OathOfAncients", Resources.OathOfAncients, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpellsOathAncients,
                powerNaturesWrath,
                powerTurnFaithless)
            .AddFeaturesAtLevel(7, powerAuraWarding)
            .AddFeaturesAtLevel(15, damageAffinityUndyingSentinel)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => FeatureDefinitionSubclassChoices
        .SubclassChoicePaladinSacredOaths;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class AuraWardingModifyMagic : IModifyMagicEffectOnTarget
    {
        private readonly ConditionDefinition _conditionWardingAura;

        internal AuraWardingModifyMagic(ConditionDefinition conditionWardingAura)
        {
            _conditionWardingAura = conditionWardingAura;
        }

        public EffectDescription ModifyEffect(
            BaseDefinition definition,
            EffectDescription effect,
            RulesetCharacter caster,
            RulesetCharacter target)
        {
            if (definition is not SpellDefinition)
            {
                return effect;
            }

            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                target.Guid,
                _conditionWardingAura,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                target.Guid,
                target.CurrentFaction.Name
            );

            target.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);

            return effect;
        }
    }
}
