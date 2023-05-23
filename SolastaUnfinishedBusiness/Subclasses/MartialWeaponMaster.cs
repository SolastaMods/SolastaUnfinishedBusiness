using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class MartialWeaponMaster : AbstractSubclass
{
    private const string Name = "MartialWeaponMaster";
    private const string Specialization = "Specialization";

    internal static readonly FeatureDefinitionCustomInvocationPool InvocationPoolSpecialization =
        CustomInvocationPoolDefinitionBuilder
            .Create($"InvocationPool{Name}{Specialization}")
            .SetGuiPresentation($"AttributeModifier{Name}{Specialization}", Category.Feature)
            .Setup(InvocationPoolTypeCustom.Pools.MartialWeaponMasterWeaponSpecialization)
            .AddToDB();

    internal MartialWeaponMaster()
    {
        // LEVEL 03

        // Specialization
        var featureSpecializationDisadvantage = FeatureDefinitionBuilder
            .Create($"Feature{Name}{Specialization}Disadvantage")
            .SetGuiPresentation($"AttributeModifier{Name}Specialization", Category.Feature, hidden: true)
            .AddToDB();

        featureSpecializationDisadvantage.SetCustomSubFeatures(
            new AttackComputeModifierSpecializationDisadvantage(featureSpecializationDisadvantage));

        var dbWeaponTypeDefinition = DatabaseRepository.GetDatabase<WeaponTypeDefinition>()
            .Where(x => x != WeaponTypeDefinitions.UnarmedStrikeType &&
                        x != CustomWeaponsContext.ThunderGauntletType &&
                        x != CustomWeaponsContext.LightningLauncherType);

        foreach (var weaponTypeDefinition in dbWeaponTypeDefinition)
        {
            var weaponTypeName = weaponTypeDefinition.Name;

            var featureSpecialization = FeatureDefinitionAttributeModifierBuilder
                .Create($"Feature{Name}{Specialization}{weaponTypeName}")
                .SetGuiPresentation($"AttributeModifier{Name}Specialization", Category.Feature)
                .AddToDB();

            featureSpecialization.SetCustomSubFeatures(
                new ModifyWeaponAttackModeSpecialization(weaponTypeDefinition, featureSpecialization));

            _ = CustomInvocationDefinitionBuilder
                .Create($"CustomInvocation{Name}{Specialization}{weaponTypeName}")
                .SetGuiPresentation(
                    weaponTypeDefinition.GuiPresentation.Title,
                    featureSpecialization.GuiPresentation.Description,
                    CustomWeaponsContext.GetStandardWeaponOfType(weaponTypeDefinition.Name))
                .SetPoolType(InvocationPoolTypeCustom.Pools.MartialWeaponMasterWeaponSpecialization)
                .SetGrantedFeature(featureSpecialization)
                .SetCustomSubFeatures(Hidden.Marker)
                .AddToDB();
        }

        // Focused Strikes

        const string FocusedStrikes = "FocusedStrikes";

        var featureFocusedStrikes = FeatureDefinitionBuilder
            .Create($"Feature{Name}{FocusedStrikes}")
            .SetGuiPresentation($"Condition{Name}{FocusedStrikes}", Category.Condition)
            .AddToDB();

        featureFocusedStrikes.SetCustomSubFeatures(new CustomBehaviorFocusedStrikes(featureFocusedStrikes));

        var conditionFocusedStrikes = ConditionDefinitionBuilder
            .Create($"Condition{Name}{FocusedStrikes}")
            .SetGuiPresentation($"Condition{Name}{FocusedStrikes}", Category.Condition, ConditionGuided)
            .SetPossessive()
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
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
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .SetFeatures(FeatureDefinitionAdditionalActionBuilder
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

        featureMomentum.SetCustomSubFeatures(new TargetReducedToZeroHpMomentum(featureMomentum, conditionMomentum));
        DatabaseHelper.ActionDefinitions.ActionSurge.SetCustomSubFeatures(new ActionFinishedActionSurge());

        // LEVEL 10

        // Battle Stance

        var featureBattleStance = FeatureDefinitionBuilder
            .Create($"Feature{Name}BattleStance")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new BattleStartedBattleStance())
            .AddToDB();

        // LEVEL 15

        // Deadly Accuracy

        var additionalDamageDeadlyAccuracy = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}DeadlyAccuracy")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("DeadlyAccuracy")
            .SetDamageDice(DieType.D6, 2)
            .AddToDB();

        var featureDeadlyAccuracy = FeatureDefinitionBuilder
            .Create($"Feature{Name}DeadlyAccuracy")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new CustomAdditionalDamageDeadlyAccuracy(additionalDamageDeadlyAccuracy))
            .AddToDB();

        // LEVEL 18

        // Perfect Strikes

        var featurePerfectStrikes = FeatureDefinitionBuilder
            .Create($"Feature{Name}PerfectStrikes")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featurePerfectStrikes.SetCustomSubFeatures(new PerfectStrikes(featurePerfectStrikes));

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.MartialWeaponMaster, 256))
            .AddFeaturesAtLevel(3,
                FeatureDefinitionAttributeModifiers.AttributeModifierMartialChampionImprovedCritical,
                InvocationPoolSpecialization,
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

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Helpers
    //

    internal static bool HasSpecializedWeapon(
        RulesetCharacter rulesetCharacter,
        RulesetAttackMode rulesetAttackMode = null)
    {
        var specializedWeapons = rulesetCharacter
            .GetSubFeaturesByType<ModifyWeaponAttackModeSpecialization>()
            .Select(x => x.WeaponTypeDefinition)
            .ToList();

        return rulesetAttackMode != null
            ? specializedWeapons.Any(x => ValidatorsWeapon.IsOfWeaponType(x)(rulesetAttackMode, null, null))
            : specializedWeapons.Any(x => ValidatorsCharacter.HasWeaponType(x)(rulesetCharacter));
    }

    //
    // Specialization
    //

    private sealed class AttackComputeModifierSpecializationDisadvantage : IAttackComputeModifier
    {
        private readonly FeatureDefinition _featureDefinition;

        public AttackComputeModifierSpecializationDisadvantage(FeatureDefinition featureDefinition)
        {
            _featureDefinition = featureDefinition;
        }

        public void OnAttackComputeModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            ref ActionModifier attackModifier)
        {
            if (HasSpecializedWeapon(myself, attackMode))
            {
                return;
            }

            attackModifier.attackAdvantageTrends.Add(
                new TrendInfo(-1, FeatureSourceType.CharacterFeature, _featureDefinition.Name, _featureDefinition));
        }
    }

    private sealed class ModifyWeaponAttackModeSpecialization : IModifyWeaponAttackMode
    {
        private readonly FeatureDefinition _featureDefinition;
        public readonly WeaponTypeDefinition WeaponTypeDefinition;

        public ModifyWeaponAttackModeSpecialization(
            WeaponTypeDefinition weaponTypeDefinition,
            FeatureDefinition featureDefinition)
        {
            WeaponTypeDefinition = weaponTypeDefinition;
            _featureDefinition = featureDefinition;
        }

        public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode)
        {
            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            if (!HasSpecializedWeapon(character, attackMode))
            {
                return;
            }

            var characterLevel = character.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
            var bonus = !IsWeaponMaster(character)
                ? 0
                : characterLevel >= 17
                    ? 3
                    : characterLevel >= 9
                        ? 2
                        : 1;

            attackMode.ToHitBonus += bonus;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(bonus, FeatureSourceType.CharacterFeature,
                _featureDefinition.Name, _featureDefinition));

            damage.BonusDamage += bonus;
            damage.DamageBonusTrends.Add(new TrendInfo(bonus, FeatureSourceType.CharacterFeature,
                _featureDefinition.Name, _featureDefinition));
        }

        private static bool IsWeaponMaster(RulesetCharacter rulesetCharacter)
        {
            var hero = rulesetCharacter as RulesetCharacterHero ??
                       rulesetCharacter.OriginalFormCharacter as RulesetCharacterHero;

            if (hero == null)
            {
                return false;
            }

            return hero.ClassesAndSubclasses.TryGetValue(CharacterClassDefinitions.Fighter,
                out var characterSubclassDefinition) && characterSubclassDefinition.Name == Name;
        }
    }

    //
    // Focused Strikes
    //

    private sealed class CustomBehaviorFocusedStrikes : IAttackComputeModifier
    {
        private readonly FeatureDefinition _featureDefinition;

        public CustomBehaviorFocusedStrikes(FeatureDefinition featureDefinition)
        {
            _featureDefinition = featureDefinition;
        }

        public void OnAttackComputeModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            ref ActionModifier attackModifier)
        {
            if (!HasSpecializedWeapon(myself, attackMode))
            {
                return;
            }

            attackModifier.attackAdvantageTrends.Add(
                new TrendInfo(1, FeatureSourceType.CharacterFeature, _featureDefinition.Name, _featureDefinition));
        }
    }

    //
    // Momentum
    //

    private class TargetReducedToZeroHpMomentum : ITargetReducedToZeroHp
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly FeatureDefinition _featureDefinition;

        public TargetReducedToZeroHpMomentum(
            FeatureDefinition featureDefinition,
            ConditionDefinition conditionDefinition)
        {
            _featureDefinition = featureDefinition;
            _conditionDefinition = conditionDefinition;
        }

        public IEnumerator HandleCharacterReducedToZeroHp(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (gameLocationBattleService is not { IsBattleInProgress: true })
            {
                yield break;
            }

            if (attacker.UsedSpecialFeatures.ContainsKey(_featureDefinition.Name) ||
                gameLocationBattleService.Battle.ActiveContender != attacker)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!HasSpecializedWeapon(rulesetAttacker, attackMode))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd(_featureDefinition.Name, 1);
            GameConsoleHelper.LogCharacterUsedFeature(rulesetAttacker, _featureDefinition);
            rulesetAttacker.InflictCondition(
                _conditionDefinition.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
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

    private sealed class ActionFinishedActionSurge : IActionFinished
    {
        public IEnumerator OnActionFinished(CharacterAction characterAction)
        {
            if (characterAction.ActionDefinition != DatabaseHelper.ActionDefinitions.ActionSurge)
            {
                yield break;
            }

            characterAction.ActingCharacter.RulesetCharacter
                .RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagCombat, $"Condition{Name}Momentum");
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

            if (rulesetCharacter == null)
            {
                return;
            }

            if (!HasSpecializedWeapon(rulesetCharacter))
            {
                return;
            }

            var classLevel = rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Fighter);
            var proficiencyBonus = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var constitution = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Constitution);
            var constitutionModifier = AttributeDefinitions.ComputeAbilityScoreModifier(constitution);
            var totalHealing = classLevel + proficiencyBonus + constitutionModifier;

            rulesetCharacter.ReceiveTemporaryHitPoints(
                totalHealing, DurationType.Minute, 1, TurnOccurenceType.EndOfTurn, rulesetCharacter.guid);

            //
            // not the best code practice here but reuse this same interface for Focused Strikes 10th feature
            //

            // Focused Strikes

            var powerFocusedStrikes = GetDefinition<FeatureDefinitionPower>($"Power{Name}FocusedStrikes");
            var rulesetUsablePower = rulesetCharacter.UsablePowers.Find(x => x.PowerDefinition == powerFocusedStrikes);

            if (rulesetUsablePower == null || rulesetCharacter.GetRemainingUsesOfPower(rulesetUsablePower) > 0)
            {
                return;
            }

            GameConsoleHelper.LogCharacterUsedPower(rulesetCharacter, powerFocusedStrikes, Line);
            rulesetUsablePower.RepayUse();
        }
    }

    //
    // Deadly Accuracy
    //

    private sealed class CustomAdditionalDamageDeadlyAccuracy : CustomAdditionalDamage
    {
        public CustomAdditionalDamageDeadlyAccuracy(IAdditionalDamageProvider provider) : base(provider)
        {
        }

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

    private sealed class PerfectStrikes : IChangeDiceRoll
    {
        private readonly FeatureDefinition _featureDefinition;

        public PerfectStrikes(FeatureDefinition featureDefinition)
        {
            _featureDefinition = featureDefinition;
        }

        public bool IsValid(
            RollContext rollContext,
            RulesetCharacter rulesetCharacter)
        {
            return rollContext == RollContext.AttackDamageValueRoll && HasSpecializedWeapon(rulesetCharacter);
        }

        public void BeforeRoll(
            RollContext rollContext,
            RulesetCharacter rulesetCharacter,
            ref DieType dieType,
            ref AdvantageType advantageType)
        {
            advantageType = AdvantageType.Advantage;
        }

        public void AfterRoll(
            RollContext rollContext,
            RulesetCharacter rulesetCharacter,
            ref int firstRoll,
            ref int secondRoll)
        {
            GameConsoleHelper.LogCharacterUsedFeature(rulesetCharacter, _featureDefinition);
        }
    }
}
