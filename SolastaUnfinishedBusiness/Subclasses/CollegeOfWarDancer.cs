using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using UnityEngine;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CollegeOfWarDancer : AbstractSubclass
{
    private const string PowerWarDanceName = "CollegeOfWarDancerWarDance";

    internal CollegeOfWarDancer()
    {
        var warDance = FeatureDefinitionPowerBuilder
            .Create(PowerWarDanceName)
            .SetCustomSubFeatures(ValidatorsCharacter.HasMeleeWeaponInMainHand)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.BardicInspiration)
            .SetGuiPresentation(Category.Feature)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBardGiveBardicInspiration)
                .SetEffectForms(EffectFormBuilder
                        .Create()
                        .SetConditionForm(ConditionWarDance, ConditionForm.ConditionOperation.Add)
                        .Build(),
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(ConditionDefinitions.ConditionBardicInspiration,
                            ConditionForm.ConditionOperation.Add)
                        .Build()
                )
                .Build())
            .SetCustomSubFeatures(ValidatorsPowerUse.HasNoCondition(ConditionWarDance.Name))
            .AddToDB();

        var improveWarDance = FeatureDefinitionPowerBuilder
            .Create("CollegeOfWarDancerImprovedWarDance")
            .SetGuiPresentation(Category.Feature)
            .SetTriggeringPower(warDance)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.PerceivingWithinDistance, 6, 6)
                .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
                .SetSavingThrowData(false, AttributeDefinitions.Charisma, false,
                    EffectDifficultyClassComputation.SpellCastingFeature)
                .AddEffectForms(EffectFormBuilder
                    .Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create("ConditionCharmedByDanceOfWar")
                        .SetGuiPresentation(Category.Feature, ConditionDefinitions.ConditionCharmedByHypnoticPattern)
                        .SetFeatures(FeatureDefinitionAttributeModifierBuilder
                            .Create("CharmedByDanceOfWarArmorModifier")
                            .SetGuiPresentation(Category.Feature)
                            .SetModifier(
                                FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddConditionAmount,
                                AttributeDefinitions.ArmorClass)
                            .AddToDB())
                        .SetCustomSubFeatures(new ConditionCharmedByDanceOfWar())
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build()
            )
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("CollegeOfWarDancer")
            .SetOrUpdateGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.RangerSwiftBlade)
            .AddFeaturesAtLevel(3, warDance, CommonBuilders.FeatureSetCasterFightingProficiency)
            .AddFeaturesAtLevel(6, improveWarDance, CommonBuilders.AttributeModifierCasterFightingExtraAttack)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    private static readonly ConditionDefinition ConditionWarDance = BuildConditionWarDance();

    private static ConditionDefinition BuildConditionWarDance()
    {
        return ConditionDefinitionBuilder
            .Create("ConditionWarDance")
            .SetGuiPresentation(Category.Condition)
            .AddFeatures(FeatureDefinitionMovementAffinityBuilder
                    .Create("ConditionWarDanceExtraMovement3")
                    .SetGuiPresentation(Category.Feature)
                    .SetBaseSpeedAdditiveModifier(3)
                    .SetImmunities(false, false, true)
                    .AddToDB(),
                FeatureDefinitionBuilder
                    .Create("WarDanceCustomFeature")
                    .SetGuiPresentation(Category.Feature)
                    .SetCustomSubFeatures(new WarDanceFlurryAttack(), new WarDanceRefundOneAttackOfMainAction(),
                        new ExtendedWarDanceDurationOnKill())
                    .AddToDB(),
                FeatureDefinitionAdditionalDamageBuilder
                    .Create("WarDanceAdditionalDamage")
                    .SetGuiPresentationNoContent(true)
                    .SetDamageValueDetermination(AdditionalDamageValueDetermination.Die)
                    .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
                    .SetTriggerCondition(AdditionalDamageTriggerCondition.AlwaysActive)
                    .SetDamageDice(DieType.D8, 1)
                    .SetSpecificDamageType(DamageTypePsychic)
                    .SetAttackOnly()
                    .AddToDB(),
                FeatureDefinitionBuilder
                    .Create("WarDanceSwitchWeaponFreely")
                    .SetCustomSubFeatures(new SwitchWeaponFreely())
                    .AddToDB()
            )
            .SetCustomSubFeatures(new RemoveOnAttackMissOrAttackWithNonMeleeWeapon())
            .AddToDB();
    }

    private sealed class RemoveOnAttackMissOrAttackWithNonMeleeWeapon
    {
    }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBardColleges;

    private sealed class WarDanceFlurryAttack : IReactToMyAttackFinished
    {
        public IEnumerator HandleReactToMyAttackFinished(GameLocationCharacter me, GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams, RulesetAttackMode mode, ActionModifier modifier)
        {
            if (mode == null || !me.RulesetCharacter.HasConditionOfType(ConditionWarDance))
            {
                yield break;
            }

            if (outcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure) && ValidatorsWeapon.IsMelee(mode))
            {
                yield break;
            }

            RemoveConditionOnAttackMissOrAttackWithNonMeleeWeapon(me.RulesetActor);
        }

        internal static void RemoveConditionOnAttackMissOrAttackWithNonMeleeWeapon(RulesetActor attacker)
        {
            var conditionsToRemove = new List<RulesetCondition>();
            conditionsToRemove.AddRange(
                attacker.ConditionsByCategory
                    .SelectMany(x => x.Value)
                    .Where(x => x.ConditionDefinition
                        .HasSubFeatureOfType<RemoveOnAttackMissOrAttackWithNonMeleeWeapon>()));
            foreach (var conditionToRemove in conditionsToRemove)
            {
                attacker.RemoveCondition(conditionToRemove);
            }
        }
    }

    private static readonly ConditionDefinition WarDanceFlurryAttackPenalty = ConditionDefinitionBuilder
        .Create("WarDanceFlurryAttackModifier")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBaned)
        .SetAllowMultipleInstances(true)
        .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
        .SetCustomSubFeatures(new RemoveOnAttackMissOrAttackWithNonMeleeWeapon())
        .AddToDB();
    
    private sealed class WarDanceRefundOneAttackOfMainAction : IMightRefundOneAttackOfMainAction
    {
        private static readonly ConditionDefinition WarDanceFlurry = ConditionDefinitionBuilder
            .Create("WarDanceFlurry")
            .SetGuiPresentationNoContent(true)
            .SetFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create("WarDanceRefundedActionAffinity")
                    .SetGuiPresentationNoContent(true)
                    .SetDefaultAllowedActionTypes()
                    .SetForbiddenActions(ActionDefinitions.Id.CastMain, ActionDefinitions.Id.PowerMain,
                        ActionDefinitions.Id.UseItemMain, ActionDefinitions.Id.HideMain)
                    .SetCustomSubFeatures(new WarDanceFlurryAttackModifier())
                    .AddToDB())
            .SetCustomSubFeatures(new RemoveOnAttackMissOrAttackWithNonMeleeWeapon())
            .AddToDB();

        public bool MightRefundOneAttackOfMainAction(GameLocationCharacter hero, CharacterActionParams actionParams)
        {
            // spell cantrip is allowed
            if (actionParams.actionDefinition.Id != ActionDefinitions.Id.AttackMain)
            {
                if (actionParams.actionDefinition.Id != ActionDefinitions.Id.CastMain)
                {
                    return false;
                }

                if (actionParams.RulesetEffect is not RulesetEffectSpell spellEffect ||
                    spellEffect.spellDefinition.spellLevel > 0 || !spellEffect.SpellDefinition
                        .HasSubFeatureOfType<IPerformAttackAfterMagicEffectUse>())
                {
                    return false;
                }
            }

            // apply action affinity through condition
            if (!hero.RulesetCharacter.HasConditionOfType(WarDanceFlurry))
            {
                var actionAffinity = RulesetCondition.CreateActiveCondition(
                    hero.RulesetCharacter.Guid,
                    WarDanceFlurry,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.EndOfTurn,
                    hero.RulesetCharacter.Guid,
                    hero.RulesetCharacter.CurrentFaction.Name);
                hero.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, actionAffinity);
            }

            // apply attack modifier
            var attackModifier = RulesetCondition.CreateActiveCondition(
                hero.RulesetCharacter.Guid,
                WarDanceFlurryAttackPenalty,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfTurn,
                hero.RulesetCharacter.Guid,
                hero.RulesetCharacter.CurrentFaction.Name);
            hero.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, attackModifier);

            // Only refund one attack
            var num = actionParams.ActingCharacter.RulesetCharacter.AttackModes
                .Where(attackMode => attackMode.ActionType == ActionDefinitions.ActionType.Main)
                .Aggregate(0, (current, attackMode) => Mathf.Max(current, attackMode.AttacksNumber));
            hero.usedMainAttacks = num - 1;

            return true;
        }
    }

    private sealed class ConditionCharmedByDanceOfWar : ICustomConditionFeature
    {
        public void ApplyFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            Main.Log("ConditionCharmedByDanceOfWar applied", true);
            GameLocationCharacter hero = null;
            if (RulesetEntity.TryGetEntity(rulesetCondition.SourceGuid, out RulesetActor entity))
                hero = GameLocationCharacter.GetFromActor(entity);

            if (hero == null)
            {
                return;
            }

            rulesetCondition.amount = -AttributeDefinitions.ComputeAbilityScoreModifier(
                hero.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Charisma));
            Main.Log($"ConditionCharmedByDanceOfWar applied {rulesetCondition.amount}", true);
        }

        public void RemoveFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
        }
    }

    internal static void OnItemEquipped([NotNull] RulesetCharacter character)
    {
        if (character is RulesetCharacterHero hero && !ValidatorsWeapon.IsMelee(hero.GetMainWeapon()))
        {
            WarDanceFlurryAttack.RemoveConditionOnAttackMissOrAttackWithNonMeleeWeapon(character);
        }
    }

    private sealed class ExtendedWarDanceDurationOnKill : ITargetReducedToZeroHp
    {
        public IEnumerator HandleCharacterReducedToZeroHp(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode, RulesetEffect activeEffect)
        {
            if (attackMode == null || activeEffect != null ||
                !attacker.RulesetCharacter.HasConditionOfType(ConditionWarDance) ||
                !ValidatorsWeapon.IsMelee(attackMode))
            {
                yield break;
            }

            if (!attacker.RulesetCharacter.TryGetConditionOfCategoryAndType(AttributeDefinitions.TagEffect,
                    ConditionWarDance.Name, out var activeCondition))
            {
                yield break;
            }

            activeCondition.remainingRounds += 1;
            activeCondition.endOccurence = TurnOccurenceType.EndOfTurn;

            foreach (var power in attacker.RulesetCharacter.powersUsedByMe.Where(power =>
                         power.Name == PowerWarDanceName))
            {
                power.remainingRounds += 1;
                break;
            }
        }
    }
    
    private static readonly FeatureDefinition WarDanceFlurryAttackModifierMinus3 = FeatureDefinitionBuilder
        .Create("WarDanceFlurryAttackModifierMinus3")
        .SetGuiPresentation(Category.Feature)
        .AddToDB();

    private sealed class WarDanceFlurryAttackModifier : IModifyAttackModeForWeapon
    {
        private const int ToHitModifier = -3;

        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            Main.Log($"ModifyAttackMode enter", true);
            if (attackMode == null || ValidatorsWeapon.IsRanged(attackMode))
            {
                Main.Log($"ModifyAttackMode return 1", true);
                return;
            }

            var conditions = new List<RulesetCondition>();
            conditions.AddRange(
                character.ConditionsByCategory
                    .SelectMany(x => x.Value)
                    .Where(x => x.ConditionDefinition == WarDanceFlurryAttackPenalty));
                
            if (conditions.Count <= 0)
            {
                Main.Log($"ModifyAttackMode return 2", true);
                return;
            }

            Main.Log($"{WarDanceFlurryAttackModifierMinus3} {conditions.Count}", true);
            var toHit = conditions.Count * ToHitModifier;
            attackMode.ToHitBonus += toHit;
            var trendInfo = new TrendInfo(toHit, FeatureSourceType.Feat,
                WarDanceFlurryAttackModifierMinus3.name, WarDanceFlurryAttackModifierMinus3);
            var index = attackMode.ToHitBonusTrends.IndexOf(trendInfo);
            if (index == -1)
            {
                attackMode.ToHitBonusTrends.Add(trendInfo);
            }
            else
            {
                attackMode.ToHitBonusTrends[index] = trendInfo;
            }
        }
    }

    internal sealed class SwitchWeaponFreely
    {
        
    }
}
