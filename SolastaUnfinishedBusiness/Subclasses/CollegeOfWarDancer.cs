using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CollegeOfWarDancer : AbstractSubclass
{
    private const string PowerWarDanceName = "PowerWarDancerWarDance";

    private static readonly FeatureDefinition ImproveWarDance = FeatureDefinitionBuilder
        .Create("FeatureCollegeOfWarDancerImprovedWarDance")
        .SetGuiPresentation(Category.Feature)
        .AddToDB();

    private static readonly ConditionDefinition ConditionWarDance = BuildConditionWarDance();

    private static readonly DamageDieProvider UpgradeDice = (character, _) => GetMomentumDice(character);

    private static readonly DieNumProvider UpgradeDieNum = (character, _) => GetMomentumDiceNum(character);

    private static readonly ConditionDefinition WarDanceMomentum = ConditionDefinitionBuilder
        .Create("ConditionWarDanceMomentum")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeraldOfBattle)
        .AllowMultipleInstances()
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetFeatures(FeatureDefinitionAdditionalDamageBuilder
                .Create("AdditionalDamageWarDanceMomentum")
                .SetGuiPresentationNoContent(true)
                .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
                .SetDamageDice(DieType.D6, 2)
                .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
                .SetAttackModeOnly()
                .SetTriggerCondition(AdditionalDamageTriggerCondition.AlwaysActive)
                .SetNotificationTag("Momentum")
                .SetCustomSubFeatures(UpgradeDice, UpgradeDieNum)
                .AddToDB(),
            FeatureDefinitionAdditionalActionBuilder
                .Create("AdditionalActionWarDanceMomentum")
                .SetCustomSubFeatures(AllowDuplicates.Mark)
                .SetActionType(ActionType.Main)
                .SetMaxAttacksNumber(1)
                .SetRestrictedActions(Id.AttackMain, Id.CastMain, Id.Shove, Id.DashMain, Id.DisengageMain)
                .AddToDB())
        .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
        .SetCustomSubFeatures(new RemoveOnAttackMissOrAttackWithNonMeleeWeapon())
        .AddToDB();

    internal CollegeOfWarDancer()
    {
        // BEGIN BACKWARD COMPATIBILITY
        _ = ConditionDefinitionBuilder
            .Create("ConditionWarDanceMomentumAlreadyApplied")
            .SetGuiPresentationNoContent()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();
        // END BACKWARD COMPATIBILITY

        var warDance = FeatureDefinitionPowerBuilder
            .Create(PowerWarDanceName)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.BardicInspiration)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.MagicWeapon)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBardGiveBardicInspiration)
                .SetEffectForms(
                    EffectFormBuilder.ConditionForm(ConditionWarDance),
                    EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionBardicInspiration)
                )
                .Build())
            .SetCustomSubFeatures(
                ValidatorsCharacter.HasMeleeWeaponInMainHand,
                ValidatorsPowerUse.HasNoneOfConditions(ConditionWarDance.Name),
                new WarDanceMomentumExtraAttacks())
            .AddToDB();

        var focusedWarDance = FeatureDefinitionBuilder
            .Create("FeatureCollegeOfWarDancerFocusedWarDance")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new FocusedWarDance())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("CollegeOfWarDancer")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("CollegeOfWarDancer", Resources.CollegeOfWarDancer, 256))
            .AddFeaturesAtLevel(3,
                warDance,
                CommonBuilders.FeatureSetCasterFightingProficiency,
                CommonBuilders.MagicAffinityCasterFightingCombatMagic)
            .AddFeaturesAtLevel(6, ImproveWarDance)
            .AddFeaturesAtLevel(14, focusedWarDance)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBardColleges;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static ConditionDefinition BuildConditionWarDance()
    {
        return ConditionDefinitionBuilder
            .Create("ConditionWarDance")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionRaging)
            .AddFeatures(FeatureDefinitionMovementAffinityBuilder
                    .Create("MovementAffinityConditionWarDanceExtraMovement3")
                    .SetGuiPresentationNoContent(true)
                    .SetBaseSpeedAdditiveModifier(3)
                    .SetImmunities(false, false, true)
                    .AddToDB(),
                FeatureDefinitionBuilder
                    .Create("FeatureConditionWarDanceCustom")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(new WarDanceFlurryPhysicalAttack(),
                        new ExtendedWarDanceDurationOnKill())
                    .AddToDB(),
                FeatureDefinitionBuilder
                    .Create("FeatureConditionWarDanceSwitchWeaponFreely")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(new SwitchWeaponFreely())
                    .AddToDB(),
                FeatureDefinitionAttackModifierBuilder
                    .Create("AttackModifierWarDance")
                    .SetGuiPresentation(Category.Feature)
                    .SetAttackRollModifier(0, AttackModifierMethod.AddAbilityScoreBonus, AttributeDefinitions.Charisma)
                    .AddToDB()
            )
            .SetCustomSubFeatures(new RemoveOnAttackMissOrAttackWithNonMeleeWeapon())
            .AddToDB();
    }

    private static DieType GetMomentumDice(RulesetCharacter character)
    {
        var item = character.GetMainWeapon();

        if (item == null || !item.itemDefinition.isWeapon)
        {
            return DieType.D1;
        }

        var isLight = item.itemDefinition.WeaponDescription.WeaponTags.Contains(TagsDefinitions.WeaponTagLight);

        if (isLight)
        {
            return DieType.D6;
        }

        var isHeavy = item.itemDefinition.WeaponDescription.WeaponTags.Contains(TagsDefinitions.WeaponTagHeavy);

        return isHeavy ? DieType.D10 : DieType.D8;
    }

    private static int GetMomentumDiceNum(RulesetCharacter character)
    {
        var momentum = GetMomentumStacks(character);

        var item = character.GetMainWeapon();

        if (item == null || !item.itemDefinition.isWeapon)
        {
            return 0;
        }

        var isLight = ValidatorsWeapon.HasAnyWeaponTag(item.itemDefinition, TagsDefinitions.WeaponTagLight);

        if (isLight)
        {
            return momentum;
        }

        var isHeavy = ValidatorsWeapon.HasAnyWeaponTag(item.itemDefinition, TagsDefinitions.WeaponTagHeavy);

        if (isHeavy)
        {
            return 2 * momentum;
        }

        return momentum + 1;
    }

    internal static void OnItemEquipped([NotNull] RulesetCharacter character)
    {
        if (character is RulesetCharacterHero hero && !ValidatorsWeapon.IsMelee(hero.GetMainWeapon()))
        {
            WarDanceFlurryPhysicalAttack.RemoveConditionOnAttackMissOrAttackWithNonMeleeWeapon(character);
        }
    }

    private static bool RemoveMomentumAnyway(IControllableCharacter hero)
    {
        var rulesetCharacter = hero?.RulesetCharacter;
        if (rulesetCharacter == null)
        {
            return true;
        }

        var pb = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
        if (pb == 0)
        {
            return true;
        }

        return GetMomentumStacks(rulesetCharacter) >= (pb + 1) / 2;
    }

    private static int GetMomentumStacks(RulesetActor character)
    {
        return character?.ConditionsByCategory
            .SelectMany(x => x.Value)
            .Count(x => x.conditionDefinition == WarDanceMomentum) ?? 0;
    }

    private sealed class RemoveOnAttackMissOrAttackWithNonMeleeWeapon
    {
    }

    private sealed class WarDanceFlurryPhysicalAttack : IPhysicalAttackFinished
    {
        private const string Format = "Reaction/&CustomReactionDanceOfWarOnMissDescription";

        public IEnumerator OnAttackFinished(GameLocationBattleManager battleManager, CharacterAction action,
            GameLocationCharacter attacker, GameLocationCharacter defender, RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome, int damageAmount)
        {
            if (attackerAttackMode == null || !attacker.RulesetCharacter.HasConditionOfType(ConditionWarDance))
            {
                yield break;
            }

            if (attackRollOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure) &&
                ValidatorsWeapon.IsMelee(attackerAttackMode))
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            if (!RemoveMomentumAnyway(attacker) && attacker.RulesetCharacter.RemainingBardicInspirations > 0 &&
                manager != null)
            {
                // spend bardic inspiration dice to keep war dance condition
                var description = Gui.Format(Format, attacker.Name);
                var reactionParams = new CharacterActionParams(attacker, (Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = description
                };
                var reactionRequest = new ReactionRequestCustom("DanceOfWarOnMiss", reactionParams);

                var count = manager.PendingReactionRequestGroups.Count;

                manager.AddInterruptRequest(reactionRequest);
                yield return battleManager.WaitForReactions(attacker, manager, count);

                if (reactionParams.ReactionValidated)
                {
                    attacker.RulesetCharacter.usedBardicInspiration += 1;
                    yield break;
                }
            }

            RemoveConditionOnAttackMissOrAttackWithNonMeleeWeapon(attacker.RulesetCharacter);
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

    private sealed class WarDanceMomentumExtraAttacks : IActionExecutionHandled
    {
        private static readonly ConditionDefinition WarDanceMomentumExtraAction = ConditionDefinitionBuilder
            .Create("ConditionWarDanceMomentumExtraAction")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create("ActionAffinityWarDanceExtraAction")
                    .SetGuiPresentationNoContent(true)
                    .SetForbiddenActions(Id.CastMain)
                    .SetCustomSubFeatures(new WarDanceFlurryWeaponAttackModifier())
                    .AddToDB())
            .AddSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        private static readonly ConditionDefinition ImprovedWarDanceMomentumExtraAction = ConditionDefinitionBuilder
            .Create("ConditionImprovedWarDanceMomentumExtraAction")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionBuilder //using old name to not break characters that are under war dance when update hits
                    .Create("ActionAffinityImprovedWarDanceExtraAction")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(new WarDanceFlurryWeaponAttackModifier())
                    .AddToDB())
            .AddSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();


        public void OnActionExecutionHandled(
            GameLocationCharacter hero,
            CharacterActionParams actionParams,
            ActionScope scope)
        {
            var actionDefinition = actionParams.ActionDefinition;
            var rulesetHero = hero.RulesetCharacter;

            //Wrong scope or type of action, skip
            if (scope != ActionScope.Battle
                || actionDefinition.ActionType != ActionType.Main)
            {
                return;
            }

            //Still has attacks, skip
            if (hero.GetActionAvailableIterations(Id.AttackMain) > 0)
            {
                return;
            }

            //Wrong action after momentum started, skip
            if (actionDefinition.Id is not (Id.AttackMain or Id.CastMain) && GetMomentumStacks(rulesetHero) > 0)
            {
                return;
            }

            //No active War Dance or too many Momentum stacks, skip
            if (!rulesetHero.HasConditionOfType(ConditionWarDance) || RemoveMomentumAnyway(hero))
            {
                return;
            }

            // apply action affinity
            hero.UsedMainSpell = true;
            hero.UsedMainCantrip = false;
            ApplyActionAffinity(hero.RulesetCharacter);

            GrantWarDanceMomentum(hero);

            var text = WarDanceMomentum.GuiPresentation.Title;
            rulesetHero.ShowLabel(text, Gui.ColorPositive);
            GameConsoleHelper.LogCharacterActivatesAbility(rulesetHero, text, "Feedback/&ActivateMomentum", true);
        }

        private static void ApplyActionAffinity(RulesetCharacter rulesetCharacter)
        {
            if (rulesetCharacter is not RulesetCharacterHero)
            {
                return;
            }

            var condition = WarDanceMomentumExtraAction;

            if (rulesetCharacter.HasAnyFeature(ImproveWarDance))
            {
                condition = ImprovedWarDanceMomentumExtraAction;
            }

            if (rulesetCharacter.HasConditionOfType(condition))
            {
                return;
            }

            rulesetCharacter.InflictCondition(
                condition.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }

        private static void GrantWarDanceMomentum(IControllableCharacter hero)
        {
            // required for wildshape scenarios
            var rulesetCharacter = hero.RulesetCharacter;

            if (rulesetCharacter is not RulesetCharacterHero)
            {
                return;
            }

            var item = hero.RulesetCharacter.GetMainWeapon();

            if (item == null || !item.itemDefinition.isWeapon)
            {
                return;
            }

            rulesetCharacter.InflictCondition(
                WarDanceMomentum.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }

    private sealed class ExtendedWarDanceDurationOnKill : ITargetReducedToZeroHp
    {
        public IEnumerator HandleCharacterReducedToZeroHp(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode, RulesetEffect activeEffect)
        {
            // activeEffect != null means a magical attack
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

    private sealed class WarDanceFlurryWeaponAttackModifier : IModifyWeaponAttackMode
    {
        private const int LightMomentumModifier = -2;
        private const int MomentumModifier = -3;
        private const int HeavyMomentumModifier = -4;

        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (attackMode == null || !ValidatorsWeapon.IsMelee(attackMode))
            {
                return;
            }

            var momentum = GetMomentumStacks(character);

            if (momentum == 0)
            {
                return;
            }

            ApplyMomentum(momentum, attackMode);
        }

        private static void ApplyMomentum(int momentum, RulesetAttackMode attackMode)
        {
            if (attackMode == null)
            {
                return;
            }

            //this.sourceDefinition is ItemDefinition sourceDefinition && sourceDefinition.IsWeapon && sourceDefinition.WeaponDescription.WeaponTags.Contains("Light");
            var item = attackMode.sourceDefinition as ItemDefinition;

            if (item == null || !item.IsWeapon)
            {
                return;
            }

            var isLight = item.WeaponDescription.WeaponTags.Contains("Light");
            var isHeavy = item.WeaponDescription.WeaponTags.Contains("Heavy");
            var toHit = -1;

            if (isLight)
            {
                toHit += LightMomentumModifier * momentum;
            }
            else if (isHeavy)
            {
                toHit += HeavyMomentumModifier * momentum;
            }
            else
            {
                toHit += MomentumModifier * momentum;
            }

            attackMode.toHitBonus += toHit;

            var trendInfo = new TrendInfo(toHit, FeatureSourceType.CharacterFeature,
                Gui.Localize("Feedback/&AdditionalDamageMomentumFormat"), null);
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

    private sealed class SwitchWeaponFreely : IUnlimitedFreeAction
    {
    }

    private sealed class FocusedWarDance : IChangeConcentrationAttribute
    {
        public bool IsValid(RulesetActor rulesetActor)
        {
            return rulesetActor.HasAnyConditionOfType(ConditionWarDance.Name);
        }

        public string ConcentrationAttribute(RulesetActor rulesetActor)
        {
            return AttributeDefinitions.Charisma;
        }
    }
}
