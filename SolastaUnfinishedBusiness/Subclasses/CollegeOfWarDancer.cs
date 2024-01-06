using System.Collections;
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
using static RuleDefinitions;
using static ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class CollegeOfWarDancer : AbstractSubclass
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
        .SetPossessive()
        .SetFeatures(
            FeatureDefinitionAdditionalDamageBuilder
                .Create("AdditionalDamageWarDanceMomentum")
                .SetGuiPresentationNoContent(true)
                .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
                .SetDamageDice(DieType.D6, 2)
                .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
                .SetAttackModeOnly()
                .SetIgnoreCriticalDoubleDice(true)
                .SetTriggerCondition(AdditionalDamageTriggerCondition.AlwaysActive)
                .SetNotificationTag("Momentum")
                .AddCustomSubFeatures(UpgradeDice, UpgradeDieNum)
                .AddToDB())
        .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
        .AddToDB();

    private static readonly ConditionDefinition WarDanceExtraAttack = ConditionDefinitionBuilder
        .Create("ConditionWarDanceExtraAttack")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .AllowMultipleInstances()
        .SetFeatures(
            FeatureDefinitionAdditionalActionBuilder
                .Create("AdditionalActionWarDanceMomentum")
                .SetGuiPresentation(ImproveWarDance.GuiPresentation)
                .AddCustomSubFeatures(AllowDuplicates.Mark, AdditionalActionAttackValidator.MeleeOnly)
                .SetActionType(ActionType.Main)
                .SetMaxAttacksNumber(1)
                .SetRestrictedActions(Id.AttackMain)
                .AddToDB())
        .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
        .AddToDB();

    private static readonly ConditionDefinition WarDanceMissedAttack = ConditionDefinitionBuilder
        .Create("ConditionWarDanceMomentumAlreadyApplied") //name kept from old code for backwards compatibility
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
        .AddToDB();

    public CollegeOfWarDancer()
    {
        var warDance = FeatureDefinitionPowerBuilder
            .Create(PowerWarDanceName)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.BardicInspiration)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.MagicWeapon)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBardGiveBardicInspiration)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(ConditionWarDance),
                        EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionBardicInspiration))
                    .Build())
            .AddCustomSubFeatures(EffectWithConcentrationCheck.Mark,
                ValidatorsValidatePowerUse.HasNoneOfConditions(ConditionWarDance.Name))
            .AddToDB();

        var focusedWarDance = FeatureDefinitionBuilder
            .Create("FeatureCollegeOfWarDancerFocusedWarDance")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new FocusedWarDance())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("CollegeOfWarDancer")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("CollegeOfWarDancer", Resources.CollegeOfWarDancer, 256))
            .AddFeaturesAtLevel(3,
                warDance, FeatureSetCasterFightingProficiency, MagicAffinityCasterFightingCombatMagic)
            .AddFeaturesAtLevel(6, ImproveWarDance)
            .AddFeaturesAtLevel(14, focusedWarDance)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Bard;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBardColleges;

    internal override DeityDefinition DeityDefinition => null;

    private static ConditionDefinition BuildConditionWarDance()
    {
        return ConditionDefinitionBuilder
            .Create("ConditionWarDance")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionRaging)
            .AddFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create("MovementAffinityConditionWarDanceExtraMovement3")
                    .SetGuiPresentation("ConditionWarDance", Category.Condition, Gui.NoLocalization)
                    .SetBaseSpeedAdditiveModifier(3)
                    .SetImmunities(false, false, true)
                    .AddToDB(),
                FeatureDefinitionAttackModifierBuilder
                    .Create("AttackModifierWarDance")
                    .SetGuiPresentation("ConditionWarDance", Category.Condition, Gui.NoLocalization)
                    .SetAttackRollModifier(0, AttackModifierMethod.AddAbilityScoreBonus, AttributeDefinitions.Charisma)
                    .AddCustomSubFeatures(
                        FreeWeaponSwitching.Mark,
                        new StopMomentumAndAttacksWhenRemoved(),
                        new WarDanceFlurryPhysicalAttack(),
                        new WarDanceFlurryWeaponAttackModifier(),
                        new WarDanceExtraAttacks())
                    .AddToDB())
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

    private static int GetMomentumStacks(RulesetActor character)
    {
        return character?.ConditionsByCategory
            .SelectMany(x => x.Value)
            .Count(x => x.conditionDefinition == WarDanceMomentum) ?? 0;
    }

    private static int GetExtraAttacks(RulesetActor character)
    {
        return character?.ConditionsByCategory
            .SelectMany(x => x.Value)
            .Count(x => x.conditionDefinition == WarDanceExtraAttack) ?? 0;
    }

    private static void GrantWarDanceCondition(RulesetCharacter rulesetCharacter, BaseDefinition condition)
    {
        if (rulesetCharacter is not RulesetCharacterHero)
        {
            return;
        }

        rulesetCharacter.InflictCondition(
            condition.Name,
            DurationType.Round,
            1,
            TurnOccurenceType.StartOfTurn,
            AttributeDefinitions.TagEffect,
            rulesetCharacter.guid,
            rulesetCharacter.CurrentFaction.Name,
            1,
            condition.Name,
            0,
            0,
            0);
    }

    private sealed class WarDanceFlurryPhysicalAttack : IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(GameLocationBattleManager battleManager, CharacterAction action,
            GameLocationCharacter attacker, GameLocationCharacter defender, RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome, int damageAmount)
        {
            var rulesetCharacter = attacker.RulesetCharacter;

            if (attackerAttackMode == null || rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (action.ActionType == ActionType.Reaction)
            {
                yield break;
            }

            if (!rulesetCharacter.HasConditionOfType(ConditionWarDance))
            {
                yield break;
            }

            if (!ValidatorsWeapon.IsMelee(attackerAttackMode))
            {
                yield break;
            }

            if (attackRollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                GrantWarDanceCondition(rulesetCharacter, WarDanceMissedAttack);
                yield break;
            }

            GrantWarDanceCondition(rulesetCharacter, WarDanceMomentum);
            rulesetCharacter.ShowLabel(WarDanceMomentum.GuiPresentation.Title, Gui.ColorPositive);
            rulesetCharacter.LogCharacterUsedFeature(WarDanceMomentum, "Feedback/&ActivateWarDancerMomentum", true);
        }
    }

    private sealed class WarDanceExtraAttacks : IActionExecutionHandled
    {
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

            //Character isn't War Dancing or doesn't have Improved War Dance, skip
            if (!rulesetHero.HasConditionOfType(ConditionWarDance)
                || !rulesetHero.HasAnyFeature(ImproveWarDance))
            {
                return;
            }

            //Still has attacks, skip
            if (hero.GetActionAvailableIterations(Id.AttackMain) > 0)
            {
                return;
            }

            //Wrong action or weapon or a miss after momentum started, skip
            var extraAttacks = GetExtraAttacks(rulesetHero);
            var wrongAction = actionDefinition.Id is not Id.AttackMain;
            var wrongWeapon = !ValidatorsWeapon.IsMelee(actionParams.attackMode);
            var missed = rulesetHero.RemoveAllConditionsOfType(WarDanceMissedAttack.Name);

            if (extraAttacks > 0 && (wrongAction || wrongWeapon || missed))
            {
                return;
            }

            //Too many extra attacks, skip
            var maxAttacks = rulesetHero.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus) / 2;

            if (extraAttacks >= maxAttacks)
            {
                return;
            }

            GrantWarDanceCondition(rulesetHero, WarDanceExtraAttack);
            rulesetHero.LogCharacterUsedFeature(ImproveWarDance, "Feedback/&ActivateWarDancerAttack", true);
        }
    }

    private sealed class WarDanceFlurryWeaponAttackModifier : ModifyWeaponAttackModeBase
    {
        private const int LightMomentumModifier = -2;
        private const int MomentumModifier = -3;
        private const int HeavyMomentumModifier = -4;

        public WarDanceFlurryWeaponAttackModifier() : base(ValidatorsWeapon.IsMelee)
        {
        }

        protected override void TryModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            var momentum = GetMomentumStacks(character);

            if (momentum == 0)
            {
                return;
            }

            var item = attackMode.sourceDefinition as ItemDefinition;
            var isLight = ValidatorsWeapon.HasAnyWeaponTag(item, TagsDefinitions.WeaponTagLight);
            var isHeavy = ValidatorsWeapon.HasAnyWeaponTag(item, TagsDefinitions.WeaponTagHeavy);
            var toHit = 0;

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

            var trendInfo = new TrendInfo(toHit, FeatureSourceType.Condition, WarDanceMomentum.Name, character);

            attackMode.ToHitBonusTrends.Add(trendInfo);
        }
    }

    private sealed class StopMomentumAndAttacksWhenRemoved : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            target.RemoveAllConditionsOfType(WarDanceMomentum.Name, WarDanceExtraAttack.Name);
        }
    }

    private sealed class FocusedWarDance : IModifyConcentrationAttribute
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
