using System.Collections;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Models.SpellsContext;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class OathOfAncients : AbstractSubclass
{
    private const string Name = "OathOfAncients";

    private static readonly ConditionDefinition ConditionElderChampionEnemy = ConditionDefinitionBuilder
        .Create($"Condition{Name}ElderChampionEnemy")
        .SetGuiPresentation($"Condition{Name}ElderChampion", Category.Condition)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
        .SetSpecialInterruptions(ConditionInterruption.SavingThrow)
        .SetFeatures(FeatureDefinitionSavingThrowAffinityBuilder
            .Create($"SavingThrowAffinity{Name}ElderChampionEnemy")
            .SetGuiPresentation($"Power{Name}ElderChampion", Category.Feature)
            .SetAffinities(CharacterSavingThrowAffinity.Disadvantage, false,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Charisma)
            .AddToDB())
        .AddToDB();

    internal OathOfAncients()
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
            .SetConditionParticleReference(Entangle.effectDescription.EffectParticleParameters
                .conditionParticleReference)
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
                    .SetParticleEffectParameters(PowerWindShelteringBreeze.EffectDescription.effectParticleParameters)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetDurationData(DurationType.Round, 5)
                    .SetRestrictedCreatureFamilies(
                        CharacterFamilyDefinitions.Fey.Name,
                        CharacterFamilyDefinitions.Fiend.Name,
                        CharacterFamilyDefinitions.Elemental.Name)
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
            .Create($"Feature{Name}AuraWarding")
            .SetCustomSubFeatures(new AuraWardingModifyMagic(conditionAuraWardingResistance))
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
            .Create(DamageAffinityHalfOrcRelentlessEndurance, $"DamageAffinity{Name}UndyingSentinel")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .AddToDB();

        //
        // Level 18
        //

        var effectPowerAuraWarding18 = new EffectDescription();

        effectPowerAuraWarding18.Copy(powerAuraWarding.EffectDescription);
        effectPowerAuraWarding18.targetParameter = 13;

        var powerAuraWarding18 = FeatureDefinitionPowerBuilder
            .Create(powerAuraWarding, $"Power{Name}AuraWarding18")
            .SetGuiPresentation(Category.Feature)
            .SetEffectDescription(effectPowerAuraWarding18)
            .SetOverriddenPower(powerAuraWarding)
            .AddToDB();

        //
        // Level 20
        //

        var additionalActionElderChampion = FeatureDefinitionAdditionalActionBuilder
            .Create($"AdditionalAction{Name}ElderChampion")
            .SetGuiPresentationNoContent(true)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetRestrictedActions(ActionDefinitions.Id.AttackMain)
            .AddToDB();

        var conditionElderChampionAdditionalAttack = ConditionDefinitionBuilder
            .Create($"Condition{Name}ElderChampionAdditionalAttack")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalActionElderChampion)
            .AddToDB();

        var featureElderChampion = FeatureDefinitionBuilder
            .Create($"Feature{Name}ElderChampion")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(new CustomBehaviorElderChampion(conditionElderChampionAdditionalAttack))
            .AddToDB();

        var conditionElderChampion = ConditionDefinitionBuilder
            .Create($"Condition{Name}ElderChampion")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPactChainImp)
            .SetPossessive()
            .SetFeatures(featureElderChampion)
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
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionElderChampion, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetGuiPresentation(Category.Feature)
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

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => FeatureDefinitionSubclassChoices
        .SubclassChoicePaladinSacredOaths;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Elder Champion
    //

    internal static void OnRollSavingThrowElderChampion(
        RulesetCharacter caster,
        RulesetActor target,
        BaseDefinition sourceDefinition)
    {
        if (sourceDefinition is not ItemDefinition &&
            sourceDefinition is not FeatureDefinitionAdditionalDamage &&
            sourceDefinition is not SpellDefinition { castingTime: ActivationTime.Action } &&
            sourceDefinition is not FeatureDefinitionPower { RechargeRate: RechargeRate.ChannelDivinity })
        {
            return;
        }

        var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();
        var gameLocationCaster = GameLocationCharacter.GetFromActor(caster);
        var gameLocationTarget = GameLocationCharacter.GetFromActor(target);

        if (gameLocationCaster == null ||
            gameLocationTarget == null ||
            gameLocationBattleService == null ||
            !gameLocationBattleService.IsWithinXCells(gameLocationCaster, gameLocationTarget, 2))
        {
            return;
        }

        if (!caster.HasAnyConditionOfType($"Condition{Name}ElderChampion"))
        {
            return;
        }

        target.InflictCondition(
            ConditionElderChampionEnemy.Name,
            ConditionElderChampionEnemy.DurationType,
            ConditionElderChampionEnemy.DurationParameter,
            ConditionElderChampionEnemy.TurnOccurence,
            AttributeDefinitions.TagCombat,
            caster.guid,
            caster.CurrentFaction.Name,
            1,
            null,
            0,
            0,
            0);
    }

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

            target.InflictCondition(
                _conditionWardingAura.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagCombat,
                target.guid,
                target.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);

            return effect;
        }
    }

    private sealed class CustomBehaviorElderChampion : ICharacterTurnStartListener, IActionFinished
    {
        private readonly ConditionDefinition _conditionElderChampionAdditionalAttack;

        public CustomBehaviorElderChampion(ConditionDefinition conditionElderChampionAdditionalAttack)
        {
            _conditionElderChampionAdditionalAttack = conditionElderChampionAdditionalAttack;
        }

        public IEnumerator OnActionFinished(CharacterAction characterAction)
        {
            if (characterAction.ActionType != ActionDefinitions.ActionType.Main ||
                characterAction is not CharacterActionCastSpell)
            {
                yield break;
            }

            var actingCharacter = characterAction.ActingCharacter;

            if (!actingCharacter.CanAct())
            {
                yield break;
            }

            var rulesetCharacter = actingCharacter.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                _conditionElderChampionAdditionalAttack.Name,
                _conditionElderChampionAdditionalAttack.DurationType,
                _conditionElderChampionAdditionalAttack.DurationParameter,
                _conditionElderChampionAdditionalAttack.TurnOccurence,
                AttributeDefinitions.TagCombat,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }

        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            locationCharacter.RulesetCharacter.ReceiveHealing(10, true, locationCharacter.Guid);
        }
    }
}
