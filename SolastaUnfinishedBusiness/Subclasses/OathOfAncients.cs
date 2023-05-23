using System.Collections;
using System.Linq;
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
    internal OathOfAncients()
    {
        const string NAME = "OathOfAncients";

        //
        // LEVEL 03
        //

        //Based on Oath of the Ancients prepared spells though changed Planet Growth to Spirit Guardians.
        var autoPreparedSpellsOathAncients = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{NAME}")
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

        //
        // Level 18
        //

        var effectPowerAuraWarding18 = new EffectDescription();

        effectPowerAuraWarding18.Copy(powerAuraWarding.EffectDescription);
        effectPowerAuraWarding18.targetParameter = 13;

        var powerAuraWarding18 = FeatureDefinitionPowerBuilder
            .Create(powerAuraWarding, $"Power{NAME}AuraWarding18")
            .SetGuiPresentation(Category.Feature)
            .SetEffectDescription(effectPowerAuraWarding18)
            .SetOverriddenPower(powerAuraWarding)
            .AddToDB();

        //
        // Level 20
        //

        var savingThrowAffinityElderChampionEnemy = FeatureDefinitionSavingThrowAffinityBuilder
            .Create($"SavingThrowAffinity{NAME}ElderChampionEnemy")
            .SetGuiPresentation($"Power{NAME}ElderChampion", Category.Feature)
            .SetAffinities(CharacterSavingThrowAffinity.Disadvantage, false,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Charisma)
            .AddToDB();

        var conditionElderChampionEnemy = ConditionDefinitionBuilder
            .Create($"Condition{NAME}ElderChampionEnemy")
            .SetGuiPresentation($"Condition{NAME}ElderChampion", Category.Condition)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .SetFeatures(savingThrowAffinityElderChampionEnemy)
            .AddToDB();

        var additionalActionElderChampion = FeatureDefinitionAdditionalActionBuilder
            .Create($"AdditionalAction{NAME}ElderChampion")
            .SetGuiPresentationNoContent(true)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetRestrictedActions(ActionDefinitions.Id.AttackMain)
            .AddToDB();

        var conditionElderChampionAdditionalAttack = ConditionDefinitionBuilder
            .Create($"Condition{NAME}ElderChampionAdditionalAttack")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalActionElderChampion)
            .AddToDB();

        var featureElderChampion = FeatureDefinitionBuilder
            .Create($"Feature{NAME}ElderChampion")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(new CustomBehaviorElderChampion(conditionElderChampionEnemy,
                conditionElderChampionAdditionalAttack))
            .AddToDB();

        var conditionElderChampion = ConditionDefinitionBuilder
            .Create($"Condition{NAME}ElderChampion")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPactChainImp)
            .SetPossessive()
            .SetFeatures(featureElderChampion)
            .AddToDB();

        var powerElderChampion = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}ElderChampion")
            .SetGuiPresentation(Category.Feature, Sprites.GetSprite(NAME, Resources.PowerElderChampion, 256, 128))
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
            .Create(NAME)
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

    //
    // Elder Champion
    //

    private sealed class CustomBehaviorElderChampion :
        IMagicalEffectInitiated, ICharacterTurnStartListener, IActionFinished
    {
        private readonly ConditionDefinition _conditionElderChampionAdditionalAttack;
        private readonly ConditionDefinition _conditionElderChampionEnemy;

        public CustomBehaviorElderChampion(
            ConditionDefinition conditionAspectOfDreadEnemy,
            ConditionDefinition conditionElderChampionAdditionalAttack)
        {
            _conditionElderChampionEnemy = conditionAspectOfDreadEnemy;
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

        public IEnumerator OnMagicalEffectInitiated(CharacterActionMagicEffect characterActionMagicEffect)
        {
            var definition = characterActionMagicEffect.GetBaseDefinition();

            if (definition is not SpellDefinition { castingTime: ActivationTime.Action } &&
                definition is not FeatureDefinitionPower { RechargeRate: RechargeRate.ChannelDivinity })
            {
                yield break;
            }

            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (gameLocationBattleService == null)
            {
                yield break;
            }

            var actingCharacter = characterActionMagicEffect.ActingCharacter;
            var rulesetAttacker = actingCharacter.RulesetCharacter;

            foreach (var rulesetDefender in characterActionMagicEffect.ActionParams.TargetCharacters
                         .Where(x =>
                             gameLocationBattleService.IsWithinXCells(actingCharacter, x, 2) &&
                             x.RulesetCharacter is { IsDeadOrDying: false })
                         .Select(x => x.RulesetCharacter))
            {
                rulesetDefender.InflictCondition(
                    _conditionElderChampionEnemy.Name,
                    _conditionElderChampionEnemy.DurationType,
                    _conditionElderChampionEnemy.DurationParameter,
                    _conditionElderChampionEnemy.TurnOccurence,
                    AttributeDefinitions.TagCombat,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    null,
                    0,
                    0,
                    0);
            }
        }
    }
}
