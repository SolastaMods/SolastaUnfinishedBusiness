using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class CollegeOfWarDancer : AbstractSubclass
{
    private const string Name = "CollegeOfWarDancer";

    private static readonly FeatureDefinition ImprovedWarDance = FeatureDefinitionBuilder
        .Create($"Feature{Name}ImprovedWarDance")
        .SetGuiPresentation(Category.Feature)
        .AddToDB();

    private static readonly ConditionDefinition ConditionWarDance = ConditionDefinitionBuilder
        .Create("ConditionWarDance")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionRaging)
        .AddFeatures(
            FeatureDefinitionMovementAffinityBuilder
                .Create("MovementAffinityConditionWarDanceExtraMovement3")
                .SetGuiPresentation("ConditionWarDance", Category.Condition, Gui.NoLocalization)
                .SetBaseSpeedAdditiveModifier(3)
                .SetImmunities(difficultTerrainImmunity: true)
                .AddToDB(),
            FeatureDefinitionAttackModifierBuilder
                .Create("AttackModifierWarDance")
                .SetGuiPresentation("ConditionWarDance", Category.Condition, Gui.NoLocalization)
                .SetAttackRollModifier(0, AttackModifierMethod.AddAbilityScoreBonus, AttributeDefinitions.Charisma)
                .AddToDB())
        .AddCustomSubFeatures(
            AllowFreeWeaponSwitching.Mark,
            new StopMomentumAndAttacksWhenRemoved(),
            new WarDanceFlurryPhysicalAttack(),
            new WarDanceFlurryWeaponAttackModifier(),
            new WarDanceExtraAttacks())
        .AddToDB();

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
                .SetNotificationTag("Momentum")
                .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
                .SetDamageDice(DieType.D6, 2)
                .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
                .SetAttackModeOnly()
                .SetIgnoreCriticalDoubleDice(true)
                .AddCustomSubFeatures(new ModifyAdditionalDamageFormMomentum())
                .AddToDB())
        .AddToDB();

    private static readonly ConditionDefinition WarDanceExtraAttack = ConditionDefinitionBuilder
        .Create("ConditionWarDanceExtraAttack")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .AllowMultipleInstances()
        .SetFeatures(
            FeatureDefinitionAdditionalActionBuilder
                .Create("AdditionalActionWarDanceMomentum")
                .SetGuiPresentationNoContent(true)
                .AddCustomSubFeatures(AllowConditionDuplicates.Mark, ValidateAdditionalActionAttack.MeleeOnly)
                .SetActionType(ActionType.Main)
                .SetMaxAttacksNumber(1)
                .SetRestrictedActions(Id.AttackMain)
                .AddToDB())
        .AddToDB();

    private static readonly ConditionDefinition WarDanceMissedAttack = ConditionDefinitionBuilder
        .Create("ConditionWarDanceMissedAttack")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .AddToDB();

    public CollegeOfWarDancer()
    {
        var powerWarDance = FeatureDefinitionPowerBuilder
            .Create("PowerWarDancerWarDance")
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
            .AddCustomSubFeatures(ForceConcentrationCheck.Mark,
                ValidatorsValidatePowerUse.HasNoneOfConditions(ConditionWarDance.Name))
            .AddToDB();

        var focusedWarDance = FeatureDefinitionBuilder
            .Create($"Feature{Name}FocusedWarDance")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new FocusedWarDance())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.CollegeOfWarDancer, 256))
            .AddFeaturesAtLevel(3,
                powerWarDance, FeatureSetCasterFightingProficiency, MagicAffinityCasterFightingCombatMagic)
            .AddFeaturesAtLevel(6, ImprovedWarDance)
            .AddFeaturesAtLevel(14, focusedWarDance)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Bard;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBardColleges;

    internal override DeityDefinition DeityDefinition => null;

    private static int GetMomentumStacks(RulesetActor character)
    {
        return character?.ConditionsByCategory
            .SelectMany(x => x.Value)
            .Count(x => x.conditionDefinition == WarDanceMomentum) ?? 0;
    }

    private static void GrantWarDanceCondition(RulesetCharacter rulesetCharacter, BaseDefinition condition)
    {
        rulesetCharacter.InflictCondition(
            condition.Name,
            DurationType.Round,
            0,
            TurnOccurenceType.EndOfTurn,
            AttributeDefinitions.TagEffect,
            rulesetCharacter.guid,
            rulesetCharacter.CurrentFaction.Name,
            1,
            condition.Name,
            0,
            0,
            0);
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

    private sealed class WarDanceExtraAttacks : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            var actingCharacter = action.ActingCharacter;
            var actionParams = action.ActionParams;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var actionDefinition = actionParams.ActionDefinition;

            //Wrong scope or action type, skip
            if (Gui.Battle == null ||
                actionDefinition.ActionType != ActionType.Main)
            {
                yield break;
            }

            //Character isn't War Dancing or doesn't have Improved War Dance, skip
            if (!rulesetCharacter.HasConditionOfType(ConditionWarDance) ||
                !rulesetCharacter.HasAnyFeature(ImprovedWarDance))
            {
                yield break;
            }

            //Still has attacks, skip
            if (action is CharacterActionAttack &&
                actingCharacter.GetActionAvailableIterations(Id.AttackMain) > 1)
            {
                yield break;
            }

            //Wrong action or weapon or a miss after momentum started, skip
            var extraAttacks = GetExtraAttacks(rulesetCharacter);
            var wrongAction = actionDefinition.Id is not Id.AttackMain;
            var wrongWeapon = !ValidatorsWeapon.IsMelee(actionParams.attackMode);
            var missed = rulesetCharacter.RemoveAllConditionsOfType(WarDanceMissedAttack.Name);

            if (extraAttacks > 0 && (wrongAction || wrongWeapon || missed))
            {
                yield break;
            }

            //Too many extra attacks, skip
            var maxAttacks = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus) / 2;

            if (extraAttacks >= maxAttacks)
            {
                yield break;
            }

            GrantWarDanceCondition(rulesetCharacter, WarDanceExtraAttack);
            rulesetCharacter.LogCharacterUsedFeature(ImprovedWarDance, "Feedback/&ActivateWarDancerAttack", true);
        }

        private static int GetExtraAttacks(RulesetActor character)
        {
            return character?.ConditionsByCategory
                .SelectMany(x => x.Value)
                .Count(x => x.conditionDefinition == WarDanceExtraAttack) ?? 0;
        }
    }

    private sealed class WarDanceFlurryPhysicalAttack : IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetCharacter = attacker.RulesetCharacter;

            if (action.ActionType == ActionType.Reaction ||
                rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false } ||
                !rulesetCharacter.HasConditionOfType(ConditionWarDance) ||
                !ValidatorsWeapon.IsMelee(attackMode))
            {
                yield break;
            }

            if (rollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                GrantWarDanceCondition(rulesetCharacter, WarDanceMissedAttack);

                yield break;
            }

            GrantWarDanceCondition(rulesetCharacter, WarDanceMomentum);
            rulesetCharacter.ShowLabel(WarDanceMomentum.GuiPresentation.Title, Gui.ColorPositive);
            rulesetCharacter.LogCharacterUsedFeature(WarDanceMomentum, "Feedback/&ActivateWarDancerMomentum", true);
        }
    }

    private sealed class WarDanceFlurryWeaponAttackModifier() : ModifyWeaponAttackModeBase(ValidatorsWeapon.IsMelee)
    {
        private const int LightMomentumModifier = -2;
        private const int MomentumModifier = -3;
        private const int HeavyMomentumModifier = -4;

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

            attackMode.ToHitBonus += toHit;
            attackMode.ToHitBonusTrends.Add(
                new TrendInfo(toHit, FeatureSourceType.Condition, WarDanceMomentum.Name, character));
        }
    }

    private sealed class ModifyAdditionalDamageFormMomentum : IModifyAdditionalDamageForm
    {
        public DamageForm AdditionalDamageForm(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
            DamageForm damageForm)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            damageForm.DieType = GetMomentumDice(rulesetAttacker);
            damageForm.DiceNumber = GetMomentumDiceNumber(rulesetAttacker);

            return damageForm;
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

        private static int GetMomentumDiceNumber(RulesetCharacter character)
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
