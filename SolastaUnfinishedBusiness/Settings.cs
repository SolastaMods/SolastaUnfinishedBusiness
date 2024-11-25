using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Displays;
using SolastaUnfinishedBusiness.Models;
using UnityModManagerNet;

namespace SolastaUnfinishedBusiness;

public sealed class Core;

[Serializable]
[XmlRoot(ElementName = "Settings")]
public class Settings : UnityModManager.ModSettings
{
    //
    // UI Saved State
    //

    public int DisplayModMessage { get; set; }
    public int EnableDiagsDump { get; set; }
    public int SelectedTab { get; set; }

    //
    // SETTINGS UI TOGGLES
    //

    public bool DisplayMultiplayerToggle { get; set; }
    public bool DisplayOneDndToggle { get; set; }
    public bool DisplayTabletopToggle { get; set; }
    public bool DisplayRacesToggle { get; set; }
    public bool DisplaySubracesToggle { get; set; }
    public bool DisplayBackgroundsToggle { get; set; }
    public bool DisplayFeatsToggle { get; set; }
    public bool DisplayFeatGroupsToggle { get; set; }
    public bool DisplayFightingStylesToggle { get; set; }
    public bool DisplayInvocationsToggle { get; set; }
    public bool DisplayMetamagicToggle { get; set; }
    public bool DisplayCraftingToggle { get; set; }
    public bool DisplayItemsToggle { get; set; }
    public bool DisplayBackgroundsAndRacesGeneralToggle { get; set; }
    public bool DisplayProficienciesGeneralToggle { get; set; }
    public bool DisplaySpellsGeneralToggle { get; set; }
    public bool DisplaySubClassesGeneralToggle { get; set; }
    public SerializableDictionary<string, bool> DisplayKlassToggle { get; set; } = [];
    public SerializableDictionary<string, bool> DisplaySpellListsToggle { get; set; } = [];

    //
    // SETTINGS HIDDEN ON UI
    //

    public List<string> DefaultPartyHeroes { get; } = [];
    public bool EnableCtrlClickOnlySwapsMainHand { get; set; } = true;
    public bool EnableDisplaySorceryPointBoxSorcererOnly { get; set; } = true;
    public bool EnableLevelUpFeaturesSelection { get; set; } = true;
    public bool EnableSameWidthFeatSelection { get; set; } = true;
    public bool EnableSameWidthInvocationSelection { get; set; } = true;
    public bool EnableSortingFightingStyles { get; set; } = true;
    public bool EnableSortingSubclasses { get; set; } = true;
    public bool FixAsianLanguagesTextWrap { get; set; } = true;
    public bool KeepCharactersPanelOpenAndHeroSelectedAfterLevelUp { get; set; } = true;
    public bool DisableStreamlinedMultiLevelUp { get; set; } = true;

    //
    // Gameplay - General
    //

    public bool EnablePcgRandom { get; set; }
    public bool EnableCustomPortraits { get; set; }
    public bool DisableMultilineSpellOffering { get; set; }
    public bool DisableUnofficialTranslations { get; set; }

    //
    // Gameplay - Rules
    //

    public bool EnableEpicPointsAndArray { get; set; }
    public bool EnableLevel20 { get; set; }
    public bool EnableMulticlass { get; set; }
    public int MaxAllowedClasses { get; set; }
    public bool DisplayAllKnownSpellsDuringLevelUp { get; set; }
    public bool DisplayPactSlotsOnSpellSelectionPanel { get; set; }
    public bool EnableMinInOutAttributes { get; set; }
    public bool EnableActionSwitching { get; set; }
    public bool DontEndTurnAfterReady { get; set; }
    public bool EnableProneAction { get; set; }
    public bool EnableGrappleAction { get; set; }
    public bool EnableHelpAction { get; set; }
    public bool EnableRespecAction { get; set; }
    public bool EnableUnarmedMainAttackAction { get; set; }
    public bool UseOfficialAdvantageDisadvantageRules { get; set; }
    public bool UseAlternateSpellPointsSystem { get; set; }
    public bool UseOfficialFlankingRules { get; set; }
    public bool UseMathFlankingRules { get; set; }
    public bool UseOfficialFlankingRulesAlsoForReach { get; set; }
    public bool UseOfficialFlankingRulesAlsoForRanged { get; set; }
    public bool UseOfficialFlankingRulesButAddAttackModifier { get; set; }
    public bool BlindedConditionDontAllowAttackOfOpportunity { get; set; }
    public bool UseOfficialLightingObscurementAndVisionRules { get; set; }
    public bool OfficialObscurementRulesInvisibleCreaturesCanBeTarget { get; set; }
    public bool OfficialObscurementRulesCancelAdvDisPairs { get; set; }
    public bool OfficialObscurementRulesHeavilyObscuredAsProjectileBlocker { get; set; }
    public bool OfficialObscurementRulesMagicalDarknessAsProjectileBlocker { get; set; }
    public bool OfficialObscurementRulesTweakMonsters { get; set; }
    public bool KeepStealthOnHeroIfPerceivedDuringSurpriseAttack { get; set; }
    public bool StealthDoesNotBreakWithSubtle { get; set; }
    public bool StealthBreaksWhenAttackHits { get; set; }
    public bool StealthBreaksWhenAttackMisses { get; set; }
    public bool StealthBreaksWhenCastingMaterial { get; set; }
    public bool StealthBreaksWhenCastingVerbose { get; set; }
    public bool StealthBreaksWhenCastingSomatic { get; set; }
    public bool AccountForAllDiceOnSavageAttack { get; set; }
    public bool AddDexModifierToEnemiesInitiativeRoll { get; set; }
    public bool EnemiesAlwaysRollInitiative { get; set; }
    public bool AllowFlightSuspend { get; set; }
    public bool FlightSuspendWingedBoots { get; set; }
    public bool EnablePullPushOnVerticalDirection { get; set; }
    public bool FullyControlConjurations { get; set; }
    public bool EnableHigherGroundRules { get; set; }
    public bool EnableSurprisedToEnforceDisadvantage { get; set; }
    public bool EnableTeleportToRemoveRestrained { get; set; }
    public bool EnableCharactersOnFireToEmitLight { get; set; }
    public bool ColdResistanceAlsoGrantsImmunityToChilledCondition { get; set; }
    public bool ColdImmunityAlsoGrantsImmunityToChilledAndFrozenCondition { get; set; }
    public int SenseNormalVisionRangeMultiplier { get; set; } = 1;
    public int CriticalHitModeAllies { get; set; }
    public int CriticalHitModeEnemies { get; set; }
    public int CriticalHitModeNeutral { get; set; }

    //
    // Gameplay - Campaigns
    //

    public bool NoExperienceOnLevelUp { get; set; }
    public bool OverrideMinMaxLevel { get; set; }
    public float FasterTimeModifier { get; set; } = CampaignsDisplay.DefaultFastTimeModifier;
    public int MultiplyTheExperienceGainedBy { get; set; } = 100;
    public int OverridePartySize { get; set; } = ToolsContext.GamePartySize;
    public bool AllowAllPlayersOnNarrativeSequences { get; set; }
    public bool AddPickPocketableLoot { get; set; }
    public bool AltOnlyHighlightItemsInPartyFieldOfView { get; set; }
    public bool EnableAdditionalIconsOnLevelMap { get; set; }
    public bool HideExitsAndTeleportersGizmosIfNotDiscovered { get; set; }
    public bool EnableLogDialoguesToConsole { get; set; }
    public bool EnableSpeech { get; set; }
    public bool EnableSpeechFemaleVoice { get; set; }
    public float SpeechScale { get; set; } = 1f;
    public bool EnableHeroWithBestProficiencyToRollChoice { get; set; }
    public bool MarkInvisibleTeleportersOnLevelMap { get; set; }
    public bool EnableAlternateVotingSystem { get; set; }
    public bool EnableSumD20OnAlternateVotingSystem { get; set; }
    public bool AllowMoreRealStateOnRestPanel { get; set; }
    public bool EnableStatsOnHeroTooltip { get; set; }
    public bool EnableAdditionalBackstoryDisplay { get; set; }
    public bool EnableExtendedProficienciesPanelDisplay { get; set; }
    public bool HideMonsterHitPoints { get; set; }
    public bool RemoveBugVisualModels { get; set; }
    public bool ShowButtonWithControlledMonsterInfo { get; set; }

    // Battle
    public bool DontFollowCharacterInBattle { get; set; }
    public bool EnableElevationCameraToStayAtPosition { get; set; }
    public bool NeverMoveCameraOnEnemyTurn { get; set; }
    public bool EnableCancelEditOnRightMouseClick { get; set; }
    public int DontFollowMargin { get; set; } = 5;
    public int GridSelectedColor { get; set; } = 1;
    public int MovementGridWidthModifier { get; set; } = 100;
    public int OutlineGridWidthModifier { get; set; } = 100;
    public int OutlineGridWidthSpeed { get; set; } = 100;
    public bool EnableDistanceOnTooltip { get; set; }
    public bool ShowMotionFormPreview { get; set; }
    public int HighContrastTargetingAoeSelectedColor { get; set; }
    public int HighContrastTargetingSingleSelectedColor { get; set; }

    // Formation
    public int FormationGridSelectedSet { get; set; } = -1;

    public int[][][] FormationGridSets { get; set; } =
    [
        [
            new int[CampaignsContext.GridSize], new int[CampaignsContext.GridSize], new int[CampaignsContext.GridSize],
            new int[CampaignsContext.GridSize], new int[CampaignsContext.GridSize]
        ],
        [
            new int[CampaignsContext.GridSize], new int[CampaignsContext.GridSize], new int[CampaignsContext.GridSize],
            new int[CampaignsContext.GridSize], new int[CampaignsContext.GridSize]
        ],
        [
            new int[CampaignsContext.GridSize], new int[CampaignsContext.GridSize], new int[CampaignsContext.GridSize],
            new int[CampaignsContext.GridSize], new int[CampaignsContext.GridSize]
        ],
        [
            new int[CampaignsContext.GridSize], new int[CampaignsContext.GridSize], new int[CampaignsContext.GridSize],
            new int[CampaignsContext.GridSize], new int[CampaignsContext.GridSize]
        ],
        [
            new int[CampaignsContext.GridSize], new int[CampaignsContext.GridSize], new int[CampaignsContext.GridSize],
            new int[CampaignsContext.GridSize], new int[CampaignsContext.GridSize]
        ]
    ];

    // Merchants
    public bool ScaleMerchantPricesCorrectly { get; set; }
    public bool StockGorimStoreWithAllNonMagicalClothing { get; set; }
    public bool StockHugoStoreWithAdditionalFoci { get; set; }
    public bool StockGorimStoreWithAllNonMagicalInstruments { get; set; }
    public bool EnableAdditionalFociInDungeonMaker { get; set; }
    public bool RestockAntiquarians { get; set; }
    public bool RestockArcaneum { get; set; }
    public bool RestockCircleOfDanantar { get; set; }
    public bool RestockTowerOfKnowledge { get; set; }

    //
    // Gameplay - Items, Crafting & Merchants
    //

    public bool AllowAnyClassToUseArcaneShieldstaff { get; set; }
    public bool IdentifyAfterRest { get; set; }
    public bool IncreaseMaxAttunedItems { get; set; }
    public bool RemoveAttunementRequirements { get; set; }
    public bool AllowAnyClassToWearSylvanArmor { get; set; }
    public bool AllowClubsToBeThrown { get; set; }
    public bool UseOfficialFoodRationsWeight { get; set; }
    public bool MakeAllMagicStaveArcaneFoci { get; set; }
    public bool FixRingOfRegenerationHealRate { get; set; }
    public bool IgnoreHandXbowFreeHandRequirements { get; set; }
    public bool OneDndHealingPotionBonusAction { get; set; }
    public bool KeepInvisibilityWhenUsingItems { get; set; }
    public bool AddCustomIconsToOfficialItems { get; set; }
    public bool DisableAutoEquip { get; set; }
    public bool EnableInventoryFilteringAndSorting { get; set; }
    public bool EnableInventoryTaintNonProficientItemsRed { get; set; }
    public bool EnableInventoryTintKnownRecipesRed { get; set; }
    public bool EnableVersatileAmmunitionSlots { get; set; }
    public bool EnableVersatileOffHandSlot { get; set; }
    public int SetBeltOfDwarvenKindBeardChances { get; set; } = 50;

    // Crafting
    public bool ShowCraftingRecipeInDetailedTooltips { get; set; }
    public bool ShowCraftedItemOnRecipeIcon { get; set; }
    public bool SwapCraftedItemAndRecipeIcons { get; set; }
    public int RecipeCost { get; set; } = 200;
    public int TotalCraftingTimeModifier { get; set; }
    public bool AddNewWeaponsAndRecipesToShops { get; set; }
    public List<string> CraftingInStore { get; } = [];

    //
    // Interface - Dungeon Maker
    //

    public bool EnableLoggingInvalidReferencesInUserCampaigns { get; set; }
    public bool EnableSortingDungeonMakerAssets { get; set; }
    public bool AllowGadgetsAndPropsToBePlacedAnywhere { get; set; }
    public bool UnleashEnemyAsNpc { get; set; }
    public bool AddNewWeaponsAndRecipesToEditor { get; set; }
    public bool UnleashNpcAsEnemy { get; set; }
    public bool EnableVariablePlaceholdersOnTexts { get; set; }
    public bool EnableDungeonMakerModdedContent { get; set; }
    public string SelectedLanguageCode { get; set; } = TranslatorContext.English;

    //
    // Characters - Classes
    //

    public bool GrantScimitarSpecializationToBardRogue { get; set; }
    public bool ChangeBardicInspirationDurationToOneHour { get; set; }
    public bool EnableBardCounterCharmAsReactionAtLevel7 { get; set; }
    public bool EnableBardExpertiseOneLevelBefore { get; set; }
    public bool EnableBardSuperiorInspirationAtLevel18 { get; set; }
    public bool EnableBardWordsOfCreationAtLevel20 { get; set; }
    public bool RemoveBardMagicalSecretAt14And18 { get; set; }
    public bool RemoveBardSongOfRestAt2 { get; set; }
    public bool EnableBarbarianBrutalStrike { get; set; }
    public bool DisableBarbarianBrutalCritical { get; set; }
    public bool EnableBarbarianFightingStyle { get; set; }
    public bool EnableBarbarianRecklessSameBuffDebuffDuration { get; set; }
    public bool EnableBarbarianRegainOneRageAtShortRest { get; set; }
    public bool EnableDruidUseMetalArmor { get; set; }
    public bool EnableDruidPrimalOrderAndRemoveMediumArmorProficiency { get; set; }
    public bool SwapDruidToUseOneDndWeaponProficiency { get; set; }
    public bool AddFighterLevelToIndomitableSavingReroll { get; set; }
    public bool AddPersuasionToFighterSkillOptions { get; set; }
    public bool EnableSecondWindToUseOneDndUsagesProgression { get; set; }
    public bool EnableFighterWeaponSpecialization { get; set; }
    public bool AddHumanoidFavoredEnemyToRanger { get; set; }
    public bool EnableRangerNatureShroudAt14 { get; set; }
    public bool EnableRangerSpellCastingAtLevel1 { get; set; }
    public bool EnableMonkAbundantKi { get; set; }
    public bool EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack { get; set; }
    public bool EnableMonkDoNotRequireAttackActionForFlurry { get; set; }
    public bool EnableMonkFightingStyle { get; set; }
    public bool EnableMonkImprovedUnarmoredMovementToMoveOnTheWall { get; set; }
    public bool EnableMonkHandwrapsUseGauntletSlot { get; set; }
    public bool EnableMonkHeightenedMetabolism { get; set; }
    public bool EnableMonkSuperiorDefenseToReplaceEmptyBody { get; set; }
    public bool EnableMonkBodyAndMindToReplacePerfectSelf { get; set; }
    public bool EnableMonkWeaponSpecialization { get; set; }
    public bool SwapMonkToUseOneDndUnarmedDieTypeProgression { get; set; }
    public bool AddPaladinSmiteToggle { get; set; }
    public bool EnablePaladinLayOnHandsAsBonusAction { get; set; }
    public bool EnablePaladinSmiteAsBonusAction { get; set; }
    public bool EnablePaladinSpellCastingAtLevel1 { get; set; }
    public bool ShowChannelDivinityOnPortrait { get; set; }
    public bool EnableRogueCunningStrike { get; set; }
    public bool EnableRogueFightingStyle { get; set; }
    public bool EnableRogueSlipperyMind { get; set; }
    public bool EnableRogueSteadyAim { get; set; }
    public bool RemoveRogueBlindSense { get; set; }
    public bool EnableSorcererInnateSorceryAt1 { get; set; }
    public bool EnableSorcererMagicalGuidance { get; set; }
    public bool EnableSorcererQuickenedAction { get; set; }
    public bool HideQuickenedActionWhenMetamagicOff { get; set; }
    public bool EnableSorcerousRestorationAtLevel5 { get; set; }
    public bool EnableWarlockToUseOneDndInvocationProgression { get; set; }
    public bool EnableWarlockMagicalCunningAtLevel2AndImprovedEldritchMasterAt20 { get; set; }
    public bool EnableWarlockToLearnPatronAtLevel3 { get; set; }
    public bool EnableWizardToLearnScholarAtLevel2 { get; set; }
    public bool EnableWizardToLearnSchoolAtLevel3 { get; set; }
    public bool EnableSignatureSpellsRelearn { get; set; }

    //
    // Characters - Backgrounds, Races & Subraces
    //

    public bool EnableFlexibleBackgrounds { get; set; }
    public bool EnableFlexibleRaces { get; set; }
    public bool ChangeDragonbornElementalBreathUsages { get; set; }
    public bool EnableAlternateHuman { get; set; }
    public bool UseOfficialSmallRacesDisWithHeavyWeapons { get; set; }
    public bool DisableSenseDarkVisionFromAllRaces { get; set; }
    public bool DisableSenseSuperiorDarkVisionFromAllRaces { get; set; }
    public bool AddDarknessPerceptiveToDarkRaces { get; set; }
    public bool RaceLightSensitivityApplyOutdoorsOnly { get; set; }
    public int RaceSliderPosition { get; set; } = ModUi.DontDisplayDescription;
    public List<string> RaceEnabled { get; } = [];
    public int SubraceSliderPosition { get; set; } = ModUi.DontDisplayDescription;
    public List<string> SubraceEnabled { get; } = [];
    public int BackgroundSliderPosition { get; set; } = ModUi.DontDisplayDescription;
    public List<string> BackgroundEnabled { get; } = [];

    //
    // Characters - Feats, Groups, Fighting Styles, Invocations and Metamagic
    //

    public bool DisableLevelPrerequisitesOnModFeats { get; set; }
    public bool DisableRacePrerequisitesOnModFeats { get; set; }
    public bool DisableCastSpellPreRequisitesOnModFeats { get; set; }
    public int TotalFeatsGrantedFirstLevel { get; set; }
    public bool EnablesAsiAndFeat { get; set; }
    public bool EnableFeatsAtEveryFourLevels { get; set; }
    public bool EnableFeatsAtEveryFourLevelsMiddle { get; set; }
    public bool AccountForAllDiceOnFollowUpStrike { get; set; }
    public bool AllowCantripsTriggeringOnWarMagic { get; set; }
    public int FeatSliderPosition { get; set; } = ModUi.DontDisplayDescription;
    public List<string> FeatEnabled { get; } = [];
    public int FeatGroupSliderPosition { get; set; } = ModUi.DontDisplayDescription;
    public List<string> FeatGroupEnabled { get; } = [];
    public int FightingStyleSliderPosition { get; set; } = ModUi.DontDisplayDescription;
    public List<string> FightingStyleEnabled { get; } = [];
    public int InvocationSliderPosition { get; set; } = ModUi.DontDisplayDescription;
    public List<string> InvocationEnabled { get; } = [];
    public int MetamagicSliderPosition { get; set; } = ModUi.DontDisplayDescription;
    public List<string> MetamagicEnabled { get; } = [];

    //
    // Characters - Spells
    //

    public bool AllowBladeCantripsToUseReach { get; set; }
    public bool EnableOneDnDPreparedSpellsTables { get; set; }
    public bool EnableRitualOnAllCasters { get; set; }
    public bool QuickCastLightCantripOnWornItemsFirst { get; set; }
    public bool IllusionSpellsAutomaticallyFailAgainstTrueSightInRange { get; set; }
    public bool AllowTargetingSelectionWhenCastingChainLightningSpell { get; set; }
    public bool RemoveHumanoidFilterOnHideousLaughter { get; set; }
    public bool AddBleedingToLesserRestoration { get; set; }
    public bool BestowCurseNoConcentrationRequiredForSlotLevel5OrAbove { get; set; }
    public bool RemoveRecurringEffectOnEntangle { get; set; }
    public bool EnableUpcastConjureElementalAndFey { get; set; }
    public bool OnlyShowMostPowerfulUpcastConjuredElementalOrFey { get; set; }
    public bool ChangeSleetStormToCube { get; set; }
    public bool UseHeightOneCylinderEffect { get; set; }
    public bool FixEldritchBlastRange { get; set; }
    public bool ModifyGravitySlam { get; set; }
    public bool EnableOneDndBarkskinSpell { get; set; }
    public bool EnableOneDndDamagingSpellsUpgrade { get; set; }
    public bool EnableOneDndHealingSpellsUpgrade { get; set; }
    public bool EnableOneDndDivineFavorSpell { get; set; }
    public bool EnableOneDndGuidanceSpell { get; set; }
    public bool EnableOneDndHideousLaughterSpell { get; set; }
    public bool EnableOneDndHuntersMarkSpell { get; set; }
    public bool EnableOneDndLesserRestorationSpell { get; set; }
    public bool EnableOneDndMagicWeaponSpell { get; set; }
    public bool EnableOneDndPowerWordStunSpell { get; set; }
    public bool EnableOneDndSpareTheDyingSpell { get; set; }
    public bool EnableOneDndSpiderClimbSpell { get; set; }
    public bool EnableOneDndStoneSkinSpell { get; set; }
    public bool EnableOneDndTrueStrikeCantrip { get; set; }
    public bool AllowHasteCasting { get; set; }
    public bool AllowStackedMaterialComponent { get; set; }
    public bool EnableRelearnSpells { get; set; }
    public bool AllowDisplayingOfficialSpells { get; set; }
    public bool AllowDisplayingNonSuggestedSpells { get; set; }

    public SerializableDictionary<string, int> SpellListSliderPosition { get; set; } = [];
    public SerializableDictionary<string, List<string>> SpellListSpellEnabled { get; set; } = [];

    //
    // Characters - Subclasses
    //

    public bool AllowAlliesToPerceiveRangerGloomStalkerInNaturalDarkness { get; set; }
    public bool EnableBardHealingBalladOnLongRest { get; set; }
    public bool EnableBg3AbjurationArcaneWard { get; set; }
    public bool EnableRogueStrSaving { get; set; }
    public bool RemoveSchoolRestrictionsFromShadowCaster { get; set; }
    public bool RemoveSchoolRestrictionsFromSpellBlade { get; set; }
    public int WildSurgeDieRollThreshold { get; set; } = 2;
    public bool SwapAbjurationSavant { get; set; }
    public bool SwapEvocationSavant { get; set; }
    public bool SwapEvocationPotentCantripAndSculptSpell { get; set; }
    public SerializableDictionary<string, int> KlassListSliderPosition { get; set; } = [];
    public SerializableDictionary<string, List<string>> KlassListSubclassEnabled { get; set; } = [];

    //
    // Encounters - General
    //

    public bool EnableEnemiesControlledByPlayer { get; set; }
    public bool EnableHeroesControlledByComputer { get; set; }

    //
    // Debug
    //

    public bool DebugDisableVerifyDefinitionNameIsNotInUse { get; set; }

#if DEBUG
    public int WildSurgeEffectDie { get; set; }
    public bool DebugLogDefinitionCreation { get; set; }
    public bool DebugLogFieldInitialization { get; set; }
    public bool DebugLogVariantMisuse { get; set; }
#endif
}
