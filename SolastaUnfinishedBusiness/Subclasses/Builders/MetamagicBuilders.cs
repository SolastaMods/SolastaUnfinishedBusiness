using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Validators;
using static MetricsDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses.Builders;

internal static class MetamagicBuilders
{
    private const string MetamagicAltruistic = "MetamagicAltruisticSpell";
    private const string MetamagicFocused = "MetamagicFocusedSpell";
    private const string MetamagicPowerful = "MetamagicPowerfulSpell";
    private const string MetamagicSeeking = "MetamagicSeekingSpell";
    private const string MetamagicTransmuted = "MetamagicTransmutedSpell";
    private const string MetamagicWidened = "MetamagicWidenedSpell";

    #region Metamagic Altruistic

    internal static MetamagicOptionDefinition BuildMetamagicAltruisticSpell()
    {
        var validator = new ValidateMetamagicApplication(IsMetamagicAltruisticSpellValid);

        var altruisticAlly = MetamagicOptionDefinitionBuilder
            .Create($"{MetamagicAltruistic}Ally")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetCost()
            .AddCustomSubFeatures(new MetamagicAltruisticAlly(), validator)
            .AddToDB();

        var altruisticSelf = MetamagicOptionDefinitionBuilder
            .Create($"{MetamagicAltruistic}Self")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetCost(sorceryPointsCost: 3)
            .AddCustomSubFeatures(new MetamagicAltruisticSelf(), validator)
            .AddToDB();

        return MetamagicOptionDefinitionBuilder
            .Create(MetamagicAltruistic)
            .SetGuiPresentation(Category.Feature)
            .SetCost(MetamagicCostMethod.SpellLevel)
            .AddCustomSubFeatures(new ReplaceMetamagicOption(altruisticAlly, altruisticSelf))
            .AddToDB();
    }

    private static void IsMetamagicAltruisticSpellValid(
        RulesetCharacter caster,
        RulesetEffectSpell rulesetEffectSpell,
        MetamagicOptionDefinition metamagicOption,
        ref bool result,
        ref string failure)
    {
        var effect = rulesetEffectSpell.EffectDescription;

        if (effect.rangeType == RangeType.Self && effect.targetType == TargetType.Self)
        {
            return;
        }

        failure = "Failure/&FailureFlagSpellRangeMustBeSelf";

        result = false;
    }

    private sealed class MetamagicAltruisticAlly : IModifyEffectDescription
    {
        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return true;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            effectDescription.targetSide = Side.Ally;
            effectDescription.rangeType = RangeType.Distance;
            effectDescription.rangeParameter = 6;
            effectDescription.targetType = TargetType.IndividualsUnique;
            effectDescription.targetParameter = 1;
            effectDescription.targetExcludeCaster = true;

            return effectDescription;
        }
    }

    private sealed class MetamagicAltruisticSelf : IModifyEffectDescription
    {
        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return true;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            effectDescription.targetSide = Side.Ally;
            effectDescription.rangeType = RangeType.Distance;
            effectDescription.rangeParameter = 6;
            effectDescription.targetType = TargetType.IndividualsUnique;
            effectDescription.targetParameter = 1;
            effectDescription.inviteOptionalAlly = true;

            return effectDescription;
        }
    }

    #endregion

    #region Metamagic Focused

    internal static MetamagicOptionDefinition BuildMetamagicFocusedSpell()
    {
        var validator = new ValidateMetamagicApplication(IsMetamagicFocusedSpellValid);

        var magicAffinity = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagiAffinity{MetamagicFocused}")
            .SetGuiPresentation(MetamagicFocused, Category.Feature)
            .SetConcentrationModifiers(ConcentrationAffinity.Advantage)
            .AddToDB();

        var condition = ConditionDefinitionBuilder
            .Create($"Condition{MetamagicFocused}")
            .SetGuiPresentation(MetamagicFocused, Category.Feature,
                DatabaseHelper.ConditionDefinitions.ConditionBearsEndurance)
            .SetPossessive()
            .AddFeatures(magicAffinity)
            .AddToDB();

        var metamagic = MetamagicOptionDefinitionBuilder
            .Create(MetamagicFocused)
            .SetGuiPresentation(Category.Feature)
            .SetCost()
            .AddToDB();

        metamagic.AddCustomSubFeatures(new ModifyEffectDescriptionMetamagicFocused(metamagic, condition), validator);

        return metamagic;
    }

    private static void IsMetamagicFocusedSpellValid(
        RulesetCharacter caster,
        RulesetEffectSpell rulesetEffectSpell,
        MetamagicOptionDefinition metamagicOption,
        ref bool result,
        ref string failure)
    {
        var spell = rulesetEffectSpell.SpellDefinition;

        if (spell.RequiresConcentration)
        {
            return;
        }

        failure = "Failure/&FailureFlagSpellMustRequireConcentration";

        result = false;
    }

    private sealed class ModifyEffectDescriptionMetamagicFocused(
        MetamagicOptionDefinition metamagicFocused,
        ConditionDefinition conditionFocused)
        : IMagicEffectInitiatedByMe
    {
        public IEnumerator OnMagicEffectInitiatedByMe(
            CharacterActionMagicEffect action,
            RulesetEffect rulesetEffect,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            var rulesetCharacter = attacker.RulesetCharacter;

            if (rulesetEffect.MetamagicOption != metamagicFocused)
            {
                yield break;
            }

            rulesetCharacter.InflictCondition(
                conditionFocused.Name,
                rulesetEffect.EffectDescription.DurationType,
                rulesetEffect.EffectDescription.DurationParameter,
                rulesetEffect.EffectDescription.EndOfEffect,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionFocused.Name,
                0,
                0,
                0);
        }
    }

    #endregion

    #region Metamagic Powerful

    internal static MetamagicOptionDefinition BuildMetamagicPowerfulSpell()
    {
        var validator = new ValidateMetamagicApplication(IsMetamagicPowerfulSpellValid);

        var metamagic = MetamagicOptionDefinitionBuilder
            .Create(MetamagicPowerful)
            .SetGuiPresentation(Category.Feature)
            .SetCost()
            .AddToDB();

        metamagic.AddCustomSubFeatures(new ModifyEffectDescriptionMetamagicPowerful(metamagic), validator);

        return metamagic;
    }

    private static void IsMetamagicPowerfulSpellValid(
        RulesetCharacter caster,
        RulesetEffectSpell rulesetEffectSpell,
        MetamagicOptionDefinition metamagicOption,
        ref bool result,
        ref string failure)
    {
        var effect = rulesetEffectSpell.EffectDescription;

        if (effect.EffectForms.Any(x => x.FormType == EffectForm.EffectFormType.Damage))
        {
            return;
        }

        failure = "Failure/&FailureFlagSpellMustHaveDamageForm";

        result = false;
    }

    private sealed class ModifyEffectDescriptionMetamagicPowerful(
        MetamagicOptionDefinition metamagicOptionDefinition) : IMagicEffectBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (rulesetEffect.MetamagicOption != metamagicOptionDefinition)
            {
                yield break;
            }

            foreach (var effectForm in actualEffectForms
                         .Where(x => x.FormType == EffectForm.EffectFormType.Damage))
            {
                effectForm.DamageForm.diceNumber += 1;
            }
        }
    }

    #endregion

    #region Metamagic Transmuted

    private static readonly List<string> TransmutedDamageTypes =
        [DamageTypeAcid, DamageTypeCold, DamageTypeFire, DamageTypeLightning, DamageTypePoison, DamageTypeThunder];

    internal static MetamagicOptionDefinition BuildMetamagicTransmutedSpell()
    {
        var validator = new ValidateMetamagicApplication(IsMetamagicTransmutedSpellValid);

        var powerPool = FeatureDefinitionPowerBuilder
            .Create($"Power{MetamagicTransmuted}")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .AddToDB();

        var powers = (from damageType in TransmutedDamageTypes
                let title = Gui.Localize($"Tooltip/&Tag{damageType}Title")
                let description = Gui.Format($"Feedback/&{MetamagicTransmuted}Description", title)
                select FeatureDefinitionPowerSharedPoolBuilder
                    .Create($"Power{MetamagicTransmuted}{damageType}")
                    .SetGuiPresentation(title, description)
                    .SetSharedPool(ActivationTime.NoCost, powerPool)
                    .SetEffectDescription(
                        EffectDescriptionBuilder
                            .Create()
                            .Build())
                    .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
                    .AddToDB())
            .ToList();

        PowerBundle.RegisterPowerBundle(powerPool, false, powers);

        var condition = ConditionDefinitionBuilder
            .Create($"Condition{MetamagicTransmuted}")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(powers)
            .AddFeatures(powerPool)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var metamagic = MetamagicOptionDefinitionBuilder
            .Create(MetamagicTransmuted)
            .SetGuiPresentation(Category.Feature)
            .SetCost()
            .AddToDB();

        metamagic.AddCustomSubFeatures(
            new MagicEffectBeforeHitConfirmedOnEnemyTransmuted(metamagic, condition, powerPool), validator);

        return metamagic;
    }

    private static void IsMetamagicTransmutedSpellValid(
        RulesetCharacter caster,
        RulesetEffectSpell rulesetEffect,
        MetamagicOptionDefinition metamagicOption,
        ref bool result,
        ref string failure)
    {
        if (rulesetEffect.EffectDescription.EffectForms.Any(x =>
                x.FormType == EffectForm.EffectFormType.Damage &&
                TransmutedDamageTypes.Contains(x.DamageForm.DamageType)) ||
            rulesetEffect.SpellDefinition.Name == "BoomingStep")
        {
            return;
        }

        failure = "Failure/&FailureTransmutedSpell";
        result = false;
    }

    private sealed class MagicEffectBeforeHitConfirmedOnEnemyTransmuted(
        MetamagicOptionDefinition metamagicOptionDefinition,
        ConditionDefinition condition,
        FeatureDefinitionPower powerPool) : IMagicEffectBeforeHitConfirmedOnEnemy
    {
        private string _newDamageType;

        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetEffect.MetamagicOption != metamagicOptionDefinition &&
                rulesetAttacker.SpellsCastByMe
                    .FirstOrDefault(x => x.SystemName == "BoomingStep")?.MetamagicOption != metamagicOptionDefinition)
            {
                yield break;
            }

            if (!rulesetAttacker.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, condition.Name))
            {
                rulesetAttacker.InflictCondition(
                    condition.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.StartOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    condition.Name,
                    0,
                    0,
                    0);

                var actionManager =
                    ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

                if (!actionManager)
                {
                    yield break;
                }

                var implementationManager =
                    ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

                var usablePower = PowerProvider.Get(powerPool, rulesetAttacker);
                var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower)
                {
                    StringParameter = MetamagicTransmuted,
                    RulesetEffect = implementationManager
                        .MyInstantiateEffectPower(rulesetAttacker, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { defender }
                };
                var count = actionManager.PendingReactionRequestGroups.Count;
                var reactionRequest = new ReactionRequestSpendBundlePower(actionParams);

                actionManager.AddInterruptRequest(reactionRequest);

                yield return battleManager.WaitForReactions(attacker, actionManager, count);

                if (!actionParams.ReactionValidated)
                {
                    rulesetAttacker.SpendSorceryPoints(-1);

                    yield break;
                }

                var option = reactionRequest.SelectedSubOption;

                _newDamageType = TransmutedDamageTypes[option];
            }

            foreach (var effectForm in actualEffectForms
                         .Where(x =>
                             x.FormType == EffectForm.EffectFormType.Damage &&
                             TransmutedDamageTypes.Contains(x.DamageForm.DamageType)))
            {
                effectForm.DamageForm.damageType = _newDamageType;
            }
        }
    }

    #endregion

    #region Metamagic Seeking

    internal static MetamagicOptionDefinition BuildMetamagicSeekingSpell()
    {
        var validator = new ValidateMetamagicApplication(IsMetamagicSeekingSpellValid);

        return MetamagicOptionDefinitionBuilder
            .Create(MetamagicSeeking)
            .SetGuiPresentation(Category.Feature)
            .SetCost(MetamagicCostMethod.FixedValue, 2)
            .AddCustomSubFeatures(new TryAlterOutcomeAttackMetamagicSeeking(), validator)
            .AddToDB();
    }

    private static void IsMetamagicSeekingSpellValid(
        RulesetCharacter caster,
        RulesetEffectSpell rulesetEffectSpell,
        MetamagicOptionDefinition metamagicOption,
        ref bool result,
        ref string failure)
    {
        failure = "Failure/&FailureSeekingSpell";

        result = false;
    }

    private sealed class TryAlterOutcomeAttackMetamagicSeeking : ITryAlterOutcomeAttack
    {
        public int HandlerPriority => -10;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            if (action is not CharacterActionCastSpell)
            {
                yield break;
            }

            var actionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var rulesetHelper = helper.RulesetCharacter;

            if (!actionManager ||
                action.AttackRollOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure) ||
                helper != attacker ||
                !helper.IsActionOnGoing(ActionDefinitions.Id.MetamagicToggle) ||
                rulesetHelper.RemainingSorceryPoints < 2)
            {
                yield break;
            }

            var reactionParams =
                new CharacterActionParams(helper, (ActionDefinitions.Id)ExtraActionId.DoNothingFree)
                {
                    StringParameter = "CustomReactionMetamagicSeekingSpellDescription".Formatted(
                        Category.Reaction, defender.Name),
                    StringParameter2 = "2"
                };
            var count = actionManager.PendingReactionRequestGroups.Count;

            var reactionRequest = new ReactionRequestCustom("MetamagicSeekingSpell", reactionParams)
            {
                Resource = ReactionResourceSorceryPoints.Instance
            };

            actionManager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(attacker, actionManager, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetHelper.SpendSorceryPoints(2);

            var dieRoll = rulesetHelper.RollDie(DieType.D20, RollContext.None, false, AdvantageType.None, out _, out _);
            var previousRoll = action.AttackRoll;

            action.AttackSuccessDelta += dieRoll - previousRoll;
            action.AttackRoll = dieRoll;

            if (action.AttackSuccessDelta >= 0)
            {
                action.AttackRollOutcome = dieRoll == 20 ? RollOutcome.CriticalSuccess : RollOutcome.Success;
            }
            else
            {
                action.AttackRollOutcome = dieRoll == 1 ? RollOutcome.CriticalFailure : RollOutcome.Failure;
            }

            rulesetHelper.LogCharacterActivatesAbility(
                "Feature/&MetamagicSeekingSpellTitle",
                "Feedback/&MetamagicSeekingSpellToHitRoll",
                extra:
                [
                    (dieRoll > previousRoll ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                        dieRoll.ToString()),
                    (previousRoll > dieRoll ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                        previousRoll.ToString())
                ]);
        }
    }

    #endregion

    #region Metamagic Widened

    internal static MetamagicOptionDefinition BuildMetamagicWidenedSpell()
    {
        var validator = new ValidateMetamagicApplication(IsMetamagicWidenedSpellValid);

        return MetamagicOptionDefinitionBuilder
            .Create(MetamagicWidened)
            .SetGuiPresentation(Category.Feature)
            .SetCost(MetamagicCostMethod.FixedValue, 2)
            .AddCustomSubFeatures(new ModifyEffectDescriptionMetamagicWidened(), validator)
            .AddToDB();
    }

    private static void IsMetamagicWidenedSpellValid(
        RulesetCharacter caster,
        RulesetEffectSpell rulesetEffectSpell,
        MetamagicOptionDefinition metamagicOption,
        ref bool result,
        ref string failure)
    {
        var effect = rulesetEffectSpell.EffectDescription;
        var shapeType = effect.TargetType switch
        {
            TargetType.Line => GeometricShapeType.Line,
            TargetType.Cone => GeometricShapeType.Cone,
            TargetType.Cube or TargetType.CubeWithOffset => GeometricShapeType.Cube,
            TargetType.Cylinder or TargetType.CylinderWithDiameter => GeometricShapeType.Cylinder,
            TargetType.Sphere or TargetType.PerceivingWithinDistance or TargetType.InLineOfSightWithinDistance
                or TargetType.ClosestWithinDistance => GeometricShapeType.Sphere,
            TargetType.WallLine => GeometricShapeType.WallLine,
            TargetType.WallRing => GeometricShapeType.WallRing,
            _ => GeometricShapeType.None
        };

        if (shapeType
            is GeometricShapeType.Cone
            or GeometricShapeType.Cube
            or GeometricShapeType.Cylinder
            or GeometricShapeType.Sphere)
        {
            return;
        }

        failure = "Failure/&FailureFlagSpellMustBeOfTargetArea";

        result = false;
    }

    private sealed class ModifyEffectDescriptionMetamagicWidened : IModifyEffectDescription
    {
        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return true;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            effectDescription.targetParameter += effectDescription.TargetType == TargetType.Cube ? 2 : 1;

            return effectDescription;
        }
    }

    #endregion
}
