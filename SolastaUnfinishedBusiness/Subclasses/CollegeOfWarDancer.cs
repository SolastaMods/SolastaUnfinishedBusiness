using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
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
            .AddToDB();

        var improveWarDance = FeatureDefinitionPowerBuilder
            .Create("CollegeOfWarDancerImprovedWarDance")
            .SetGuiPresentation(Category.Feature)
            .SetTriggeringPower(warDance)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.PerceivingWithinDistance, 6, 6)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
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
            .AddFeaturesAtLevel(3, warDance, CommonBuilders.FeatureSetCasterFightingProficiency, improveWarDance)
            .AddFeaturesAtLevel(6, improveWarDance)
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
                .SetGuiPresentationNoContent(true)
                .SetBaseSpeedAdditiveModifier(3)
                .SetImmunities(false, false, true)
                .SetCustomSubFeatures(new WarDanceFlurryAttack(), new RefundOneAttack(), new ExtendedWarDanceDurationOnKill())
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
        private static readonly FeatureDefinitionAttackModifier WarDanceFlurryBaneAttackModifier =
            BuildWarDanceFlurryBaneAttackModifier();

        private static FeatureDefinitionAttackModifier BuildWarDanceFlurryBaneAttackModifier()
        {
            return FeatureDefinitionAttackModifierBuilder
                .Create("WarDanceBaneAttackModifier")
                .SetGuiPresentation(Category.Feature)
                .SetAttackRollModifier(-3)
                .AddToDB();
        }

        private static readonly ConditionDefinition ConditionWarDanceFlurryBane = BuildConditionWarDanceFlurryBane();

        private static ConditionDefinition BuildConditionWarDanceFlurryBane()
        {
            return ConditionDefinitionBuilder
                .Create("WarDanceBane")
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBaned)
                .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
                .SetFeatures(WarDanceFlurryBaneAttackModifier,
                    FeatureDefinitionActionAffinityBuilder
                        .Create("WarDanceBaneActionAffinity")
                        .SetDefaultAllowedActionTypes()
                        .SetForbiddenActions(ActionDefinitions.Id.CastMain, ActionDefinitions.Id.DashMain,
                            ActionDefinitions.Id.Dodge, ActionDefinitions.Id.DisengageMain,
                            ActionDefinitions.Id.PowerMain, ActionDefinitions.Id.UseItemMain,
                            ActionDefinitions.Id.HideMain)
                        .AddToDB()
                )
                .SetCustomSubFeatures(new RemoveOnAttackMissOrAttackWithNonMeleeWeapon())
                .SetCustomSubFeatures(ValidatorsPowerUse.HasNoCondition(ConditionWarDance.Name))
                .AddToDB();
        }

        public IEnumerator HandleReactToMyAttackFinished(GameLocationCharacter me, GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams, RulesetAttackMode mode, ActionModifier modifier)
        {
            if (mode == null || !me.RulesetCharacter.HasConditionOfType(ConditionWarDance))
            {
                yield break;
            }

            if (outcome is RollOutcome.Failure or RollOutcome.CriticalFailure || !ValidatorsWeapon.IsMelee(mode))
            {
                RemoveConditionOnAttackMissOrAttackWithNonMeleeWeapon(me.RulesetActor);
                yield break;
            }

            InflictSwordFlurryBane(me);
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

        private static void InflictSwordFlurryBane(GameLocationCharacter attacker)
        {
            var rulesetCondition =
                attacker.RulesetCharacter.FindFirstConditionHoldingFeature(WarDanceFlurryBaneAttackModifier);
            if (rulesetCondition == null)
            {
                WarDanceFlurryBaneAttackModifier.attackRollModifier = -3;
                rulesetCondition = RulesetCondition.CreateActiveCondition(
                    attacker.RulesetCharacter.Guid,
                    ConditionWarDanceFlurryBane,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    attacker.RulesetCharacter.Guid,
                    attacker.RulesetCharacter.CurrentFaction.Name);

                attacker.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
            }
            else
            {
                foreach (var f2 in from f in rulesetCondition.conditionDefinition.features
                         select f as FeatureDefinitionAttackModifier
                         into f2
                         where f2 == WarDanceFlurryBaneAttackModifier
                         where f2 != null
                         select f2)
                {
                    f2.attackRollModifier += -3;
                    break;
                }
            }
        }
    }

    private sealed class RefundOneAttack : IRefundOneAttack
    {
    }

    private sealed class ConditionCharmedByDanceOfWar : ICustomConditionFeature
    {
        public void ApplyFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            GameLocationCharacter hero = null;
            if (RulesetEntity.TryGetEntity(rulesetCondition.SourceGuid, out RulesetActor entity))
                hero = GameLocationCharacter.GetFromActor(entity);

            if (hero != null)
            {
                rulesetCondition.amount = -AttributeDefinitions.ComputeAbilityScoreModifier(
                    hero.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Charisma));
            }
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
            activeCondition.endOccurence =TurnOccurenceType.EndOfTurn;

            foreach (var power in attacker.RulesetCharacter.powersUsedByMe.Where(power => power.Name == PowerWarDanceName))
            {
                power.remainingRounds += 1;
                break;
            }
        }
    }
}
