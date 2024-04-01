using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Displays;
using SolastaUnfinishedBusiness.Models;
using UnityModManagerNet;

// ReSharper disable StringLiteralTypo

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

    public bool DisplayRacesToggle { get; set; }
    public bool DisplaySubracesToggle { get; set; }
    public bool DisplayBackgroundsToggle { get; set; }
    public bool DisplayFeatsToggle { get; set; }
    public bool DisplayFeatGroupsToggle { get; set; }
    public bool DisplayFightingStylesToggle { get; set; }
    public bool DisplayInvocationsToggle { get; set; }
    public bool DisplayMetamagicToggle { get; set; }
    public bool DisplayCraftingToggle { get; set; }
    public bool DisplayFactionRelationsToggle { get; set; }
    public bool DisplayItemsToggle { get; set; }
    public bool DisplayMerchantsToggle { get; set; }
    public SerializableDictionary<string, bool> DisplayKlassToggle { get; set; } = new();
    public SerializableDictionary<string, bool> DisplaySpellListsToggle { get; set; } = new();

    //
    // SETTINGS HIDDEN ON UI
    //

    public bool EnableCtrlClickOnlySwapsMainHand { get; set; } = true;
    public bool EnableDisplaySorceryPointBoxSorcererOnly { get; set; } = true;
    public bool EnableSameWidthFeatSelection { get; set; } = true;
    public bool EnableSameWidthInvocationSelection { get; set; } = true;
    public bool EnableSortingFightingStyles { get; set; } = true;
    public bool EnableSortingSubclasses { get; set; } = true;
    public bool EnableSortingFutureFeatures { get; set; } = true;
    public bool KeepCharactersPanelOpenAndHeroSelectedAfterLevelUp { get; set; } = true;

    // TA made level ups of more than 1 level at a time disallowing unlearning spells/invocations to streamline process
    public bool DisableStreamlinedMultiLevelUp { get; set; } = true;

    public HashSet<String> MonstersThatShouldHaveDarkvision { get; set; } =
    [
        "Adam_The_Twelth",
        "DLC3_Elven_07_Guard",
        "SRD_DLC_Mage",
        "SRD_Mage",
        "DLC1_NPC_Forge_Escorted_01",
        "DLC3_NPC_Generic_ElvenCitizen_Husk",
        "Generic_Darkweaver",
        "DLC3_ElvenClans_Leralyn",
        "DLC3_NPC_Elven3_DLC3_Ending",
        "DLC3_NPC_Elven5_DLC3_Ending",
        "Generic_HighPriest",
        "DLC3_Elven_Suspect_05_Guard_Traitor",
        "DLC3_Elven_06_Guard",
        "SRD_DLC3_Archmage",
        "Generic_ShockArcanist"
    ];

    public HashSet<String> MonstersThatShouldHaveTrueSight { get; set; } =
    [
        "Couatl",
        "CubeOfLight"
    ];

    public HashSet<String> MonstersThatShouldHaveBlindSight { get; set; } =
    [
        "Aksha",
        "Aksha_Legendary"
    ];

    public HashSet<String> MonstersThatShouldNotHaveTremorSense { get; set; } =
    [
        "Aksha",
        "Aksha_Legendary"
    ];

    public HashSet<String> EffectsThatTargetDistantIndividualsAndDontRequireSight { get; set; } =
    [
        "AcidSplash",
        "Aid",
        "AnimalShapes",
        "BeaconOfHope",
        "Bless",
        "BlessingOfRime",
        "CommandApproach",
        "CommandDrop",
        "CommandFlee",
        "CommandGrovel",
        "CommandHalt",
        "DispelMagic",
        "FeatherFall",
        "Knock",
        "Levitate",
        "MassCureWounds",
        "PassWithoutTrace",
        "RayOfEnfeeblement",
        "ResonatingStrike",
        "Sanctuary",
        "ShieldOfFaith",
        "Sparkle",
        "TrueStrike",
        "PowerBardHopeWordsOfHope6",
        "PowerBardTraditionManacalonsPerfection",
        "PowerCelestialSearingVengeance",
        "PowerCollegeOfValianceHeroicInspiration",
        "PowerDomainElementalHeraldOfTheElementsThunder",
        "PowerInnovationWeaponArcaneJolt",
        "PowerOathOfJugementPurgeCorruption",
        "PowerOathOfJugementRetribution",
        "PowerOathOfThunderThunderousRebuke",
        "PowerPatronFiendHurlThroughHell",
        "PowerRangerHellWalkerMarkOfTheDammed",
        "PowerRangerLightBearerBlessedWarrior",
        "PowerRiftWalkerRiftStrike",
        "PowerSorcerousPsionMindOverMatter",
        "PowerWayOfTheDistantHandZenArrowTechnique",
        "PowerWayOfTheDistantHandZenArrowUpgradedTechnique",
        "PowerFeatChefCookMeal"
    ];

    //
    // Gameplay - Tools
    //

    // General
    public bool DisableUnofficialTranslations { get; set; }
    public bool FixAsianLanguagesTextWrap { get; set; }
    public bool EnablePcgRandom { get; set; }
    public bool EnableSaveByLocation { get; set; }
    public bool EnableTogglesToOverwriteDefaultTestParty { get; set; }
    public List<string> DefaultPartyHeroes { get; } = [];
    public bool EnableCharacterChecker { get; set; }
    public bool EnableCheatMenu { get; set; }
    public bool EnableHotkeyDebugOverlay { get; set; }
    public bool NoExperienceOnLevelUp { get; set; }
    public bool OverrideMinMaxLevel { get; set; }
    public int MultiplyTheExperienceGainedBy { get; set; } = 100;
    public int OverridePartySize { get; set; } = ToolsContext.GamePartySize;
    public bool AllowAllPlayersOnNarrativeSequences { get; set; }
    public float FasterTimeModifier { get; set; } = ToolsDisplay.DefaultFastTimeModifier;
    public int EncounterPercentageChance { get; set; } = 5;

    //
    // Gameplay - General
    //

    // Creation
    public bool EnableFlexibleBackgrounds { get; set; }
    public bool DisableSenseDarkVisionFromAllRaces { get; set; }
    public bool DisableSenseSuperiorDarkVisionFromAllRaces { get; set; }
    public bool AddHelpActionToAllRaces { get; set; }
    public bool EnableAlternateHuman { get; set; }
    public bool EnableFlexibleRaces { get; set; }
    public bool EnableEpicPointsAndArray { get; set; }
    public bool ImproveLevelUpFeaturesSelection { get; set; }
    public int TotalFeatsGrantedFirstLevel { get; set; }
    public bool DisableLevelPrerequisitesOnModFeats { get; set; }
    public bool DisableRacePrerequisitesOnModFeats { get; set; }
    public bool DisableCastSpellPreRequisitesOnModFeats { get; set; }

    // Progression
    public bool EnableLevel20 { get; set; }
    public bool EnableMulticlass { get; set; }
    public int MaxAllowedClasses { get; set; }
    public bool EnableMinInOutAttributes { get; set; }
    public bool EnableRelearnSpells { get; set; }
    public bool DisplayAllKnownSpellsDuringLevelUp { get; set; }
    public bool DisplayPactSlotsOnSpellSelectionPanel { get; set; }
    public bool EnablesAsiAndFeat { get; set; }
    public bool EnableFeatsAtEveryFourLevels { get; set; }
    public bool EnableFeatsAtEveryFourLevelsMiddle { get; set; }
    public bool GrantScimitarSpecializationToBardRogue { get; set; }
    public bool EnableBarbarianBrutalStrike { get; set; }
    public bool DisableBarbarianBrutalCritical { get; set; }
    public bool EnableBarbarianFightingStyle { get; set; }
    public bool EnableBarbarianRecklessSameBuffDebuffDuration { get; set; }
    public bool EnableBarbarianRegainOneRageAtShortRest { get; set; }
    public bool AddFighterLevelToIndomitableSavingReroll { get; set; }
    public bool EnableFighterWeaponSpecialization { get; set; }
    public bool AddHumanoidFavoredEnemyToRanger { get; set; }
    public bool EnableRangerNatureShroudAt10 { get; set; }
    public bool EnableMonkAbundantKi { get; set; }
    public bool EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack { get; set; }
    public bool EnableMonkDoNotRequireAttackActionForFlurry { get; set; }
    public bool EnableMonkFightingStyle { get; set; }
    public bool EnableMonkImprovedUnarmoredMovementToMoveOnTheWall { get; set; }
    public bool EnableMonkWeaponSpecialization { get; set; }
    public bool EnableRogueCunningStrike { get; set; }
    public bool EnableRogueFightingStyle { get; set; }
    public bool EnableRogueSteadyAim { get; set; }

    // Visuals
    public bool OfferAdditionalLoreFriendlyNames { get; set; }
    public bool UnlockAllNpcFaces { get; set; }
    public bool AllowUnmarkedSorcerers { get; set; }
    public bool UnlockMarkAndTattoosForAllCharacters { get; set; }
    public bool UnlockEyeStyles { get; set; }
    public bool AddNewBrightEyeColors { get; set; }
    public bool UnlockGlowingEyeColors { get; set; }
    public bool UnlockGlowingColorsForAllMarksAndTattoos { get; set; }
    public bool UnlockSkinColors { get; set; }
    public bool AllowBeardlessDwarves { get; set; }

    //
    // Gameplay - Rules
    //

    // SRD
    public bool UseOfficialAdvantageDisadvantageRules { get; set; }
    public bool UseOfficialFlankingRules { get; set; }
    public bool UseMathFlankingRules { get; set; }
    public bool UseOfficialFlankingRulesButAddAttackModifier { get; set; }
    public bool UseOfficialFlankingRulesAlsoForRanged { get; set; }
    public bool UseOfficialFlankingRulesAlsoForReach { get; set; }
    public bool UseOfficialFoodRationsWeight { get; set; }
    public bool UseOfficialSmallRacesDisWithHeavyWeapons { get; set; }
    public bool UseOfficialLightingObscurementAndVisionRules { get; set; }
    public bool OfficialObscurementRulesCancelAdvDisPairs { get; set; }
    public bool OfficialObscurementRulesHeavilyObscuredAsProjectileBlocker { get; set; }
    public bool OfficialObscurementRulesMagicalDarknessAsProjectileBlocker { get; set; }
    public bool OfficialObscurementRulesTweakMonsters { get; set; }
    public bool KeepStealthOnHeroIfPerceivedDuringSurpriseAttack { get; set; }
    public bool StealthBreaksWhenAttackHits { get; set; }
    public bool StealthBreaksWhenAttackMisses { get; set; }
    public bool AddDexModifierToEnemiesInitiativeRoll { get; set; }
    public bool EnemiesAlwaysRollInitiative { get; set; }
    public bool DontEndTurnAfterReady { get; set; }
    public bool KeepInvisibilityWhenUsingItems { get; set; }
    public bool IllusionSpellsAutomaticallyFailAgainstTrueSightInRange { get; set; }
    public bool BlindedConditionDontAllowAttackOfOpportunity { get; set; }
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
    public bool FixRingOfRegenerationHealRate { get; set; }

    // House
    public bool AllowAnyClassToUseArcaneShieldstaff { get; set; }
    public bool IdentifyAfterRest { get; set; }
    public bool IncreaseMaxAttunedItems { get; set; }
    public bool RemoveAttunementRequirements { get; set; }
    public bool StealthBreaksWhenCastingMaterial { get; set; }
    public bool StealthBreaksWhenCastingVerbose { get; set; }
    public bool StealthBreaksWhenCastingSomatic { get; set; }
    public bool StealthDoesNotBreakWithSubtle { get; set; }
    public bool AllowHasteCasting { get; set; }
    public bool AllowStackedMaterialComponent { get; set; }
    public bool EnableCantripsTriggeringOnWarMagic { get; set; }
    public bool RemoveSchoolRestrictionsFromShadowCaster { get; set; }
    public bool RemoveSchoolRestrictionsFromSpellBlade { get; set; }
    public bool AllowAnyClassToWearSylvanArmor { get; set; }
    public bool AllowDruidToWearMetalArmor { get; set; }
    public bool AllowClubsToBeThrown { get; set; }
    public bool IgnoreHandXbowFreeHandRequirements { get; set; }
    public bool MakeAllMagicStaveArcaneFoci { get; set; }
    public bool ChangeDragonbornElementalBreathUsages { get; set; }
    public bool EnableRogueStrSaving { get; set; }
    public bool AccountForAllDiceOnSavageAttack { get; set; }
    public bool AllowFlightSuspend { get; set; }
    public bool FlightSuspendWingedBoots { get; set; }
    public bool EnableCharactersOnFireToEmitLight { get; set; }
    public bool EnableHigherGroundRules { get; set; }
    public bool FullyControlConjurations { get; set; }
    public bool ColdResistanceAlsoGrantsImmunityToChilledCondition { get; set; }
    public bool ColdImmunityAlsoGrantsImmunityToChilledAndFrozenCondition { get; set; }
    public bool AddDarknessPerceptiveToDarkRaces { get; set; }
    public bool RaceLightSensitivityApplyOutdoorsOnly { get; set; }
    public int SenseNormalVisionRangeMultiplier { get; set; } = 1;
    public int CriticalHitModeAllies { get; set; }
    public int CriticalHitModeEnemies { get; set; }
    public int CriticalHitModeNeutral { get; set; }

    //
    // Gameplay - Items, Crafting & Merchants
    //

    public bool AddNewWeaponsAndRecipesToShops { get; set; }
    public bool AddNewWeaponsAndRecipesToEditor { get; set; }
    public bool AddPickPocketableLoot { get; set; }
    public int RecipeCost { get; set; } = 200;
    public int TotalCraftingTimeModifier { get; set; }
    public int SetBeltOfDwarvenKindBeardChances { get; set; } = 50;
    public int EmpressGarbAppearanceIndex { get; set; }

    // Crafting
    public List<string> CraftingInStore { get; } = [];
    public List<string> CraftingItemsInDm { get; } = [];
    public List<string> CraftingRecipesInDm { get; } = [];

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
    // Characters - Races, Classes & Subclasses
    //

    public int RaceSliderPosition { get; set; } = ModUi.DontDisplayDescription;
    public List<string> RaceEnabled { get; } = [];
    public int SubraceSliderPosition { get; set; } = ModUi.DontDisplayDescription;
    public List<string> SubraceEnabled { get; } = [];
    public int BackgroundSliderPosition { get; set; } = ModUi.DontDisplayDescription;
    public List<string> BackgroundEnabled { get; } = [];
    public SerializableDictionary<string, int> KlassListSliderPosition { get; set; } = new();
    public SerializableDictionary<string, List<string>> KlassListSubclassEnabled { get; set; } = new();

    //
    // Characters - Feats, Groups, Fighting Styles, Invocations and Metamagic
    //

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

    public bool AllowDisplayingOfficialSpells { get; set; }
    public bool AllowDisplayingNonSuggestedSpells { get; set; }
    public SerializableDictionary<string, int> SpellListSliderPosition { get; set; } = new();
    public SerializableDictionary<string, List<string>> SpellListSpellEnabled { get; set; } = new();

    //
    // Interface - Game UI
    //

    // Campaigns and Locations
    public bool EnableAdditionalIconsOnLevelMap { get; set; }
    public bool EnableHeroWithBestProficiencyToRollChoice { get; set; }
    public bool EnableLogDialoguesToConsole { get; set; }
    public bool HideExitsAndTeleportersGizmosIfNotDiscovered { get; set; }
    public bool MarkInvisibleTeleportersOnLevelMap { get; set; }
    public bool EnableAlternateVotingSystem { get; set; }
    public bool EnableSumD20OnAlternateVotingSystem { get; set; }
    public bool AllowMoreRealStateOnRestPanel { get; set; }
    public bool AddPaladinSmiteToggle { get; set; }
    public bool EnableActionSwitching { get; set; }
    public bool EnableRespec { get; set; }
    public bool EnableStatsOnHeroTooltip { get; set; }
    public bool EnableCustomPortraits { get; set; }
    public bool ShowChannelDivinityOnPortrait { get; set; }
    public bool EnableAdditionalBackstoryDisplay { get; set; }
    public bool EnableExtendedProficienciesPanelDisplay { get; set; }

    // Battle
    public bool DontFollowCharacterInBattle { get; set; }
    public int DontFollowMargin { get; set; } = 5;
    public int GridSelectedColor { get; set; } = 1;
    public int MovementGridWidthModifier { get; set; } = 100;
    public int OutlineGridWidthModifier { get; set; } = 100;
    public int OutlineGridWidthSpeed { get; set; } = 100;

    public bool EnableDistanceOnTooltip { get; set; }
    public int HighContrastTargetingAoeSelectedColor { get; set; }
    public int HighContrastTargetingSingleSelectedColor { get; set; }

    // Formation
    public int FormationGridSelectedSet { get; set; } = -1;

    public int[][][] FormationGridSets { get; set; } =
    [
        [
            new int[GameUiContext.GridSize], new int[GameUiContext.GridSize], new int[GameUiContext.GridSize],
            new int[GameUiContext.GridSize], new int[GameUiContext.GridSize]
        ],
        [
            new int[GameUiContext.GridSize], new int[GameUiContext.GridSize], new int[GameUiContext.GridSize],
            new int[GameUiContext.GridSize], new int[GameUiContext.GridSize]
        ],
        [
            new int[GameUiContext.GridSize], new int[GameUiContext.GridSize], new int[GameUiContext.GridSize],
            new int[GameUiContext.GridSize], new int[GameUiContext.GridSize]
        ],
        [
            new int[GameUiContext.GridSize], new int[GameUiContext.GridSize], new int[GameUiContext.GridSize],
            new int[GameUiContext.GridSize], new int[GameUiContext.GridSize]
        ],
        [
            new int[GameUiContext.GridSize], new int[GameUiContext.GridSize], new int[GameUiContext.GridSize],
            new int[GameUiContext.GridSize], new int[GameUiContext.GridSize]
        ]
    ];

    // Input
    public bool AltOnlyHighlightItemsInPartyFieldOfView { get; set; }
    public bool InvertAltBehaviorOnTooltips { get; set; }
    public bool EnableHotkeySwapFormationSets { get; set; }
    public bool EnableHotkeyToggleHud { get; set; }
    public bool EnableCharacterExport { get; set; }
    public bool EnableTeleportParty { get; set; }
    public bool EnableRejoinParty { get; set; }
    public bool EnableVttCamera { get; set; }
    public bool EnableCancelEditOnRightMouseClick { get; set; }

    // Inventory and Items
    public bool AddCustomIconsToOfficialItems { get; set; }
    public bool DisableAutoEquip { get; set; }
    public bool EnableInventoryFilteringAndSorting { get; set; }
    public bool EnableInventoryTaintNonProficientItemsRed { get; set; }
    public bool EnableInventoryTintKnownRecipesRed { get; set; }
    public bool EnableInvisibleCrownOfTheMagister { get; set; }
    public bool DontDisplayHelmets { get; set; }
    public bool ShowCraftingRecipeInDetailedTooltips { get; set; }
    public bool ShowCraftedItemOnRecipeIcon { get; set; }
    public bool SwapCraftedItemAndRecipeIcons { get; set; }

    // Monsters
    public bool HideMonsterHitPoints { get; set; }
    public bool RemoveBugVisualModels { get; set; }
    public bool ShowButtonWithControlledMonsterInfo { get; set; }

    //
    // Interface - Dungeon Maker
    //

    public bool EnableLoggingInvalidReferencesInUserCampaigns { get; set; }
    public bool EnableSortingDungeonMakerAssets { get; set; }
    public bool AllowGadgetsAndPropsToBePlacedAnywhere { get; set; }
    public bool UnleashNpcAsEnemy { get; set; }
    public bool UnleashEnemyAsNpc { get; set; }
    public bool EnableDungeonMakerModdedContent { get; set; }

    //
    // Interface - Translations
    //

    public string SelectedLanguageCode { get; set; } = TranslatorContext.English;

    //
    // Encounters - General
    //

    public bool EnableEnemiesControlledByPlayer { get; set; }
    public bool EnableHeroesControlledByComputer { get; set; }

    // Debug
    public bool DebugDisableVerifyDefinitionNameIsNotInUse { get; set; }
#if DEBUG
    public bool DebugLogDefinitionCreation { get; set; }
    public bool DebugLogFieldInitialization { get; set; }
    public bool DebugLogVariantMisuse { get; set; }
#endif
}
