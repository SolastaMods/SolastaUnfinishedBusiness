using System.Collections;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Validators;
using TA;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPointPools;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class Tabletop2024Context
{
    internal static readonly ConditionDefinition ConditionIndomitableSaving = ConditionDefinitionBuilder
        .Create("ConditionIndomitableSaving")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .AddCustomSubFeatures(new RollSavingThrowInitiatedIndomitableSaving())
        .SetSpecialInterruptions(ConditionInterruption.SavingThrow)
        .AddToDB();

    private static readonly FeatureDefinition FeatureFighterTacticalMind = FeatureDefinitionBuilder
        .Create("FeatureFighterTacticalMind")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(new TryAlterOutcomeAttributeCheckTacticalMind())
        .AddToDB();

    private static readonly FeatureDefinition FeatureFighterTacticalShift = FeatureDefinitionBuilder
        .Create("FeatureFighterTacticalShift")
        .SetGuiPresentation(Category.Feature)
        .AddToDB();

    private static readonly ConditionDefinition ConditionStudiedAttacks = ConditionDefinitionBuilder
        .Create("ConditionStudiedAttacks")
        .SetGuiPresentation(Category.Condition, ConditionMarkedByHunter)
        .SetConditionType(ConditionType.Detrimental)
        .AddCustomSubFeatures(new PhysicalAttackFinishedOnMeStudiedAttacks())
        .SetPossessive()
        .AddToDB();

    private static readonly FeatureDefinitionCombatAffinity CombatAffinityStudiedAttacks =
        FeatureDefinitionCombatAffinityBuilder
            .Create("CombatAffinityStudiedAttacks")
            .SetGuiPresentation("Condition/&ConditionStudiedAttacksTitle", Gui.NoLocalization)
            .SetSituationalContext(
                (SituationalContext)ExtraSituationalContext.IsConditionSource, ConditionStudiedAttacks)
            .DisableAutoFormatDescription()
            .SetAttackOnMeAdvantage(AdvantageType.Advantage)
            .AddToDB();

    private static readonly FeatureDefinition FeatureFighterStudiedAttacks = FeatureDefinitionBuilder
        .Create("FeatureFighterStudiedAttacks")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(new PhysicalAttackFinishedByMeStudiedAttacks())
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerFighterTacticalMasterPool = FeatureDefinitionPowerBuilder
        .Create("PowerFighterTacticalMasterPool")
        .SetGuiPresentationNoContent(true)
        .SetShowCasting(false)
        .SetUsesFixed(ActivationTime.NoCost)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .Build())
        .AddToDB();

    private static readonly FeatureDefinitionFeatureSet FeatureSetFighterTacticalMaster =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetFighterTacticalMaster")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(
                PowerFighterTacticalMasterPool,
                FeatureDefinitionActionAffinityBuilder
                    .Create("ActionAffinityFighterTacticalMaster")
                    .SetGuiPresentationNoContent(true)
                    .SetAuthorizedActions((Id)ExtraActionId.TacticalMasterToggle)
                    .AddCustomSubFeatures(new ValidateDefinitionApplication(
                        ShouldDisplayWeaponMasteryToggle,
                        c => !c.IsToggleEnabled((Id)ExtraActionId.WeaponMasteryToggle)))
                    .AddToDB())
            .AddToDB();

    private static void LoadFighterSecondWind()
    {
        PowerFighterSecondWind.AddCustomSubFeatures(
            HasModifiedUses.Marker,
            new ModifyPowerPoolAmount
            {
                PowerPool = PowerFighterSecondWind,
                Type = PowerPoolBonusCalculationType.SecondWind2024,
                Attribute = FighterClass
            });
    }

    private static void LoadFighterStudiedAttacks()
    {
        ConditionStudiedAttacks.Features.SetRange(CombatAffinityStudiedAttacks);
    }

    private static void LoadFighterTacticalProgression()
    {
        var powerFighterSecondWindTargeting = FeatureDefinitionPowerBuilder
            .Create(PowerFighterSecondWind, "PowerFighterSecondWindTargeting")
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Position)
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden, new CustomBehaviorFilterTargetingPositionHalfMove())
            .AddToDB();

        PowerFighterSecondWind.AddCustomSubFeatures(
            new PowerOrSpellInitiatedByMeSecondWind(powerFighterSecondWindTargeting));
    }

    private static void LoadFighterTacticalMaster()
    {
        var powers = new List<FeatureDefinitionPower>();

        foreach (var mastery in new[] { MasteryProperty.Push, MasteryProperty.Sap, MasteryProperty.Slow })
        {
            var powerTacticalMasterChoice = FeatureDefinitionPowerSharedPoolBuilder
                .Create($"PowerFighterTacticalMaster{mastery}")
                .SetGuiPresentation($"FeatureWeaponMastery{mastery}", Category.Feature)
                .SetShowCasting(false)
                .SetSharedPool(ActivationTime.NoCost, PowerFighterTacticalMasterPool)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                        .Build())
                .AddToDB();

            powerTacticalMasterChoice.GuiPresentation.hidden = true;
            powers.Add(powerTacticalMasterChoice);
        }

        PowerBundle.RegisterPowerBundle(PowerFighterTacticalMasterPool, false, powers);
    }

    internal static void SwitchFighterIndomitableSaving()
    {
        UseIndomitableResistance.GuiPresentation.description =
            Main.Settings.EnableFighterIndomitableSaving2024
                ? "Feature/&EnhancedIndomitableResistanceDescription"
                : "Feature/&IndomitableResistanceDescription";
    }

    internal static void SwitchFighterStudiedAttacks()
    {
        Fighter.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureFighterStudiedAttacks);

        if (Main.Settings.EnableFighterStudiedAttacks2024)
        {
            Fighter.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureFighterStudiedAttacks, 13));
        }

        Fighter.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchFighterTacticalMaster()
    {
        Fighter.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetFighterTacticalMaster);

        if (Main.Settings.EnableFighterTacticalMaster2024)
        {
            Fighter.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetFighterTacticalMaster, 9));
        }

        Fighter.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchFighterTacticalProgression()
    {
        Fighter.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == FeatureFighterTacticalMind ||
            x.FeatureDefinition == FeatureFighterTacticalShift);

        if (Main.Settings.EnableFighterTacticalProgression2024)
        {
            Fighter.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(FeatureFighterTacticalMind, 2),
                new FeatureUnlockByLevel(FeatureFighterTacticalShift, 5));
        }

        Fighter.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchFighterSecondWind()
    {
        PowerFighterSecondWind.rechargeRate = Main.Settings.EnableFighterSecondWind2024
            ? RechargeRate.LongRest
            : RechargeRate.ShortRest;
    }

    internal static void SwitchFighterSkillOptions()
    {
        if (Main.Settings.EnableFighterSkillOptions2024)
        {
            PointPoolFighterSkillPoints.restrictedChoices.Add(SkillDefinitions.Persuasion);
        }
        else
        {
            PointPoolFighterSkillPoints.restrictedChoices.Remove(SkillDefinitions.Persuasion);
        }
    }

    private sealed class TryAlterOutcomeAttributeCheckTacticalMind : ITryAlterOutcomeAttributeCheck
    {
        public IEnumerator OnTryAlterAttributeCheck(
            GameLocationBattleManager battleManager,
            AbilityCheckData abilityCheckData,
            GameLocationCharacter defender,
            GameLocationCharacter helper)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(PowerFighterSecondWind, rulesetHelper);

            if (abilityCheckData.AbilityCheckRoll == 0 ||
                abilityCheckData.AbilityCheckRollOutcome != RollOutcome.Failure ||
                abilityCheckData.AbilityCheckSuccessDelta < -10 ||
                helper != defender ||
                rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            yield return helper.MyReactToDoNothing(
                ExtraActionId.DoNothingFree,
                defender,
                "TacticalMindCheck",
                "CustomReactionTacticalMindCheckDescription".Localized(Category.Reaction),
                ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated()
            {
                var dieRoll =
                    rulesetHelper.RollDie(DieType.D10, RollContext.None, false, AdvantageType.None, out _, out _);

                var abilityCheckModifier = abilityCheckData.AbilityCheckActionModifier;

                abilityCheckModifier.AbilityCheckModifierTrends.Add(
                    new TrendInfo(dieRoll, FeatureSourceType.CharacterFeature, FeatureFighterTacticalMind.Name,
                        FeatureFighterTacticalMind));

                abilityCheckModifier.AbilityCheckModifier += dieRoll;
                abilityCheckData.AbilityCheckSuccessDelta += dieRoll;

                if (abilityCheckData.AbilityCheckSuccessDelta >= 0)
                {
                    abilityCheckData.AbilityCheckRollOutcome = RollOutcome.Success;
                    usablePower.Consume();
                }

                rulesetHelper.LogCharacterActivatesAbility(
                    "Feature/&FeatureFighterTacticalMindTitle",
                    abilityCheckData.AbilityCheckSuccessDelta >= 0
                        ? "Feedback/&TacticalMindCheckToHitRollSuccess"
                        : "Feedback/&TacticalMindCheckToHitRollFailure",
                    extra: [(ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString())]);
            }
        }
    }

    private sealed class PowerOrSpellInitiatedByMeSecondWind(FeatureDefinitionPower powerDummyTargeting)
        : IPowerOrSpellInitiatedByMe
    {
        public IEnumerator OnPowerOrSpellInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (!Main.Settings.EnableFighterTacticalProgression2024)
            {
                yield break;
            }

            var attacker = action.ActingCharacter;
            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker.GetClassLevel(Fighter) < 5)
            {
                yield break;
            }

            yield return CampaignsContext.SelectPosition(action, powerDummyTargeting);

            var position = action.ActionParams.Positions[0];

            if (attacker.LocationPosition == position)
            {
                yield break;
            }

            rulesetAttacker.InflictCondition(
                ConditionWithdrawn.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                // all disengaging in game is set under TagCombat (why?)
                AttributeDefinitions.TagCombat,
                rulesetAttacker.Guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionWithdrawn.Name,
                attacker.UsedTacticalMoves,
                0,
                0);

            var distance = (int)int3.Distance(attacker.LocationPosition, position);

            attacker.UsedTacticalMoves -= distance;
            attacker.UsedTacticalMovesChanged?.Invoke(attacker);

            var actionParams = new CharacterActionParams(
                attacker, Id.TacticalMove, MoveStance.Run, position, LocationDefinitions.Orientation.North)
            {
                BoolParameter3 = false, BoolParameter5 = false
            };

            action.ResultingActions.Add(new CharacterActionMove(actionParams));
        }
    }

    private sealed class PhysicalAttackFinishedOnMeStudiedAttacks : IPhysicalAttackFinishedOnMe
    {
        public IEnumerator OnPhysicalAttackFinishedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetDefender = defender.RulesetActor;

            if (rulesetDefender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionStudiedAttacks.Name, out var activeCondition) &&
                activeCondition.SourceGuid == attacker.Guid &&
                rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                rulesetDefender.RemoveCondition(activeCondition);
            }

            yield break;
        }
    }

    private sealed class PhysicalAttackFinishedByMeStudiedAttacks : IPhysicalAttackFinishedByMe
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
            if (rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetActor;

            rulesetDefender.InflictCondition(
                ConditionStudiedAttacks.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.Guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionStudiedAttacks.Name,
                0,
                0,
                0);
        }
    }

    private sealed class RollSavingThrowInitiatedIndomitableSaving : IRollSavingThrowInitiated
    {
        public void OnSavingThrowInitiated(
            RulesetActor rulesetActorCaster,
            RulesetActor rulesetActorDefender,
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
            if (rulesetActorDefender is not RulesetCharacterHero rulesetCharacterDefender)
            {
                return;
            }

            var classLevel = rulesetCharacterDefender.GetClassLevel(Fighter);

            rollModifier += classLevel;
            modifierTrends.Add(
                new TrendInfo(classLevel, FeatureSourceType.CharacterFeature,
                    AttributeModifierFighterIndomitable.Name,
                    AttributeModifierFighterIndomitable));
        }
    }
}
