using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WizardWarMagic : AbstractSubclass
{
    private const string Name = "WarMagic";

    public WizardWarMagic()
    {
        // LEVEL 02

        // Arcane Deflection

        var conditionArcaneDeflection = ConditionDefinitionBuilder
            .Create($"Condition{Name}ArcaneDeflection")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionRestrictedInsideMagicCircle)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .AddCustomSubFeatures(new CharacterTurnStartListenerArcaneDeflection())
            .AddToDB();

        var featureArcaneDeflection = FeatureDefinitionBuilder
            .Create($"Feature{Name}ArcaneDeflection")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // Tactical Wit

        var attributeModifierTacticalWit = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}TacticalWit")
            .SetGuiPresentation(Category.Feature)
            .SetModifierAbilityScore(AttributeDefinitions.Initiative, AttributeDefinitions.Intelligence)
            .AddToDB();

        // LEVEL 06

        // Power Surge

        var conditionSurgeMark = ConditionDefinitionBuilder
            .Create($"Condition{Name}SurgeMark")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var powerSurge = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Surge")
            .SetGuiPresentation(Category.Feature)
            .SetUsesAbilityBonus(ActivationTime.NoCost, RechargeRate.LongRest, AttributeDefinitions.Intelligence)
            .DelegatedToAction()
            .AddToDB();

        _ = ActionDefinitionBuilder
            .Create(DatabaseHelper.ActionDefinitions.MetamagicToggle, "PowerSurgeToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.PowerSurgeToggle)
            .SetActivatedPower(powerSurge)
            .AddToDB();

        var actionAffinityPowerSurgeToggle = FeatureDefinitionActionAffinityBuilder
            .Create(FeatureDefinitionActionAffinitys.ActionAffinitySorcererMetamagicToggle,
                "ActionAffinityPowerSurgeToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.PowerSurgeToggle)
            .AddCustomSubFeatures(
                new CustomBehaviorPowerSurge(powerSurge, conditionSurgeMark),
                new ValidateDefinitionApplication(
                    ValidatorsCharacter.HasAvailablePowerUsage(powerSurge)))
            .AddToDB();

        var featureSetPowerSurge = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}PowerSurge")
            .SetGuiPresentation($"Power{Name}Surge", Category.Feature)
            .SetFeatureSet(powerSurge, actionAffinityPowerSurgeToggle)
            .AddToDB();

        // LEVEL 10

        // Durable Magic

        var featureDurableMagic = FeatureDefinitionAttributeModifierBuilder
            .Create($"Feature{Name}DurableMagic")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2)
            .SetSituationalContext(ExtraSituationalContext.IsConcentratingOnSpell)
            .AddToDB();

        featureDurableMagic.AddCustomSubFeatures(new CustomBehaviorDurableMagic(featureDurableMagic));

        // LEVEL 14

        // Deflecting Shroud

        var powerDeflectionShroud = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DeflectionShroud")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique, 3)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeForce))
                    .SetCasterEffectParameters(FeatureDefinitionPowers.PowerSorcererDraconicDragonWingsSprout)
                    .SetImpactEffectParameters(SpellDefinitions.ArcaneSword)
                    .Build())
            .AddToDB();

        powerDeflectionShroud.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new ModifyEffectDescriptionDeflectionShroud(powerDeflectionShroud));
        featureArcaneDeflection.AddCustomSubFeatures(
            new CustomBehaviorArcaneDeflection(
                featureArcaneDeflection, conditionArcaneDeflection, powerDeflectionShroud));

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"Wizard{Name}")
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WizardArcaneFighter, 256))
            .AddFeaturesAtLevel(2, featureArcaneDeflection, attributeModifierTacticalWit)
            .AddFeaturesAtLevel(6, featureSetPowerSurge)
            .AddFeaturesAtLevel(10, featureDurableMagic)
            .AddFeaturesAtLevel(14, powerDeflectionShroud)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Wizard;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class CustomBehaviorArcaneDeflection(
        FeatureDefinition featureArcaneDeflection,
        ConditionDefinition conditionArcaneDeflection,
        FeatureDefinitionPower powerDeflectionShroud) : ITryAlterOutcomeAttack, ITryAlterOutcomeSavingThrow
    {
        public int HandlerPriority => 0;

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
            var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var rulesetCharacter = helper.RulesetCharacter;
            var intelligence = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Intelligence);
            var bonus = Math.Max(AttributeDefinitions.ComputeAbilityScoreModifier(intelligence), 1);

            if (!actionManager ||
                action.AttackRollOutcome != RollOutcome.Success ||
                action.AttackSuccessDelta - bonus >= 0 ||
                helper != defender ||
                !helper.CanReact() ||
                !helper.CanPerceiveTarget(attacker))
            {
                yield break;
            }

            var reactionParams =
                new CharacterActionParams(helper, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = "CustomReactionArcaneDeflectionAttackDescription".Formatted(Category.Reaction)
                };
            var reactionRequest = new ReactionRequestCustom("ArcaneDeflectionAttack", reactionParams);
            var count = actionManager.PendingReactionRequestGroups.Count;

            actionManager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(attacker, actionManager, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            EffectHelpers.StartVisualEffect(
                helper, helper, SpellDefinitions.Shield, EffectHelpers.EffectType.QuickCaster);
            rulesetCharacter.InflictCondition(
                conditionArcaneDeflection.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionArcaneDeflection.Name,
                0,
                0,
                0);

            attackModifier.attackRollModifier -= bonus;
            attackModifier.AttacktoHitTrends.Add(
                new TrendInfo(-bonus, FeatureSourceType.CharacterFeature, featureArcaneDeflection.Name,
                    featureArcaneDeflection));
            action.AttackSuccessDelta -= bonus;
            action.AttackRollOutcome = RollOutcome.Failure;
            helper.RulesetCharacter.LogCharacterUsedFeature(
                featureArcaneDeflection,
                "Feedback/&ArcaneDeflectionAttackRoll",
                extra:
                [
                    (ConsoleStyleDuplet.ParameterType.Positive, bonus.ToString()),
                    (ConsoleStyleDuplet.ParameterType.Negative, "Feedback/&RollAttackFailureTitle")
                ]);

            HandleDeflectionShroud(helper);
        }

        public IEnumerator OnTryAlterOutcomeSavingThrow(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier saveModifier,
            bool hasHitVisual,
            bool hasBorrowedLuck)
        {
            var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var rulesetCharacter = helper.RulesetCharacter;
            var intelligence = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Intelligence);
            var bonus = Math.Max(AttributeDefinitions.ComputeAbilityScoreModifier(intelligence), 1);

            if (!actionManager ||
                !action.RolledSaveThrow ||
                action.SaveOutcome != RollOutcome.Failure ||
                action.SaveOutcomeDelta + bonus < 0 ||
                helper != defender ||
                !helper.CanReact() ||
                !helper.CanPerceiveTarget(attacker))
            {
                yield break;
            }

            var reactionParams =
                new CharacterActionParams(helper, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = "CustomReactionArcaneDeflectionSavingDescription".Formatted(Category.Reaction)
                };
            var reactionRequest = new ReactionRequestCustom("ArcaneDeflectionSaving", reactionParams);
            var count = actionManager.PendingReactionRequestGroups.Count;

            actionManager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(attacker, actionManager, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            EffectHelpers.StartVisualEffect(
                helper, helper, SpellDefinitions.Shield, EffectHelpers.EffectType.QuickCaster);
            rulesetCharacter.InflictCondition(
                conditionArcaneDeflection.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionArcaneDeflection.Name,
                0,
                0,
                0);

            action.SaveOutcomeDelta += bonus;
            action.saveOutcome = RollOutcome.Success;
            helper.RulesetCharacter.LogCharacterUsedFeature(
                featureArcaneDeflection,
                "Feedback/&ArcaneDeflectionSavingRoll",
                extra:
                [
                    (ConsoleStyleDuplet.ParameterType.Positive, bonus.ToString()),
                    (ConsoleStyleDuplet.ParameterType.Positive, "Feedback/&RollCheckSuccessTitle")
                ]);

            HandleDeflectionShroud(helper);
        }

        private void HandleDeflectionShroud(GameLocationCharacter helper)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var classLevel = rulesetHelper.GetClassLevel(CharacterClassDefinitions.Wizard);

            if (classLevel < 14 || Gui.Battle == null)
            {
                return;
            }

            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerDeflectionShroud, rulesetHelper);
            var targets = Gui.Battle
                .GetContenders(helper, withinRange: 12)
                .OrderBy(x => DistanceCalculation.GetDistanceFromCharacters(helper, x))
                .Take(3)
                .ToList();
            var actionModifiers = new List<ActionModifier>();

            for (var i = 0; i < targets.Count; i++)
            {
                actionModifiers.Add(new ActionModifier());
            }

            var actionParams = new CharacterActionParams(helper, ActionDefinitions.Id.PowerNoCost)
            {
                ActionModifiers = actionModifiers,
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
                UsablePower = usablePower,
                targetCharacters = targets
            };

            ServiceRepository.GetService<IGameLocationActionService>()?
                .ExecuteAction(actionParams, null, true);
        }
    }

    private sealed class CharacterTurnStartListenerArcaneDeflection : ICharacterTurnStartListener
    {
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            locationCharacter.UsedBonusSpell = true;
            locationCharacter.UsedMainSpell = true;
        }
    }

    private sealed class CustomBehaviorPowerSurge(
        FeatureDefinitionPower powerSurge,
        ConditionDefinition conditionSurgeMark) : IMagicEffectFinishedByMeAny, IMagicEffectBeforeHitConfirmedOnEnemy
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
            if (rulesetEffect.EffectDescription.TargetType is TargetType.Individuals or TargetType.IndividualsUnique)
            {
                if (firstTarget)
                {
                    defender.RulesetActor.DamageReceived += DamageReceived;
                }
            }
            else
            {
                defender.RulesetActor.DamageReceived += DamageReceived;
            }

            yield break;
        }

        public IEnumerator OnMagicEffectFinishedByMeAny(
            CharacterActionMagicEffect action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            var targetsThatTookDamage = new List<GameLocationCharacter>();

            foreach (var target in targets)
            {
                var rulesetTarget = target.RulesetActor;

                if (!rulesetTarget.TryGetConditionOfCategoryAndType(
                        AttributeDefinitions.TagEffect, conditionSurgeMark.Name, out var activeCondition))
                {
                    continue;
                }

                rulesetTarget.RemoveCondition(activeCondition);
                rulesetTarget.DamageReceived -= DamageReceived;
                targetsThatTookDamage.Add(target);
            }

            if (action is not CharacterActionCastSpell actionCastSpell)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerSurge, rulesetAttacker);

            if (actionCastSpell.ActiveSpell.SpellDefinition == SpellDefinitions.Counterspell)
            {
                usablePower.RepayUse();
            }

            if (targetsThatTookDamage.Count == 0 ||
                !rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.PowerSurgeToggle) ||
                rulesetAttacker.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            if (!attacker.OncePerTurnIsValid(powerSurge.Name))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd(powerSurge.Name, 0);
            rulesetAttacker.UsePower(usablePower);

            var bonusDamage = rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Wizard);

            foreach (var target in targetsThatTookDamage)
            {
                var rulesetTarget = target.RulesetActor;

                if (rulesetTarget is not { IsDeadOrDyingOrUnconscious: false })
                {
                    continue;
                }

                var damageForm = new DamageForm
                {
                    DamageType = DamageTypeForce, DieType = DieType.D1, DiceNumber = 0, BonusDamage = bonusDamage
                };
                var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams
                {
                    sourceCharacter = rulesetAttacker,
                    targetCharacter = rulesetTarget,
                    position = target.LocationPosition
                };

                EffectHelpers.StartVisualEffect(attacker, target, SpellDefinitions.EldritchBlast);
                RulesetActor.InflictDamage(
                    bonusDamage,
                    damageForm,
                    DamageTypeForce,
                    applyFormsParams,
                    rulesetTarget,
                    false,
                    rulesetAttacker.Guid,
                    false,
                    [],
                    new RollInfo(DieType.D1, [], bonusDamage),
                    false,
                    out _);
            }
        }

        private void DamageReceived(RulesetActor target, int damage, string type, ulong guid, RollInfo info)
        {
            var source = EffectHelpers.GetCharacterByGuid(guid);

            if (source != null)
            {
                target.InflictCondition(
                    conditionSurgeMark.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfSourceTurn,
                    AttributeDefinitions.TagEffect,
                    source.guid,
                    source.CurrentFaction.Name,
                    1,
                    conditionSurgeMark.Name,
                    0,
                    0,
                    0);
            }
        }
    }

    private sealed class CustomBehaviorDurableMagic(
        FeatureDefinition featureDurableMagic) : IRollSavingThrowInitiated, IRollSavingCheckInitiated
    {
        public void OnRollSavingCheckInitiated(
            RulesetCharacter defender,
            int saveDC,
            string damageType,
            ref ActionModifier actionModifier,
            ref int modifier)
        {
            if (defender.ConcentratedSpell == null)
            {
                return;
            }

            modifier += 2;
            actionModifier.SavingThrowModifierTrends.Add(
                new TrendInfo(2, FeatureSourceType.CharacterFeature, featureDurableMagic.Name, featureDurableMagic));
        }

        public void OnSavingThrowInitiated(
            RulesetCharacter caster,
            RulesetCharacter defender,
            ref int saveBonus,
            ref string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> modifierTrends,
            List<TrendInfo> advantageTrends,
            ref int rollModifier,
            ref int saveDC,
            ref bool hasHitVisual,
            RollOutcome outcome,
            int outcomeDelta,
            List<EffectForm> effectForms)
        {
            if (defender.ConcentratedSpell == null)
            {
                return;
            }

            rollModifier += 2;
            modifierTrends.Add(
                new TrendInfo(2, FeatureSourceType.CharacterFeature, featureDurableMagic.Name, featureDurableMagic));
        }
    }

    private sealed class ModifyEffectDescriptionDeflectionShroud(
        FeatureDefinitionPower powerDeflectionShroud) : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerDeflectionShroud;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var halfClassLevel = (character.GetClassLevel(CharacterClassDefinitions.Wizard) + 1) / 2;

            effectDescription.EffectForms[0].damageForm.BonusDamage = halfClassLevel;

            return effectDescription;
        }
    }
}
