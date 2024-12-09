using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class MartialWeaponMaster : AbstractSubclass
{
    private const string Name = "MartialWeaponMaster";
    private const string Specialization = "Specialization";

    public MartialWeaponMaster()
    {
        // LEVEL 03

        // Specialization
        var invocationPoolSpecialization =
            CustomInvocationPoolDefinitionBuilder
                .Create($"InvocationPool{Name}{Specialization}")
                .SetGuiPresentation($"AttributeModifier{Name}{Specialization}", Category.Feature)
                .Setup(InvocationPoolTypeCustom.Pools.MartialWeaponMasterWeaponSpecialization)
                .AddToDB();

        var featureSpecializationDisadvantage = FeatureDefinitionBuilder
            .Create($"Feature{Name}{Specialization}Disadvantage")
            .SetGuiPresentation($"AttributeModifier{Name}Specialization", Category.Feature, hidden: true)
            .AddToDB();

        featureSpecializationDisadvantage.AddCustomSubFeatures(
            new ModifyAttackActionModifierSpecializationDisadvantage(featureSpecializationDisadvantage));

        var dbWeaponTypeDefinition = DatabaseRepository.GetDatabase<WeaponTypeDefinition>()
            .Where(x => x != WeaponTypeDefinitions.UnarmedStrikeType &&
                        x != CustomWeaponsContext.ThunderGauntletType &&
                        x != CustomWeaponsContext.LightningLauncherType);

        foreach (var weaponTypeDefinition in dbWeaponTypeDefinition)
        {
            var weaponTypeName = weaponTypeDefinition.Name;

            var featureSpecialization = FeatureDefinitionBuilder
                .Create($"Feature{Name}{Specialization}{weaponTypeName}")
                .SetGuiPresentation($"AttributeModifier{Name}Specialization", Category.Feature)
                .AddToDB();

            featureSpecialization.AddCustomSubFeatures(
                new ModifyWeaponAttackModeSpecialization(weaponTypeDefinition, featureSpecialization));

            _ = CustomInvocationDefinitionBuilder
                .Create($"CustomInvocation{Name}{Specialization}{weaponTypeName}")
                .SetGuiPresentation(
                    weaponTypeDefinition.GuiPresentation.Title,
                    featureSpecialization.GuiPresentation.Description,
                    CustomWeaponsContext.GetStandardWeaponOfType(weaponTypeDefinition.Name))
                .SetPoolType(InvocationPoolTypeCustom.Pools.MartialWeaponMasterWeaponSpecialization)
                .SetGrantedFeature(featureSpecialization)
                .AddCustomSubFeatures(ModifyInvocationVisibility.Marker)
                .AddToDB();
        }

        // Focused Strikes

        const string FocusedStrikes = "FocusedStrikes";

        var featureFocusedStrikes = FeatureDefinitionBuilder
            .Create($"Feature{Name}{FocusedStrikes}")
            .SetGuiPresentation($"Condition{Name}{FocusedStrikes}", Category.Condition)
            .AddToDB();

        featureFocusedStrikes.AddCustomSubFeatures(new CustomBehaviorFocusedStrikes(featureFocusedStrikes));

        var conditionFocusedStrikes = ConditionDefinitionBuilder
            .Create($"Condition{Name}{FocusedStrikes}")
            .SetGuiPresentation($"Condition{Name}{FocusedStrikes}", Category.Condition, ConditionGuided)
            .SetPossessive()
            .SetFeatures(featureFocusedStrikes)
            .AddToDB();

        var powerFocusedStrikes = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{FocusedStrikes}")
            .SetGuiPresentation($"Power{Name}{FocusedStrikes}", Category.Feature,
                Sprites.GetSprite(FocusedStrikes, Resources.PowerFocusedStrikes, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest, 1, 3)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionFocusedStrikes, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        // LEVEL 07

        // Momentum

        var conditionMomentum = ConditionDefinitionBuilder
            .Create($"Condition{Name}Momentum")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionAdditionalActionBuilder
                    .Create($"AdditionalAction{Name}Momentum")
                    .SetGuiPresentationNoContent(true)
                    .SetActionType(ActionDefinitions.ActionType.Main)
                    .SetRestrictedActions(ActionDefinitions.Id.AttackMain)
                    .SetMaxAttacksNumber(1)
                    .AddToDB())
            .AddToDB();

        var featureMomentum = FeatureDefinitionBuilder
            .Create($"Feature{Name}Momentum")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureMomentum.AddCustomSubFeatures(new OnReducedToZeroHpByMeMomentum(featureMomentum, conditionMomentum));

        // LEVEL 10

        // Battle Stance

        var featureBattleStance = FeatureDefinitionBuilder
            .Create($"Feature{Name}BattleStance")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new BattleStartedBattleStance())
            .AddToDB();

        // LEVEL 15

        // Deadly Accuracy

        var additionalDamageDeadlyAccuracy = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}DeadlyAccuracy")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("DeadlyAccuracy")
            .SetDamageDice(DieType.D6, 2)
            .SetIgnoreCriticalDoubleDice(true)
            .AddToDB();

        var featureDeadlyAccuracy = FeatureDefinitionBuilder
            .Create($"Feature{Name}DeadlyAccuracy")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CustomAdditionalDamageDeadlyAccuracy(additionalDamageDeadlyAccuracy))
            .AddToDB();

        // LEVEL 18

        // Perfect Strikes

        var featurePerfectStrikes = FeatureDefinitionBuilder
            .Create($"Feature{Name}PerfectStrikes")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featurePerfectStrikes.AddCustomSubFeatures(new PerfectStrikes(conditionFocusedStrikes, featurePerfectStrikes));

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.MartialWeaponMaster, 256))
            .AddFeaturesAtLevel(3,
                FeatureDefinitionAttributeModifiers.AttributeModifierMartialChampionImprovedCritical,
                invocationPoolSpecialization,
                featureSpecializationDisadvantage,
                powerFocusedStrikes)
            .AddFeaturesAtLevel(7,
                featureMomentum)
            .AddFeaturesAtLevel(10,
                featureBattleStance)
            .AddFeaturesAtLevel(15,
                FeatureDefinitionAttributeModifiers.AttributeModifierMartialChampionSuperiorCritical,
                featureDeadlyAccuracy)
            .AddFeaturesAtLevel(18,
                featurePerfectStrikes)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Fighter;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Helpers
    //

    private static bool HasSpecializedWeapon(
        RulesetCharacter rulesetCharacter,
        RulesetAttackMode rulesetAttackMode = null,
        // ReSharper disable once SuggestBaseTypeForParameter
        WeaponTypeDefinition weaponTypeDefinition = null)
    {
        var specializedWeapons = rulesetCharacter
            .GetSubFeaturesByType<ModifyWeaponAttackModeSpecialization>()
            .Where(x => !weaponTypeDefinition || x.WeaponTypeDefinition == weaponTypeDefinition)
            .Select(x => x.WeaponTypeDefinition)
            .ToArray();

        return rulesetAttackMode != null
            ? specializedWeapons.Any(x => ValidatorsWeapon.IsOfWeaponType(x)(rulesetAttackMode, null, null))
            : specializedWeapons.Any(x => ValidatorsCharacter.HasWeaponType(x)(rulesetCharacter));
    }

    //
    // Specialization
    //

    private sealed class ModifyAttackActionModifierSpecializationDisadvantage(FeatureDefinition featureDefinition)
        : IModifyAttackActionModifier
    {
        private readonly TrendInfo _trendInfo =
            new(-1, FeatureSourceType.CharacterFeature, featureDefinition.Name, featureDefinition);

        public void OnAttackComputeModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            string effectName,
            ref ActionModifier attackModifier)
        {
            if (attackMode == null || HasSpecializedWeapon(myself, attackMode))
            {
                return;
            }

            attackModifier.AttackAdvantageTrends.Add(_trendInfo);
        }
    }

    private sealed class ModifyWeaponAttackModeSpecialization(
        WeaponTypeDefinition weaponTypeDefinition,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureDefinition)
        : IModifyWeaponAttackMode
    {
        public readonly WeaponTypeDefinition WeaponTypeDefinition = weaponTypeDefinition;

        public void ModifyWeaponAttackMode(
            RulesetCharacter character,
            RulesetAttackMode attackMode,
            RulesetItem weapon,
            bool canAddAbilityDamageBonus)
        {
            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            // pass WeaponTypeDefinition here so it only triggers once after 2nd specialization onwards
            if (!HasSpecializedWeapon(character, attackMode, WeaponTypeDefinition))
            {
                return;
            }

            var classLevel = character.GetClassLevel(CharacterClassDefinitions.Fighter);
            var bonus = !IsWeaponMaster(character)
                ? 1
                : classLevel >= 17
                    ? 3
                    : classLevel >= 9
                        ? 2
                        : 1;

            attackMode.ToHitBonus += bonus;
            attackMode.ToHitBonusTrends.Add(
                new TrendInfo(bonus, FeatureSourceType.CharacterFeature, featureDefinition.Name, featureDefinition));

            damage.BonusDamage += bonus;
            damage.DamageBonusTrends.Add(
                new TrendInfo(bonus, FeatureSourceType.CharacterFeature, featureDefinition.Name, featureDefinition));
        }

        private static bool IsWeaponMaster(RulesetCharacter rulesetCharacter)
        {
            return rulesetCharacter.GetSubclassLevel(CharacterClassDefinitions.Fighter, Name) > 0;
        }
    }

    //
    // Focused Strikes
    //

    private sealed class CustomBehaviorFocusedStrikes(FeatureDefinition featureDefinition) : IModifyAttackActionModifier
    {
        private readonly TrendInfo _trendInfo =
            new(1, FeatureSourceType.CharacterFeature, featureDefinition.Name, featureDefinition);

        public void OnAttackComputeModifier(RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            string effectName,
            ref ActionModifier attackModifier)
        {
            if (attackMode == null || !HasSpecializedWeapon(myself, attackMode))
            {
                return;
            }

            attackModifier.AttackAdvantageTrends.Add(_trendInfo);
        }
    }

    //
    // Momentum
    //

    private class OnReducedToZeroHpByMeMomentum(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureMomentum,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionMomentum)
        : IOnReducedToZeroHpByMe
    {
        public IEnumerator HandleReducedToZeroHpByMe(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (!attacker.OnceInMyTurnIsValid(featureMomentum.Name))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!HasSpecializedWeapon(rulesetAttacker, attackMode))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd(featureMomentum.Name, 1);
            rulesetAttacker.LogCharacterUsedFeature(featureMomentum);
            rulesetAttacker.InflictCondition(
                conditionMomentum.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionMomentum.Name,
                0,
                0,
                0);
        }
    }

    //
    // Battle Stance
    //

    private sealed class BattleStartedBattleStance : ICharacterBattleStartedListener
    {
        private const string Line = "Feedback/&ActivateRepaysLine";

        public void OnCharacterBattleStarted(GameLocationCharacter locationCharacter, bool surprise)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                return;
            }

            if (!HasSpecializedWeapon(rulesetCharacter))
            {
                return;
            }

            var classLevel = rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Fighter);
            var constitution = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Constitution);
            var constitutionModifier = AttributeDefinitions.ComputeAbilityScoreModifier(constitution);
            var totalHealing = classLevel + constitutionModifier;

            rulesetCharacter.ReceiveTemporaryHitPoints(
                totalHealing, DurationType.UntilAnyRest, 0, TurnOccurenceType.StartOfTurn, rulesetCharacter.guid);

            //
            // not the best code practice here but reuse this same interface for Focused Strikes 10th feature
            //

            // Focused Strikes

            var powerFocusedStrikes = GetDefinition<FeatureDefinitionPower>($"Power{Name}FocusedStrikes");
            var rulesetUsablePower = PowerProvider.Get(powerFocusedStrikes, rulesetCharacter);

            if (rulesetUsablePower.MaxUses == rulesetUsablePower.RemainingUses)
            {
                return;
            }

            rulesetCharacter.LogCharacterUsedPower(powerFocusedStrikes, Line);
            rulesetCharacter.RepayPowerUse(rulesetUsablePower);
        }
    }

    //
    // Deadly Accuracy
    //

    private sealed class CustomAdditionalDamageDeadlyAccuracy(IAdditionalDamageProvider provider)
        : CustomAdditionalDamage(provider)
    {
        internal override bool IsValid(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget,
            out CharacterActionParams reactionParams)
        {
            reactionParams = null;

            return criticalHit && HasSpecializedWeapon(attacker.RulesetCharacter, attackMode);
        }
    }

    //
    // Perfect Strikes
    //

    private sealed class PerfectStrikes(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDefinition,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureDefinition)
        : IModifyDiceRoll
    {
        public void BeforeRoll(
            RollContext rollContext,
            RulesetCharacter rulesetCharacter,
            ref DieType dieType,
            ref AdvantageType advantageType)
        {
            if (IsValid(rollContext, rulesetCharacter))
            {
                advantageType = AdvantageType.Advantage;
            }
        }

        public void AfterRoll(
            DieType dieType,
            AdvantageType advantageType,
            RollContext rollContext,
            RulesetCharacter rulesetCharacter,
            ref int firstRoll,
            ref int secondRoll,
            ref int result)
        {
            if (IsValid(rollContext, rulesetCharacter))
            {
                rulesetCharacter.LogCharacterUsedFeature(featureDefinition);
            }
        }

        private bool IsValid(RollContext rollContext, RulesetCharacter rulesetCharacter)
        {
            return rollContext == RollContext.AttackDamageValueRoll &&
                   HasSpecializedWeapon(rulesetCharacter) &&
                   rulesetCharacter.HasConditionOfType(conditionDefinition.Name);
        }
    }
}
