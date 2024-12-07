using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
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
            .SetSpellsAtLevel(3, ConjureAnimals, ProtectionFromEnergy)
            .SetSpellsAtLevel(4, DominateBeast, GreaterInvisibility)
            .SetSpellsAtLevel(5, DominatePerson, SpellsContext.MantleOfThorns)
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
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerFeyPresence", Resources.PowerFeyPresence, 256, 128))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cone, 3)
                    .ExcludeCaster()
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .Build())
            .AddToDB();

        var powerFeyPresenceCharmed = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}FeyPresenceCharmed")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.Action, powerFeyPresence)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cone, 3)
                    .ExcludeCaster()
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionCharmed,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetCasterEffectParameters(PowerSorcererDraconicElementalResistance)
                    .SetImpactEffectParameters(
                        CharmPerson.EffectDescription.EffectParticleParameters.effectParticleReference)
                    .Build())
            .AddToDB();

        var powerFeyPresenceFrightened = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}FeyPresenceFrightened")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.Action, powerFeyPresence)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cone, 3)
                    .ExcludeCaster()
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionFrightened,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetCasterEffectParameters(PowerSorcererDraconicElementalResistance)
                    .SetImpactEffectParameters(
                        Malediction.EffectDescription.EffectParticleParameters.effectParticleReference)
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
            .SetFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create($"ActionAffinity{Name}MistyEscape")
                    .SetGuiPresentationNoContent(true)
                    .SetForbiddenActions(
                        ActionDefinitions.Id.Shove,
                        ActionDefinitions.Id.ShoveBonus,
                        ActionDefinitions.Id.AttackMain,
                        ActionDefinitions.Id.AttackOff,
                        ActionDefinitions.Id.AttackFree)
                    .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var powerMistyEscape = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MistyEscape")
            .SetGuiPresentation(Category.Feature, MistyStep)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                            .Build(),
                        EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionInvisible,
                            ConditionForm.ConditionOperation.Add, true, true))
                    .SetParticleEffectParameters(PowerMelekTeleport)
                    .Build())
            .AddToDB();

        powerMistyEscape.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new CustomBehaviorMistyEscape(powerMistyEscape, conditionMistyEscape));

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
            .SetGuiPresentation(Category.Feature, hidden: true)
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
                    .SetCasterEffectParameters(PowerSorcererDraconicElementalResistance)
                    .SetImpactEffectParameters(
                        CharmPerson.EffectDescription.EffectParticleParameters.effectParticleReference)
                    .Build())
            .AddToDB();

        powerBeguilingDefenses.AddCustomSubFeatures(
            new CustomBehaviorBeguilingDefenses(powerBeguilingDefenses));

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
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerDarkDelirium", Resources.PowerDarkDelirium, 256, 128))
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
            .SetGuiPresentation(Category.Feature, hidden: true)
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
                    .SetCasterEffectParameters(PowerGreen_Hag_Invisibility)
                    .SetImpactEffectParameters(
                        CharmPerson.EffectDescription.EffectParticleParameters.effectParticleReference)
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
            .SetGuiPresentation(Category.Feature, hidden: true)
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
                    .SetCasterEffectParameters(PowerGreen_Hag_Invisibility)
                    .SetEffectEffectParameters(PhantasmalKiller)
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
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PatronArchfey, 256))
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

    //
    // Misty Escape
    //

    private sealed class CustomBehaviorMistyEscape(
        FeatureDefinitionPower powerMistyEscape,
        ConditionDefinition conditionMistyEscape)
        : IMagicEffectBeforeHitConfirmedOnMe, IPhysicalAttackBeforeHitConfirmedOnMe,
            IMagicEffectFinishedOnMe, IPhysicalAttackFinishedOnMe,
            IIgnoreInvisibilityInterruptionCheck
    {
        private const string TagMistyEscape = "MistyEscape";

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
            if (attacker.IsOppositeSide(defender.Side))
            {
                yield return HandleReaction(battleManager, attacker, defender);
            }
        }

        public IEnumerator OnMagicEffectFinishedOnMe(CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            List<GameLocationCharacter> targets)
        {
            if (!defender.UsedSpecialFeatures.ContainsKey(TagMistyEscape))
            {
                yield break;
            }

            defender.UsedSpecialFeatures.Remove(TagMistyEscape);

            yield return SelectPositionAndExecutePower(defender);
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

        public IEnumerator OnPhysicalAttackFinishedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (!defender.UsedSpecialFeatures.ContainsKey(TagMistyEscape))
            {
                yield break;
            }

            defender.UsedSpecialFeatures.Remove(TagMistyEscape);

            yield return SelectPositionAndExecutePower(defender);
        }

        private IEnumerator SelectPositionAndExecutePower(GameLocationCharacter defender)
        {
            CampaignsContext.ResetCamera();

            var rulesetDefender = defender.RulesetCharacter;
            var cursorManager = ServiceRepository.GetService<ICursorService>() as CursorManager;
            var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();
            var usablePower = PowerProvider.Get(powerMistyEscape, rulesetDefender);
            var rulesetEffect = implementationService
                .InstantiateEffectPower(defender.RulesetCharacter, usablePower, false);

            var actionParams = new CharacterActionParams(defender, (ActionDefinitions.Id)ExtraActionId.DoNothingFree)
            {
                RulesetEffect = rulesetEffect, UsablePower = usablePower
            };

            var cursorLocationSelectPosition =
                cursorManager!.cursorsByType[typeof(CursorLocationSelectPosition)] as CursorLocationSelectPosition;

            cursorLocationSelectPosition!.selectedPositions.Clear();

            // need this outer loop to ensure we re-activate cursor if game cancels it
            while (cursorLocationSelectPosition.selectedPositions.Count == 0)
            {
                cursorManager!.ActivateCursor<CursorLocationSelectPosition>(actionParams);

                while (cursorManager.CurrentCursor is CursorLocationSelectPosition)
                {
                    yield return null;
                }
            }

            actionParams = new CharacterActionParams(defender, ActionDefinitions.Id.PowerNoCost)
            {
                ActionModifiers = { new ActionModifier() },
                RulesetEffect = rulesetEffect,
                UsablePower = usablePower,
                TargetCharacters = { defender },
                positions = [.. cursorLocationSelectPosition!.selectedPositions]
            };

            ServiceRepository.GetService<IGameLocationActionService>().ExecuteAction(actionParams, null, true);
        }

        private IEnumerator HandleReaction(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender)
        {
            if (defender.RulesetCharacter.GetRemainingPowerUses(powerMistyEscape) == 0 ||
                defender.IsMyTurn())
            {
                yield break;
            }

            yield return defender.MyReactToDoNothing(
                ExtraActionId.DoNothingFree,
                attacker,
                TagMistyEscape,
                $"CustomReaction{TagMistyEscape}Description".Localized(Category.Reaction),
                ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated()
            {
                defender.UsedSpecialFeatures.TryAdd(TagMistyEscape, 0);

                // trick to ensure enemy won't execute any more attack or shove action after teleport
                var rulesetAttacker = attacker.RulesetCharacter;
                var rulesetDefender = defender.RulesetCharacter;

                rulesetAttacker.InflictCondition(
                    conditionMistyEscape.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetDefender.guid,
                    rulesetDefender.CurrentFaction.Name,
                    1,
                    conditionMistyEscape.Name,
                    0,
                    0,
                    0);
            }
        }
    }

    //
    // Beguiling Defenses
    //

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
                    x.ConditionForm.ConditionDefinition.IsSubtypeOf(ConditionDefinitions.ConditionCharmed.Name)))
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerBeguilingDefenses, rulesetDefender);

            yield return defender.MyReactToUsePower(
                ActionDefinitions.Id.PowerReaction,
                usablePower,
                [attacker],
                attacker,
                "BeguilingDefenses",
                battleManager: battleManager);
        }
    }
}
