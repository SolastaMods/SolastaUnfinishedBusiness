using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public class PatronArchfey : AbstractSubclass
{
    private const string Name = "Archfey";

    public PatronArchfey()
    {
        // LEVEL 01

        // Expanded Spells

        var spellListArchfey = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListWizard, $"SpellList{Name}")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(1, FaerieFire, Sleep)
            .SetSpellsAtLevel(2, CalmEmotions, SpellsContext.MirrorImage)
            .SetSpellsAtLevel(3, DispelMagic, Fly)
            .SetSpellsAtLevel(4, DominateBeast, GreaterInvisibility)
            .SetSpellsAtLevel(5, DominatePerson, HoldMonster)
            .FinalizeSpells(true, 9)
            .AddToDB();

        var magicAffinityExpandedSpells = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}ExpandedSpells")
            .SetGuiPresentation("MagicAffinityPatronExpandedSpells", Category.Feature)
            .SetExtendedSpellList(spellListArchfey)
            .AddToDB();

        // Fey Presence

        var powerFeyPresence = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}FeyPresence")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .Build())
            .AddToDB();

        var powerFeyPresenceCharmed = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}FeyPresenceCharmed")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.Action, powerFeyPresence)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionCharmed,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var powerFeyPresenceFrightened = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}FeyPresenceFrightened")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.Action, powerFeyPresence)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionFrightened,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        PowerBundle.RegisterPowerBundle(powerFeyPresence, false,
            powerFeyPresenceCharmed, powerFeyPresenceFrightened);

        var featureSetFeyPresence = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}FeyPresence")
            .SetGuiPresentation($"Power{Name}FeyPresence", Category.Feature)
            .SetFeatureSet(powerFeyPresence, powerFeyPresenceCharmed, powerFeyPresenceFrightened)
            .AddToDB();

        // LEVEL 06

        // Misty Escape

        var conditionMistyEscape = ConditionDefinitionBuilder
            .Create($"Condition{Name}MistyEscape")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var powerMistyEscape = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MistyEscape")
            .SetGuiPresentation(Category.Feature, MistyStep)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                            .Build(),
                        EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionInvisible,
                            ConditionForm.ConditionOperation.Add, true, true))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerMelekTeleport)
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        conditionMistyEscape.AddCustomSubFeatures(
            new ActionFinishedByMeMistyEscape(powerMistyEscape, conditionMistyEscape));

        powerMistyEscape.AddCustomSubFeatures(new CustomBehaviorMistyEscape(conditionMistyEscape));

        // LEVEL 10

        // Beguiling Defenses

        var conditionBeguilingDefenses = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionCharmed, $"Condition{Name}BeguilingDefenses")
            .SetParentCondition(ConditionDefinitions.ConditionCharmed)
            .SetFeatures()
            .SetSpecialInterruptions(ConditionInterruption.Damaged)
            .AddToDB();

        var powerBeguilingDefenses = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BeguilingDefenses")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionBeguilingDefenses, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        powerBeguilingDefenses.AddCustomSubFeatures(new CustomBehaviorBeguilingDefenses(powerBeguilingDefenses));

        var conditionAffinityBeguilingDefenses = FeatureDefinitionConditionAffinityBuilder
            .Create($"ConditionAffinity{Name}BeguilingDefenses")
            .SetGuiPresentationNoContent(true)
            .SetConditionType(ConditionDefinitions.ConditionCharmed)
            .SetConditionAffinityType(ConditionAffinityType.Immunity)
            .AddToDB();

        var featureSetBeguilingDefenses = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}BeguilingDefenses")
            .SetGuiPresentation($"Power{Name}BeguilingDefenses", Category.Feature)
            .SetFeatureSet(powerBeguilingDefenses, conditionAffinityBeguilingDefenses)
            .AddToDB();

        // LEVEL 14

        // Dark Delirium

        var powerDarkDelirium = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DarkDelirium")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .Build())
            .AddToDB();

        var conditionDarkDeliriumCharmed = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionCharmed, $"Condition{Name}DarkDeliriumCharmed")
            .SetParentCondition(ConditionDefinitions.ConditionCharmed)
            .SetFeatures()
            .SetSpecialInterruptions(ConditionInterruption.Damaged)
            .AddToDB();

        var powerDarkDeliriumCharmed = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}DarkDeliriumCharmed")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.Action, powerDarkDelirium)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionDarkDeliriumCharmed, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var conditionDarkDeliriumFrightened = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFrightened, $"Condition{Name}DarkDeliriumFrightened")
            .SetParentCondition(ConditionDefinitions.ConditionFrightened)
            .SetFeatures()
            .SetSpecialInterruptions(ConditionInterruption.Damaged)
            .AddToDB();

        var powerDarkDeliriumFrightened = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}DarkDeliriumFrightened")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.Action, powerDarkDelirium)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionDarkDeliriumFrightened, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        PowerBundle.RegisterPowerBundle(powerDarkDelirium, false,
            powerDarkDeliriumCharmed, powerDarkDeliriumFrightened);

        var featureSetDarkDelirium = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}DarkDelirium")
            .SetGuiPresentation($"Power{Name}DarkDelirium", Category.Feature)
            .SetFeatureSet(powerDarkDelirium, powerDarkDeliriumCharmed, powerDarkDeliriumFrightened)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"Patron{Name}")
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.RangerShadowTamer)
            .AddFeaturesAtLevel(1, magicAffinityExpandedSpells, featureSetFeyPresence)
            .AddFeaturesAtLevel(6, powerMistyEscape)
            .AddFeaturesAtLevel(10, featureSetBeguilingDefenses)
            .AddFeaturesAtLevel(14, featureSetDarkDelirium)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Warlock;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class CustomBehaviorMistyEscape(ConditionDefinition conditionMistyEscape)
        : IMagicEffectBeforeHitConfirmedOnMe, IPhysicalAttackBeforeHitConfirmedOnMe
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return HandleReaction(battleManager, attacker, defender);
        }

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return HandleReaction(battleManager, attacker, defender);
        }

        private IEnumerator HandleReaction(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender)
        {
            var actionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (!actionManager)
            {
                yield break;
            }

            var reactionParams = new CharacterActionParams(defender, (ActionDefinitions.Id)ExtraActionId.DoNothingFree);
            var reactionRequest = new ReactionRequestCustom("MistyEscape", reactionParams);
            var count = actionManager.PendingReactionRequestGroups.Count;

            actionManager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(attacker, actionManager, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            rulesetAttacker.InflictCondition(
                conditionMistyEscape.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionMistyEscape.Name,
                0,
                0,
                0);
        }
    }

    private sealed class ActionFinishedByMeMistyEscape(
        FeatureDefinitionPower powerMistyEscape,
        ConditionDefinition conditionMistyEscape) : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            var attacker = action.ActingCharacter;
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionMistyEscape.Name, out var activeCondition))
            {
                yield break;
            }

            var rulesetDefender = EffectHelpers.GetCharacterByEffectGuid(activeCondition.SourceGuid);
            var defender = GameLocationCharacter.GetFromActor(rulesetDefender);

            rulesetAttacker.RemoveCondition(activeCondition);

            var cursorService = ServiceRepository.GetService<ICursorService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerMistyEscape, rulesetDefender);
            var actionParams = new CharacterActionParams(defender, ActionDefinitions.Id.PowerNoCost)
            {
                ActionModifiers = { new ActionModifier() },
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(defender.RulesetCharacter, usablePower, false),
                UsablePower = usablePower,
                TargetCharacters = { defender }
            };

            cursorService.ActivateCursor<CursorLocationSelectPosition>(actionParams);

            while (cursorService.CurrentCursor is not (CursorLocationBattleFriendlyTurn or CursorLocationBattleEnemyTurn
                   ))
            {
                yield return null;
            }

            ServiceRepository.GetService<IGameLocationActionService>()?
                .ExecuteAction(actionParams, null, true);
        }
    }

    private class CustomBehaviorBeguilingDefenses(FeatureDefinitionPower powerBeguilingDefenses)
        : IPhysicalAttackBeforeHitConfirmedOnMe, IMagicEffectBeforeHitConfirmedOnMe

    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return HandleReaction(battleManager, attacker, defender, actualEffectForms);
        }

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return HandleReaction(battleManager, attacker, defender, actualEffectForms);
        }

        private IEnumerator HandleReaction(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            List<EffectForm> actualEffectForms)
        {
            if (!defender.CanReact() ||
                !defender.CanPerceiveTarget(attacker) ||
                !actualEffectForms.Any(x =>
                    x.FormType == EffectForm.EffectFormType.Condition &&
                    x.ConditionForm.ConditionDefinition == ConditionDefinitions.ConditionCharmed))
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var rulesetDefender = defender.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerBeguilingDefenses, rulesetDefender);
            var actionParams =
                new CharacterActionParams(defender, ActionDefinitions.Id.PowerReaction)
                {
                    StringParameter = "BeguilingDefenses",
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManager
                        .MyInstantiateEffectPower(rulesetDefender, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { attacker }
                };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToUsePower(actionParams, "UsePower", defender);

            yield return battleManager.WaitForReactions(attacker, actionService, count);
        }
    }
}
