using System.Collections;
using System.Linq;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Interfaces;
using TA;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
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
        LoadFighterSecondWind();
        LoadFighterStudiedAttacks();
        LoadFighterTacticalProgression();
        LoadMonkHeightenedMetabolism();
        LoadOneDndSpellGuidanceSubspells();
        LoadOneDndSpellSpareTheDying();
        LoadOneDndTrueStrike();
        LoadRogueCunningStrike();
        LoadSorcererSorcerousRestoration();
        LoadWizardMemorizeSpell();
        SwitchBarbarianBrutalStrike();
        SwitchBarbarianInstinctivePounce();
        SwitchBarbarianPersistentRage();
        SwitchBarbarianReckless();
        SwitchBarbarianRegainOneRageAtShortRest();
        SwitchBarbarianRelentlessRage();
        SwitchBardBardMagicalSecrets();
        SwitchBardBardicInspiration();
        SwitchBardCounterCharm();
        SwitchBardExpertiseOneLevelBefore();
        SwitchBardSongOfRest();
        SwitchBardSuperiorInspiration();
        SwitchBardWordsOfCreation();
        SwitchDruidMetalArmor();
        SwitchDruidPrimalOrderAndRemoveMediumArmorProficiency();
        SwitchDruidWeaponProficiency();
        SwitchFighterIndomitableSaving();
        SwitchFighterSecondWind();
        SwitchFighterSkillOptions();
        SwitchFighterStudiedAttacks();
        SwitchFighterTacticalProgression();
        SwitchMonkBodyAndMind();
        SwitchMonkDoNotRequireAttackActionForBonusUnarmoredAttack();
        SwitchMonkDoNotRequireAttackActionForFlurry();
        SwitchMonkHeightenedMetabolism();
        SwitchMonkSuperiorDefense();
        SwitchOneDndDamagingSpellsUpgrade();
        SwitchOneDndHealingSpellsUpgrade();
        SwitchOneDndMonkUnarmedDieTypeProgression();
        SwitchOneDndPaladinLayOnHandAsBonusAction();
        SwitchOneDndPaladinLearnSpellCastingAtOne();
        SwitchOneDndPreparedSpellsTables();
        SwitchOneDndRangerLearnSpellCastingAtOne();
        SwitchOneDndSpellBarkskin();
        SwitchOneDndSpellDivineFavor();
        SwitchOneDndSpellGuidance();
        SwitchOneDndSpellHideousLaughter();
        SwitchOneDndSpellHuntersMark();
        SwitchOneDndSpellLesserRestoration();
        SwitchOneDndSpellMagicWeapon();
        SwitchOneDndSpellPowerWordStun();
        SwitchOneDndSpellSpareTheDying();
        SwitchOneDndSpellSpiderClimb();
        SwitchOneDndSpellStoneSkin();
        SwitchPoisonsBonusAction();
        SwitchPotionsBonusAction();
        SwitchRangerNatureShroud();
        SwitchRogueBlindSense();
        SwitchRogueCunningStrike();
        SwitchRogueReliableTalent();
        SwitchRogueSlipperyMind();
        SwitchRogueSteadyAim();
        SwitchSorcererArcaneApotheosis();
        SwitchSorcererInnateSorcery();
        SwitchSorcerousRestorationAtLevel5();
        SwitchSpellRitualOnAllCasters();
        SwitchSurprisedEnforceDisadvantage();
        SwitchWarlockInvocationsProgression();
        SwitchWarlockMagicalCunningAndImprovedEldritchMaster();
        SwitchWarlockPatronLearningLevel();
        SwitchWizardMemorizeSpell();
        SwitchWizardScholar();
        SwitchWizardSchoolOfMagicLearningLevel();
    }

    internal static void SwitchOneDndPaladinLearnSpellCastingAtOne()
    {
        var level = Main.Settings.EnablePaladinSpellCastingAtLevel1 ? 1 : 2;

        foreach (var featureUnlock in Paladin.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureDefinitionCastSpells.CastSpellPaladin))
        {
            featureUnlock.level = level;
        }

        // allows back and forth compatibility with EnableRitualOnAllCasters2024
        foreach (var featureUnlock in Paladin.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureSetClericRitualCasting))
        {
            featureUnlock.level = level;
        }

        Paladin.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);

        if (Main.Settings.EnablePaladinSpellCastingAtLevel1)
        {
            FeatureDefinitionCastSpells.CastSpellPaladin.slotsPerLevels = SharedSpellsContext.HalfRoundUpCastingSlots;
            SharedSpellsContext.ClassCasterType[PaladinClass] =
                FeatureDefinitionCastSpellBuilder.CasterProgression.HalfRoundUp;
        }
        else
        {
            FeatureDefinitionCastSpells.CastSpellPaladin.slotsPerLevels = SharedSpellsContext.HalfCastingSlots;
            SharedSpellsContext.ClassCasterType[PaladinClass] =
                FeatureDefinitionCastSpellBuilder.CasterProgression.Half;
        }
    }

    internal static void SwitchOneDndRangerLearnSpellCastingAtOne()
    {
        var level = Main.Settings.EnableRangerSpellCastingAtLevel1 ? 1 : 2;

        foreach (var featureUnlock in Ranger.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureDefinitionCastSpells.CastSpellRanger))
        {
            featureUnlock.level = level;
        }

        // allows back and forth compatibility with EnableRitualOnAllCasters2024
        foreach (var featureUnlock in Ranger.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureSetClericRitualCasting))
        {
            featureUnlock.level = level;
        }

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);

        if (Main.Settings.EnableRangerSpellCastingAtLevel1)
        {
            FeatureDefinitionCastSpells.CastSpellRanger.slotsPerLevels = SharedSpellsContext.HalfRoundUpCastingSlots;
            SharedSpellsContext.ClassCasterType[RangerClass] =
                FeatureDefinitionCastSpellBuilder.CasterProgression.HalfRoundUp;
        }
        else
        {
            FeatureDefinitionCastSpells.CastSpellRanger.slotsPerLevels = SharedSpellsContext.HalfCastingSlots;
            SharedSpellsContext.ClassCasterType[RangerClass] =
                FeatureDefinitionCastSpellBuilder.CasterProgression.Half;
        }
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

    internal static void SwitchOneDndPaladinLayOnHandAsBonusAction()
    {
        PowerPaladinLayOnHands.activationTime = Main.Settings.EnablePaladinLayOnHandsAsBonusAction2024
            ? ActivationTime.BonusAction
            : ActivationTime.Action;
    }

    internal static void SwitchRangerNatureShroud()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == FeatureDefinitionPowerNatureShroud);

        if (Main.Settings.EnableRangerNatureShroud2024)
        {
            Ranger.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureDefinitionPowerNatureShroud, 14));
        }

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
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
