using System.Collections;
using System.Collections.Generic;
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
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;
using static SolastaUnfinishedBusiness.Models.SpellsContext;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class CircleOfTheForestGuardian : AbstractSubclass
{
    private const string Name = "ForestGuardian";

    public CircleOfTheForestGuardian()
    {
        var autoPreparedSpellsForestGuardian = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Circle")
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, EnsnaringStrike, Shield),
                BuildSpellGroup(3, LesserRestoration, SpikeGrowth),
                BuildSpellGroup(5, ProtectionFromEnergy, DispelMagic),
                BuildSpellGroup(7, AuraOfVitality, FreedomOfMovement),
                BuildSpellGroup(9, GreaterRestoration, HoldMonster))
            .SetSpellcastingClass(CharacterClassDefinitions.Druid)
            .AddToDB();

        var attackModifierSylvanMagic = FeatureDefinitionAttackModifierBuilder
            .Create($"AttackModifier{Name}SylvanDurability")
            .SetGuiPresentation(Category.Feature)
            .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
            .AddCustomSubFeatures(
                new ValidateContextInsteadOfRestrictedProperty((_, _, character, _, _, mode, _) =>
                    (OperationType.Set, (mode is { ActionType: ActionDefinitions.ActionType.Main } &&
                                         ValidatorsCharacter.HasFreeHandWithoutTwoHandedInMain(character) &&
                                         ValidatorsCharacter.HasMeleeWeaponInMainHand(character)) ||
                                        (mode is { ActionType: ActionDefinitions.ActionType.Bonus } &&
                                         character.GetOriginalHero() is { } hero &&
                                         hero.ActiveFightingStyles.Contains(FightingStyleDefinitions.TwoWeapon) &&
                                         ValidatorsCharacter.HasMeleeWeaponInMainAndOffhand(character)))),
                new CanUseAttribute(AttributeDefinitions.Wisdom, CanWeaponBeEnchanted),
                new AddTagToWeaponWeaponAttack(TagsDefinitions.MagicalWeapon, CanWeaponBeEnchanted))
            .AddToDB();

        var conditionBarkWard = ConditionDefinitionBuilder
            .Create($"Condition{Name}BarkWard")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionMagicallyArmored)
            .SetPossessive()
            .SetCancellingConditions(ConditionDefinitions.ConditionIncapacitated)
            .CopyParticleReferences(PowerRangerSwiftBladeBattleFocus)
            .AddToDB();

        var conditionImprovedBarkWard = ConditionDefinitionBuilder
            .Create($"Condition{Name}ImprovedBarkWard")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionMagicallyArmored)
            .SetPossessive()
            .SetCancellingConditions(ConditionDefinitions.ConditionIncapacitated)
            .SetParentCondition(conditionBarkWard)
            .SetFeatures(
                FeatureDefinitionDamageAffinityBuilder
                    .Create($"DamageAffinity{Name}ImprovedBarkWard")
                    .SetGuiPresentationNoContent(true)
                    .SetDamageAffinityType(DamageAffinityType.Immunity)
                    .SetDamageType(DamageTypePoison)
                    .AddToDB())
            .CopyParticleReferences(PowerRangerSwiftBladeBattleFocus)
            .AddToDB();

        var powerBarkWard = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"PowerSharedPool{Name}BarkWard")
            .SetGuiPresentation(Category.Feature, PowerDruidWildShape)
            .SetSharedPool(ActivationTime.BonusAction, PowerDruidWildShape)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionBarkWard))
                    .SetCasterEffectParameters(SpikeGrowth)
                    .Build())
            .AddToDB();

        var powerImprovedBarkWard = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"PowerSharedPool{Name}ImprovedBarkWard")
            .SetGuiPresentation(Category.Feature, PowerDruidWildShape)
            .SetSharedPool(ActivationTime.BonusAction, PowerDruidWildShape)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionImprovedBarkWard))
                    .SetCasterEffectParameters(SpikeGrowth)
                    .Build())
            .SetOverriddenPower(powerBarkWard)
            .AddToDB();

        // kept name for backward compatibility
        var powerSuperiorBarkWard = FeatureDefinitionPowerBuilder
            .Create($"PowerSharedPool{Name}SuperiorBarkWard")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        // connect them all together

        powerBarkWard.AddCustomSubFeatures(
            new MagicEffectFinishedByMeBarkWard(powerSuperiorBarkWard));

        powerImprovedBarkWard.AddCustomSubFeatures(
            new MagicEffectFinishedByMeBarkWard(powerSuperiorBarkWard),
            new CustomBehaviorBarkWard(powerImprovedBarkWard));

        conditionBarkWard.AddCustomSubFeatures(new CharacterTurnStartListenerBarkWard(powerSuperiorBarkWard));
        conditionImprovedBarkWard.AddCustomSubFeatures(new CharacterTurnStartListenerBarkWard(powerSuperiorBarkWard));

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"CircleOfThe{Name}")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite(Name, Resources.CircleOfTheForestGuardian, 256))
            .AddFeaturesAtLevel(2, autoPreparedSpellsForestGuardian, attackModifierSylvanMagic, powerBarkWard)
            .AddFeaturesAtLevel(6, AttributeModifierCasterFightingExtraAttack, AttackReplaceWithCantripCasterFighting)
            .AddFeaturesAtLevel(10, powerImprovedBarkWard)
            .AddFeaturesAtLevel(14, powerSuperiorBarkWard)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Druid;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceDruidCircle;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static void ApplyTemporaryHitPoints(
        GameLocationCharacter locationCharacter,
        FeatureDefinitionPower powerSuperiorBarkWard)
    {
        var rulesetCharacter = locationCharacter.RulesetCharacter;
        var levels = rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Druid);
        var hitPoints = levels switch
        {
            >= 14 => 10,
            >= 10 => 8,
            >= 6 => 6,
            _ => 4
        };

        if (hitPoints > rulesetCharacter.TemporaryHitPoints)
        {
            rulesetCharacter.ReceiveTemporaryHitPoints(
                hitPoints, DurationType.UntilAnyRest, 0, TurnOccurenceType.StartOfTurn, rulesetCharacter.Guid);
        }

        if (levels < 14)
        {
            return;
        }

        if (Gui.Battle == null)
        {
            return;
        }

        rulesetCharacter.LogCharacterUsedPower(powerSuperiorBarkWard);

        foreach (var ally in Gui.Battle
                     .GetContenders(locationCharacter, isOppositeSide: false, withinRange: 3))
        {
            ally.RulesetCharacter.ReceiveTemporaryHitPoints(
                hitPoints, DurationType.UntilAnyRest, 0, TurnOccurenceType.StartOfTurn, rulesetCharacter.Guid);
        }
    }

    private sealed class MagicEffectFinishedByMeBarkWard(FeatureDefinitionPower powerSuperiorBarkWard)
        : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            ApplyTemporaryHitPoints(action.ActingCharacter, powerSuperiorBarkWard);

            yield break;
        }
    }

    private sealed class CustomBehaviorBarkWard(FeatureDefinitionPower powerBarkOrImprovedBarkWard)
        : IAttackBeforeHitPossibleOnMeOrAlly, IActionFinishedByEnemy
    {
        private bool _shouldTrigger;

        public IEnumerator OnActionFinishedByEnemy(CharacterAction action, GameLocationCharacter target)
        {
            var actingCharacter = action.ActingCharacter;
            var rulesetAttacker = actingCharacter.RulesetCharacter;

            if (_shouldTrigger &&
                rulesetAttacker is { IsDeadOrDyingOrUnconscious: false } &&
                target.IsWithinRange(actingCharacter, 1))
            {
                InflictDamage(actingCharacter, target);
            }

            _shouldTrigger = false;

            yield break;
        }

        public IEnumerator OnAttackBeforeHitPossibleOnMeOrAlly(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect,
            int attackRoll)
        {
            _shouldTrigger =
                defender == helper &&
                defender.RulesetCharacter.TemporaryHitPoints > 0 &&
                defender.RulesetCharacter.HasConditionOfTypeOrSubType($"Condition{Name}BarkWard") &&
                ValidatorsWeapon.IsMelee(attackMode);

            yield break;
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private void InflictDamage(GameLocationCharacter attacker, GameLocationCharacter me)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetMe = me.RulesetCharacter;
            var rolls = new List<int>();
            var damageForm = new DamageForm
            {
                DamageType = DamageTypePiercing, DieType = DieType.D8, DiceNumber = 2, BonusDamage = 0
            };
            var damageRoll =
                rulesetMe.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);
            var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams
            {
                sourceCharacter = rulesetMe, targetCharacter = rulesetAttacker, position = attacker.LocationPosition
            };

            rulesetMe.LogCharacterUsedPower(powerBarkOrImprovedBarkWard);
            EffectHelpers.StartVisualEffect(me, me, PowerPatronTreeExplosiveGrowth);
            RulesetActor.InflictDamage(
                damageRoll,
                damageForm,
                damageForm.DamageType,
                applyFormsParams,
                rulesetAttacker,
                false,
                rulesetMe.Guid,
                false,
                [],
                new RollInfo(damageForm.DieType, rolls, 0),
                true,
                out _);
        }
    }

    private sealed class CharacterTurnStartListenerBarkWard(FeatureDefinitionPower powerSuperiorBarkWard)
        : ICharacterTurnStartListener
    {
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            ApplyTemporaryHitPoints(locationCharacter, powerSuperiorBarkWard);
        }
    }
}
