using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
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
            .AddToDB())
        .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
        .SetCustomSubFeatures(new RemoveOnAttackMissOrAttackWithNonMeleeWeapon())
        .AddToDB();

    private static readonly ConditionDefinition MomentumAlreadyApplied = ConditionDefinitionBuilder
        .Create("ConditionWarDanceMomentumAlreadyApplied")
        .SetGuiPresentationNoContent()
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
        .AddToDB();

    internal CollegeOfWarDancer()
    {
        var warDance = FeatureDefinitionPowerBuilder
            .Create(PowerWarDanceName)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.BardicInspiration)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.MagicWeapon)
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
            .SetCustomSubFeatures(
                ValidatorsCharacter.MainHandIsMeleeWeapon,
                ValidatorsPowerUse.HasNoneOfConditions(ConditionWarDance.Name),
                new WarDanceRefundOneAttackOfMainAction())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("CollegeOfWarDancer")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("CollegeOfWarDancer", Resources.CollegeOfWarDancer, 256))
            .AddFeaturesAtLevel(3, warDance, CommonBuilders.FeatureSetCasterFightingProficiency,
                CommonBuilders.MagicAffinityCasterFightingCombatMagic)
            .AddFeaturesAtLevel(6, ImproveWarDance)
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
                    .SetCustomSubFeatures(new WarDanceFlurryAttack(),
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
                    .AddAbilityScoreBonus(AttributeDefinitions.Charisma)
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
        var momentum = character.ConditionsByCategory
            .SelectMany(x => x.Value)
            .Count(x => x.ConditionDefinition == WarDanceMomentum);

        var item = character.GetMainWeapon();

        if (item == null || !item.itemDefinition.isWeapon)
        {
            return 0;
        }

        var isLight = item.itemDefinition.WeaponDescription.WeaponTags.Contains(TagsDefinitions.WeaponTagLight);

        if (isLight)
        {
            return momentum;
        }

        var isHeavy = item.itemDefinition.WeaponDescription.WeaponTags.Contains(TagsDefinitions.WeaponTagHeavy);

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
            WarDanceFlurryAttack.RemoveConditionOnAttackMissOrAttackWithNonMeleeWeapon(character);
        }
    }

    private static bool RemoveMomentumAnyway(IControllableCharacter hero)
    {
        var currentMomentum = new List<RulesetCondition>();
        var pb = hero.RulesetCharacter.TryGetAttributeValue("ProficiencyBonus");

        currentMomentum.AddRange(
            hero.RulesetCharacter.ConditionsByCategory
                .SelectMany(x => x.Value)
                .Where(x => x.conditionDefinition == WarDanceMomentum));

        return currentMomentum.Count >= (pb + 1) / 2 || pb == 0;
    }

    private sealed class RemoveOnAttackMissOrAttackWithNonMeleeWeapon
    {
    }

    private sealed class WarDanceFlurryAttack : IAttackFinished
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
                var reactionParams =
                    new CharacterActionParams(attacker, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                    {
                        StringParameter = description
                    };
                var reactionRequest = new ReactionRequestCustom("DanceOfWarOnMiss", reactionParams);

                var service = ServiceRepository.GetService<IGameLocationActionService>();
                var count = service.PendingReactionRequestGroups.Count;

                manager.AddInterruptRequest(reactionRequest);
                yield return battleManager.WaitForReactions(attacker, service, count);

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

    private sealed class WarDanceRefundOneAttackOfMainAction : IMightRefundOneAttackOfMainAction
    {
        private static readonly ConditionDefinition WarDanceMomentumExtraAction = ConditionDefinitionBuilder
            .Create("ConditionWarDanceMomentumExtraAction")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create("ActionAffinityWarDanceExtraAction")
                    .SetGuiPresentationNoContent(true)
                    .SetMaxAttackNumber(1)
                    .SetForbiddenActions(ActionDefinitions.Id.CastMain, ActionDefinitions.Id.PowerMain,
                        ActionDefinitions.Id.UseItemMain, ActionDefinitions.Id.HideMain, ActionDefinitions.Id.Ready)
                    .SetCustomSubFeatures(new WarDanceFlurryAttackModifier())
                    .AddToDB())
            .AddToDB();

        private static readonly ConditionDefinition ImprovedWarDanceMomentumExtraAction = ConditionDefinitionBuilder
            .Create("ConditionImprovedWarDanceMomentumExtraAction")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create("ActionAffinityImprovedWarDanceExtraAction")
                    .SetGuiPresentationNoContent(true)
                    .SetMaxAttackNumber(1)
                    .SetForbiddenActions(ActionDefinitions.Id.PowerMain,
                        ActionDefinitions.Id.UseItemMain, ActionDefinitions.Id.HideMain, ActionDefinitions.Id.Ready)
                    .SetCustomSubFeatures(new WarDanceFlurryAttackModifier())
                    .AddToDB())
            .AddToDB();

        public bool MightRefundOneAttackOfMainAction(GameLocationCharacter hero, CharacterActionParams actionParams)
        {
            var flag = true;
            var momentum = 1;
            if (actionParams.actionDefinition.Id != ActionDefinitions.Id.AttackMain)
            {
                if (actionParams.actionDefinition.Id != ActionDefinitions.Id.CastMain)
                {
                    flag = false;
                }
                else
                {
                    if (actionParams.activeEffect is RulesetEffectSpell spellEffect)
                    {
                        if (spellEffect.slotLevel > 0 || spellEffect.SpellDefinition
                                .HasSubFeatureOfType<IPerformAttackAfterMagicEffectUse>())
                        {
                            if (spellEffect.slotLevel / 2 > momentum)
                            {
                                momentum = spellEffect.slotLevel / 2;
                            }
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                }
            }

            // only apply if number of momentum less than PB
            var currentMomentum = new List<RulesetCondition>();
            var pb = hero.RulesetCharacter.TryGetAttributeValue("ProficiencyBonus");
            currentMomentum.AddRange(
                hero.RulesetCharacter.ConditionsByCategory
                    .SelectMany(x => x.Value)
                    .Where(x => x.conditionDefinition == WarDanceMomentumExtraAction ||
                                x.conditionDefinition == ImprovedWarDanceMomentumExtraAction ||
                                x.conditionDefinition == WarDanceMomentum));

            if (!flag || RemoveMomentumAnyway(hero) || pb == 0 ||
                !hero.RulesetCharacter.HasConditionOfType(ConditionWarDance) ||
                hero.RulesetCharacter.HasConditionOfType(MomentumAlreadyApplied))
            {
                foreach (var cond in currentMomentum)
                {
                    hero.RulesetCharacter.RemoveCondition(cond);
                }

                var marked = RulesetCondition.CreateActiveCondition(
                    hero.Guid,
                    MomentumAlreadyApplied,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.StartOfTurn,
                    hero.Guid,
                    hero.RulesetCharacter.CurrentFaction.Name);

                hero.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, marked);
                return false;
            }

            // apply action affinity
            hero.UsedMainSpell = true;
            hero.UsedMainCantrip = false;
            ApplyActionAffinity(hero.RulesetCharacter);

            // apply momentum
            for (var i = 0; i < momentum; i++)
            {
                GrantWarDanceMomentum(hero);
            }

            return true;
        }

        private static void ApplyActionAffinity(RulesetCharacter character)
        {
            if (character is not RulesetCharacterHero)
            {
                return;
            }

            var condition = WarDanceMomentumExtraAction;

            if (character.HasAnyFeature(ImproveWarDance))
            {
                condition = ImprovedWarDanceMomentumExtraAction;
            }

            if (character.HasConditionOfType(condition))
            {
                return;
            }

            var actionAffinity = RulesetCondition.CreateActiveCondition(
                character.Guid,
                condition,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                character.Guid,
                character.CurrentFaction.Name);

            character.AddConditionOfCategory(AttributeDefinitions.TagCombat, actionAffinity);
        }

        private static void GrantWarDanceMomentum(IControllableCharacter hero)
        {
            // required for wildshape scenarios
            if (hero.RulesetCharacter is not RulesetCharacterHero)
            {
                return;
            }

            var item = hero.RulesetCharacter.GetMainWeapon();

            if (item == null || !item.itemDefinition.isWeapon)
            {
                return;
            }

            var attackModifier = RulesetCondition.CreateActiveCondition(
                hero.RulesetCharacter.Guid,
                WarDanceMomentum,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                hero.RulesetCharacter.Guid,
                hero.RulesetCharacter.CurrentFaction.Name);

            hero.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, attackModifier);
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

    private sealed class WarDanceFlurryAttackModifier : IModifyAttackModeForWeapon
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

            var momentum = character.ConditionsByCategory
                .SelectMany(x => x.Value)
                .Count(x => x.ConditionDefinition == WarDanceMomentum);

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
}
