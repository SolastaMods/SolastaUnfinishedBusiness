using System.Collections;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using TA;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCombatAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMovementAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class Tabletop2024Context
{
    private static readonly FeatureDefinitionCombatAffinity CombatAffinityConditionSurprised =
        FeatureDefinitionCombatAffinityBuilder
            .Create("CombatAffinityConditionSurprised")
            .SetGuiPresentationNoContent(true)
            .SetInitiativeAffinity(AdvantageType.Disadvantage)
            .AddToDB();

    private static readonly FeatureDefinition AbilityCheckAffinityChampionRemarkableAthlete2024 =
        FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AbilityCheckAffinityChampionRemarkableAthlete2024")
            .SetGuiPresentation("ChampionRemarkableAthlete", Category.Feature)
            .BuildAndSetAffinityGroups(
                CharacterAbilityCheckAffinity.Advantage, DieType.D1, 0,
                AbilityCheckGroupOperation.AddDie, (AttributeDefinitions.Strength, SkillDefinitions.Athletics))
            .AddCustomSubFeatures(
                new PhysicalAttackFinishedByMeRemarkableAthlete(
                    FeatureDefinitionPowerBuilder
                        .Create("PowerChampionRemarkableAthlete")
                        .SetGuiPresentation("ChampionRemarkableAthlete", Category.Feature)
                        .SetShowCasting(false)
                        .SetEffectDescription(
                            EffectDescriptionBuilder
                                .Create()
                                .SetDurationData(DurationType.Round)
                                .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Position)
                                .Build())
                        .AddCustomSubFeatures(
                            ModifyPowerVisibility.Hidden,
                            new CustomBehaviorFilterTargetingPositionHalfMove())
                        .AddToDB()))
            .AddToDB();

    private static readonly FeatureDefinition FeatureChampionHeroicWarrior = FeatureDefinitionBuilder
        .Create("FeatureChampionHeroicWarrior")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(new CustomBehaviorHeroicWarrior())
        .AddToDB();

    internal static void LateLoad()
    {
        LoadBarbarianBrutalStrike();
        LoadBarbarianInstinctivePounce();
        LoadBarbarianPersistentRage();
        LoadBardCounterCharm();
        LoadClericBlessedStrikes();
        LoadClericChannelDivinity();
        LoadClericSearUndead();
        LoadDruidArchDruid();
        LoadDruidElementalFury();
        LoadDruidWildshape();
        LoadFighterSecondWind();
        LoadFighterStudiedAttacks();
        LoadFighterTacticalMaster();
        LoadFighterTacticalProgression();
        LoadMonkStunningStrike();
        LoadOneDndSpellGuidanceSubspells();
        LoadOneDndSpellSpareTheDying();
        LoadOneDndSpellTrueStrike();
        LoadPaladinRestoringTouch();
        LoadRogueCunningStrike();
        LoadSorcererSorcerousRestoration();
        LoadWizardMemorizeSpell();
        LoadWeaponMastery();
        SwitchBarbarianBrutalStrike();
        SwitchBarbarianInstinctivePounce();
        SwitchBarbarianPersistentRage();
        SwitchBarbarianReckless();
        SwitchBarbarianRage();
        SwitchBarbarianRelentlessRage();
        SwitchBardBardMagicalSecrets();
        SwitchBardBardicInspiration();
        SwitchBardCounterCharm();
        SwitchBardExpertiseOneLevelBefore();
        SwitchBardSongOfRest();
        SwitchBardSuperiorInspiration();
        SwitchBardWordsOfCreation();
        SwitchClericBlessedStrikes();
        SwitchClericChannelDivinity();
        SwitchClericDivineOrder();
        SwitchClericDomainLearningLevel();
        SwitchClericSearUndead();
        SwitchDruidArchDruid();
        SwitchDruidCircleLearningLevel();
        SwitchDruidElementalFury();
        SwitchDruidMetalArmor();
        SwitchDruidPrimalOrder();
        SwitchDruidWeaponProficiency();
        SwitchDruidWildResurgence();
        SwitchDruidWildshape();
        SwitchFighterIndomitableSaving();
        SwitchFighterSecondWind();
        SwitchFighterSkillOptions();
        SwitchFighterStudiedAttacks();
        SwitchFighterTacticalMaster();
        SwitchFighterTacticalProgression();
        SwitchMonkBodyAndMind();
        SwitchMonkDeflectAttacks();
        SwitchMonkFocus();
        SwitchMonkHeightenedFocus();
        SwitchMonkMartialArts();
        SwitchMonkStunningStrike();
        SwitchMonkSelfRestoration();
        SwitchMonkSuperiorDefense();
        SwitchMonkUncannyMetabolism();
        SwitchOneDndDamagingSpellsUpgrade();
        SwitchOneDndHealingSpellsUpgrade();
        SwitchOneDndPreparedSpellsTables();
        SwitchOneDndSpellBarkskin();
        SwitchOneDndSpellDivineFavor();
        SwitchOneDndSpellGuidance();
        SwitchOneDndSpellHideousLaughter();
        SwitchOneDndSpellHuntersMark();
        SwitchOneDndSpellLesserRestoration();
        SwitchOneDndSpellMagicWeapon();
        SwitchOneDndSpellPowerWordStun();
        SwitchOneDndSpellRitualOnAllCasters();
        SwitchOneDndSpellSpareTheDying();
        SwitchOneDndSpellSpiderClimb();
        SwitchOneDndSpellStoneSkin();
        SwitchPaladinAbjureFoes();
        SwitchPaladinChannelDivinity();
        SwitchPaladinLayOnHand();
        SwitchPaladinRestoringTouch();
        SwitchPaladinSpellCastingAtOne();
        SwitchPoisonsBonusAction();
        SwitchPotionsBonusAction();
        SwitchRangerDeftExplorer();
        SwitchRangerExpertise();
        SwitchRangerFavoredEnemy();
        SwitchRangerFeralSenses();
        SwitchRangerFoeSlayers();
        SwitchRangerNatureShroud();
        SwitchRangerPreciseHunter();
        SwitchRangerPrimevalAwareness();
        SwitchRangerRelentlessHunter();
        SwitchRangerRoving();
        SwitchRangerSpellCastingAtOne();
        SwitchRangerTireless();
        SwitchRogueBlindSense();
        SwitchRogueCunningStrike();
        SwitchRogueReliableTalent();
        SwitchRogueSlipperyMind();
        SwitchRogueSteadyAim();
        SwitchSorcererArcaneApotheosis();
        SwitchSorcererInnateSorcery();
        SwitchSorcererMetamagic();
        SwitchSorcererOriginLearningLevel();
        SwitchSorcererSorcerousRestorationAtLevel5();
        SwitchSurprisedEnforceDisadvantage();
        SwitchWarlockInvocationsProgression();
        SwitchWarlockMagicalCunningAndImprovedEldritchMaster();
        SwitchWarlockPatronLearningLevel();
        SwitchWizardMemorizeSpell();
        SwitchWizardScholar();
        SwitchWizardSchoolOfMagicLearningLevel();
        SwitchWeaponMastery();
    }

    internal static void SwitchSurprisedEnforceDisadvantage()
    {
        if (Main.Settings.EnableSurprisedToEnforceDisadvantage)
        {
            ConditionDefinitions.ConditionSurprised.Features.SetRange(CombatAffinityConditionSurprised);
            ConditionDefinitions.ConditionSurprised.GuiPresentation.Description = Gui.NoLocalization;
        }
        else
        {
            ConditionDefinitions.ConditionSurprised.Features.SetRange(
                ActionAffinityConditionSurprised,
                MovementAffinityConditionSurprised);
            ConditionDefinitions.ConditionSurprised.GuiPresentation.Description =
                "Rules/&ConditionSurprisedDescription";
        }
    }

    internal static void SwitchMartialChampion()
    {
        MartialChampion.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == FeatureSetChampionRemarkableAthlete ||
            x.FeatureDefinition == FightingStyleChampionAdditional ||
            x.FeatureDefinition == FeatureChampionHeroicWarrior);

        if (Main.Settings.EnableMartialChampion2024)
        {
            FeatureSetChampionRemarkableAthlete.GuiPresentation.description =
                "Feature/&ChampionRemarkableAthleteExtendedDescription";
            FeatureSetChampionRemarkableAthlete.FeatureSet.SetRange(
                AbilityCheckAffinityChampionRemarkableAthlete2024,
                CombatAffinityEagerForBattle);

            MartialChampion.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(FeatureSetChampionRemarkableAthlete, 3),
                new FeatureUnlockByLevel(FightingStyleChampionAdditional, 7),
                new FeatureUnlockByLevel(FeatureChampionHeroicWarrior, 10));
        }
        else
        {
            FeatureSetChampionRemarkableAthlete.GuiPresentation.description =
                "Feature/&ChampionRemarkableAthleteDescription";
            FeatureSetChampionRemarkableAthlete.FeatureSet.SetRange(
                AbilityCheckAffinityChampionRemarkableAthlete,
                MovementAffinityChampionRemarkableAthlete);

            MartialChampion.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(FeatureSetChampionRemarkableAthlete, 7),
                new FeatureUnlockByLevel(FightingStyleChampionAdditional, 10));
        }

        MartialChampion.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    private sealed class PhysicalAttackFinishedByMeRemarkableAthlete(FeatureDefinitionPower powerDummyTargeting)
        : IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(GameLocationBattleManager battleManager, CharacterAction action,
            GameLocationCharacter attacker, GameLocationCharacter defender, RulesetAttackMode attackMode,
            RollOutcome rollOutcome, int damageAmount)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

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
                attacker, ActionDefinitions.Id.TacticalMove, ActionDefinitions.MoveStance.Run, position,
                LocationDefinitions.Orientation.North) { BoolParameter3 = false, BoolParameter5 = false };

            action.ResultingActions.Add(new CharacterActionMove(actionParams));
        }
    }

    private sealed class CustomBehaviorHeroicWarrior : ITryAlterOutcomeAttack, ITryAlterOutcomeAttributeCheck,
        ITryAlterOutcomeSavingThrow
    {
        private const string ChampionHeroicWarrior = "ChampionHeroicWarrior";

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
            if (action.AttackRollOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure) ||
                attacker != helper ||
                !helper.OncePerTurnIsValid(ChampionHeroicWarrior))
            {
                yield break;
            }

            // any reaction within an attack flow must use the attacker as waiter
            yield return helper.MyReactToDoNothing(
                ExtraActionId.DoNothingReaction,
                attacker,
                "HeroicWarrior",
                "CustomReactionHeroicWarriorAttackDescription".Formatted(Category.Reaction, attacker.Name,
                    defender.Name, helper.Name),
                ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated()
            {
                helper.SetSpecialFeatureUses(ChampionHeroicWarrior, 0);

                var rulesetHelper = helper.RulesetCharacter;
                var dieRoll = rulesetHelper.RollDie(DieType.D20, RollContext.None, false, AdvantageType.None, out _,
                    out _);
                var previousRoll = action.AttackRoll;

                if (dieRoll <= action.AttackRoll)
                {
                    rulesetHelper.LogCharacterActivatesAbility(
                        "Feature/&FeatureChampionHeroicWarriorTitle",
                        "Feedback/&IsNotLuckyLower",
                        extra:
                        [
                            (ConsoleStyleDuplet.ParameterType.Negative, dieRoll.ToString()),
                            (ConsoleStyleDuplet.ParameterType.Positive, action.AttackRoll.ToString())
                        ]);

                    return;
                }

                action.AttackSuccessDelta += dieRoll - action.AttackRoll;
                action.AttackRoll = dieRoll;

                if (action.AttackSuccessDelta >= 0)
                {
                    action.AttackRollOutcome = dieRoll == 20 ? RollOutcome.CriticalSuccess : RollOutcome.Success;
                }

                rulesetHelper.LogCharacterActivatesAbility(
                    "Feature/&FeatureChampionHeroicWarriorTitle",
                    "Feedback/&BountifulLuckAttackToHitRoll",
                    extra:
                    [
                        (dieRoll > previousRoll ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                            dieRoll.ToString()),
                        (previousRoll > dieRoll ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                            previousRoll.ToString())
                    ]);
            }
        }

        public IEnumerator OnTryAlterAttributeCheck(
            GameLocationBattleManager battleManager,
            AbilityCheckData abilityCheckData,
            GameLocationCharacter defender,
            GameLocationCharacter helper)
        {
            if (abilityCheckData.AbilityCheckRollOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure) ||
                helper != defender ||
                !helper.OncePerTurnIsValid(ChampionHeroicWarrior))
            {
                yield break;
            }

            // any reaction within an attribute check flow must use the yielder as waiter
            yield return helper.MyReactToDoNothing(
                ExtraActionId.DoNothingReaction,
                helper,
                "HeroicWarriorCheck",
                "CustomReactionHeroicWarriorCheckDescription".Formatted(Category.Reaction, defender.Name, helper.Name),
                ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated()
            {
                helper.SetSpecialFeatureUses(ChampionHeroicWarrior, 0);

                var rulesetHelper = helper.RulesetCharacter;
                var dieRoll = rulesetHelper.RollDie(DieType.D20, RollContext.None, false, AdvantageType.None, out _,
                    out _);
                var previousRoll = abilityCheckData.AbilityCheckRoll;

                if (dieRoll <= abilityCheckData.AbilityCheckRoll)
                {
                    rulesetHelper.LogCharacterActivatesAbility(
                        "Feature/&FeatureChampionHeroicWarriorTitle",
                        "Feedback/&IsNotLuckyLower",
                        extra:
                        [
                            (ConsoleStyleDuplet.ParameterType.Negative, dieRoll.ToString()),
                            (ConsoleStyleDuplet.ParameterType.Positive, abilityCheckData.AbilityCheckRoll.ToString())
                        ]);

                    return;
                }

                abilityCheckData.AbilityCheckSuccessDelta += dieRoll - abilityCheckData.AbilityCheckRoll;
                abilityCheckData.AbilityCheckRoll = dieRoll;
                abilityCheckData.AbilityCheckRollOutcome = abilityCheckData.AbilityCheckSuccessDelta >= 0
                    ? RollOutcome.Success
                    : RollOutcome.Failure;

                rulesetHelper.LogCharacterActivatesAbility(
                    "Feature/&FeatureChampionHeroicWarriorTitle",
                    "Feedback/&BountifulLuckCheckToHitRoll",
                    extra:
                    [
                        (dieRoll > previousRoll ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                            dieRoll.ToString()),
                        (previousRoll > dieRoll ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                            previousRoll.ToString())
                    ]);
            }
        }

        public IEnumerator OnTryAlterOutcomeSavingThrow(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            SavingThrowData savingThrowData,
            bool hasHitVisual)
        {
            if (savingThrowData.SaveOutcome != RollOutcome.Failure ||
                helper != defender ||
                !helper.OncePerTurnIsValid(ChampionHeroicWarrior))
            {
                yield break;
            }

            var saveDC = savingThrowData.SaveDC;
            var rollModifier = savingThrowData.SaveBonusAndRollModifier;
            var savingRoll = savingThrowData.SaveOutcomeDelta - rollModifier + saveDC;

            // any reaction within a saving flow must use the yielder as waiter
            yield return helper.MyReactToDoNothing(
                ExtraActionId.DoNothingReaction,
                helper,
                "HeroicWarriorSaving",
                "CustomReactionHeroicWarriorSavingDescription".Formatted(
                    Category.Reaction, defender.Name, attacker?.Name ?? ReactionRequestCustom.EnvTitle,
                    savingThrowData.Title),
                ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated()
            {
                helper.SetSpecialFeatureUses(ChampionHeroicWarrior, 0);

                var rulesetHelper = helper.RulesetCharacter;
                var dieRoll = rulesetHelper.RollDie(DieType.D20, RollContext.None, false, AdvantageType.None, out _,
                    out _);

                if (dieRoll <= savingRoll)
                {
                    rulesetHelper.LogCharacterActivatesAbility(
                        "Feature/&FeatureChampionHeroicWarriorTitle",
                        "Feedback/&IsNotLuckyLower",
                        extra:
                        [
                            (ConsoleStyleDuplet.ParameterType.Negative, dieRoll.ToString()),
                            (ConsoleStyleDuplet.ParameterType.Positive, savingRoll.ToString())
                        ]);

                    return;
                }

                savingThrowData.SaveOutcomeDelta += dieRoll - savingRoll;
                savingThrowData.SaveOutcome =
                    savingThrowData.SaveOutcomeDelta >= 0 ? RollOutcome.Success : RollOutcome.Failure;

                rulesetHelper.LogCharacterActivatesAbility(
                    "Feature/&FeatureChampionHeroicWarriorTitle",
                    "Feedback/&BountifulLuckSavingToHitRoll",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString()),
                        (ConsoleStyleDuplet.ParameterType.Negative, savingRoll.ToString())
                    ]);
            }
        }
    }

    private sealed class CustomBehaviorFilterTargetingPositionHalfMove
        : IFilterTargetingPosition, IIgnoreInvisibilityInterruptionCheck
    {
        public IEnumerator ComputeValidPositions(CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            cursorLocationSelectPosition.validPositionsCache.Clear();

            var actingCharacter = cursorLocationSelectPosition.ActionParams.ActingCharacter;
            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var halfMaxTacticalMoves = (actingCharacter.MaxTacticalMoves + 1) / 2; // half-rounded up
            var boxInt = new BoxInt(actingCharacter.LocationPosition, int3.zero, int3.zero);

            boxInt.Inflate(halfMaxTacticalMoves, 0, halfMaxTacticalMoves);

            foreach (var position in boxInt.EnumerateAllPositionsWithin())
            {
                if (!positioningService.CanPlaceCharacter(
                        actingCharacter, position, CellHelpers.PlacementMode.Station) ||
                    !positioningService.CanCharacterStayAtPosition_Floor(
                        actingCharacter, position, onlyCheckCellsWithRealGround: true))
                {
                    continue;
                }

                cursorLocationSelectPosition.validPositionsCache.Add(position);

                if (cursorLocationSelectPosition.stopwatch.Elapsed.TotalMilliseconds > 0.5)
                {
                    yield return null;
                }
            }
        }
    }
}
