using System.Collections;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Interfaces;
using TA;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMovementAffinitys;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class Tabletop2024Context
{
    private static readonly FeatureDefinitionCombatAffinity CombatAffinityConditionSurprised =
        FeatureDefinitionCombatAffinityBuilder
            .Create("CombatAffinityConditionSurprised")
            .SetGuiPresentationNoContent(true)
            .SetInitiativeAffinity(AdvantageType.Disadvantage)
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
        LoadFighterTacticalProgression();
        LoadMonkStunningStrike();
        LoadOneDndSpellGuidanceSubspells();
        LoadOneDndSpellSpareTheDying();
        LoadOneDndSpellTrueStrike();
        LoadPaladinRestoringTouch();
        LoadRogueCunningStrike();
        LoadSorcererSorcerousRestoration();
        LoadWizardMemorizeSpell();
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
