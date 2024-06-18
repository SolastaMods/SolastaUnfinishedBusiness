using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Models.SpellsContext;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class OathOfAncients : AbstractSubclass
{
    private const string Name = "OathOfAncients";

    internal const string ConditionElderChampionName = $"Condition{Name}ElderChampion";

    internal static readonly ConditionDefinition ConditionElderChampionEnemy = ConditionDefinitionBuilder
        .Create($"Condition{Name}ElderChampionEnemy")
        .SetGuiPresentation(ConditionElderChampionName, Category.Condition)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetPossessive()
        .SetSpecialInterruptions(ConditionInterruption.SavingThrow)
        .SetFeatures(
            FeatureDefinitionSavingThrowAffinityBuilder
                .Create($"SavingThrowAffinity{Name}ElderChampionEnemy")
                .SetGuiPresentation(ConditionElderChampionName, Category.Condition, Global.Empty)
                .SetAffinities(CharacterSavingThrowAffinity.Disadvantage, false,
                    AttributeDefinitions.Strength,
                    AttributeDefinitions.Dexterity,
                    AttributeDefinitions.Constitution,
                    AttributeDefinitions.Intelligence,
                    AttributeDefinitions.Wisdom,
                    AttributeDefinitions.Charisma)
                .AddToDB())
        .AddToDB();

    public OathOfAncients()
    {
        //
        // LEVEL 03
        //

        //Based on Oath of the Ancients prepared spells though changed Planet Growth to Spirit Guardians.
        var autoPreparedSpellsOathAncients = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation("Subclass/&OathOfAncientsTitle", "Feature/&DomainSpellsDescription")
            .SetAutoTag("Oath")
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, EnsnaringStrike, AnimalFriendship),
                BuildSpellGroup(5, MoonBeam, MistyStep),
                BuildSpellGroup(9, ProtectionFromEnergy, SpiritGuardians),
                BuildSpellGroup(13, IceStorm, Stoneskin))
            .SetSpellcastingClass(CharacterClassDefinitions.Paladin)
            .AddToDB();

        var conditionNaturesWrath = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionRestrainedByEntangle, $"Condition{Name}NaturesWrath")
            .AddToDB();

        //Free single target entangle on Channel Divinity use
        var powerNaturesWrath = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}NaturesWrath")
            .SetGuiPresentation(Category.Feature, Entangle)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 10)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 8, TargetType.IndividualsUnique)
                    .SetParticleEffectParameters(Entangle)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(
                                conditionNaturesWrath,
                                ConditionForm.ConditionOperation.Add)
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
            .Create($"Power{Name}TurnFaithless")
            .SetGuiPresentation(Category.Feature, PowerWindShelteringBreeze)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetParticleEffectParameters(PowerWindShelteringBreeze)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetDurationData(DurationType.Round, 5)
                    .SetRestrictedCreatureFamilies("Fey", "Fiend", "Elemental")
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
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
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
            .Create($"Condition{Name}AuraWardingResistance")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddFeatures(
                DamageAffinityAcidResistance,
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
            .AddToDB();

        var featureAuraWarding = FeatureDefinitionBuilder
            .Create($"Feature{Name}AuraWarding")
            .AddCustomSubFeatures(new MagicEffectBeforeHitConfirmedOnMeAuraWarding(conditionAuraWardingResistance))
            .SetGuiPresentationNoContent(true)
            .AddToDB();

        var conditionAuraWarding = ConditionDefinitionBuilder
            .Create($"Condition{Name}AuraWarding")
            .SetGuiPresentation($"Power{Name}AuraWarding", Category.Feature, ConditionDefinitions.ConditionBlessed)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(featureAuraWarding)
            .AddToDB();

        var powerAuraWarding = FeatureDefinitionPowerBuilder
            .Create(PowerPaladinAuraOfProtection, $"Power{Name}AuraWarding")
            .SetGuiPresentation(Category.Feature, PowerOathOfDevotionAuraDevotion)
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
            .Create(DamageAffinityHalfOrcRelentlessEndurance, $"DamageAffinity{Name}UndyingSentinel")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .AddToDB();

        //
        // Level 18
        //

        var powerAuraWarding18 = FeatureDefinitionPowerBuilder
            .Create(powerAuraWarding, $"Power{Name}AuraWarding18")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetOverriddenPower(powerAuraWarding)
            .AddToDB();

        powerAuraWarding18.EffectDescription.targetParameter = 13;

        //
        // Level 20
        //

        var additionalActionElderChampion = FeatureDefinitionAdditionalActionBuilder
            .Create($"AdditionalAction{Name}ElderChampion")
            .SetGuiPresentationNoContent(true)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetRestrictedActions(ActionDefinitions.Id.AttackMain)
            .SetMaxAttacksNumber(2)
            .AddToDB();

        var conditionElderChampionAdditionalAttack = ConditionDefinitionBuilder
            .Create($"Condition{Name}ElderChampionAdditionalAttack")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalActionElderChampion)
            .AddToDB();

        var conditionElderChampion = ConditionDefinitionBuilder
            .Create(ConditionElderChampionName)
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPactChainImp)
            .SetPossessive()
            .AddCustomSubFeatures(new CustomBehaviorElderChampion(conditionElderChampionAdditionalAttack))
            .AddToDB();

        var powerElderChampion = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ElderChampion")
            .SetGuiPresentation(Category.Feature, Sprites.GetSprite(Name, Resources.PowerElderChampion, 256, 128))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetParticleEffectParameters(PowerRangerSwiftBladeBattleFocus)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionElderChampion, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite("OathOfAncients", Resources.OathOfAncients, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpellsOathAncients,
                powerNaturesWrath,
                powerTurnFaithless)
            .AddFeaturesAtLevel(7, powerAuraWarding)
            .AddFeaturesAtLevel(15, damageAffinityUndyingSentinel)
            .AddFeaturesAtLevel(18, powerAuraWarding18)
            .AddFeaturesAtLevel(20, powerElderChampion)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Paladin;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => FeatureDefinitionSubclassChoices
        .SubclassChoicePaladinSacredOaths;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class MagicEffectBeforeHitConfirmedOnMeAuraWarding(ConditionDefinition conditionWardingAura)
        : IMagicEffectBeforeHitConfirmedOnMe
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetDefender = defender.RulesetCharacter;

            rulesetDefender.InflictCondition(
                conditionWardingAura.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetDefender.guid,
                rulesetDefender.CurrentFaction.Name,
                1,
                conditionWardingAura.Name,
                0,
                0,
                0);

            yield break;
        }
    }

    private sealed class CustomBehaviorElderChampion(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionElderChampionAdditionalAttack)
        : ICharacterTurnStartListener, IMagicEffectFinishedByMeAny
    {
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            locationCharacter.RulesetCharacter.ReceiveHealing(10, true, locationCharacter.Guid);
        }

        public IEnumerator OnMagicEffectFinishedByMeAny(
            CharacterActionMagicEffect action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            if (action.ActionType != ActionDefinitions.ActionType.Main ||
                action is not CharacterActionCastSpell)
            {
                yield break;
            }

            var rulesetCharacter = attacker.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                conditionElderChampionAdditionalAttack.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionElderChampionAdditionalAttack.Name,
                0,
                0,
                0);
        }
    }
}
