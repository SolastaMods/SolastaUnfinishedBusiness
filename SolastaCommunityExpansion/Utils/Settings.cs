using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using ModKit.Utility;
using SolastaCommunityExpansion.Models;
using UnityModManagerNet;

namespace SolastaCommunityExpansion.Utils;

public class Core
{
}

[Serializable]
[XmlRoot(ElementName = "Settings")]
public class Settings : UnityModManager.ModSettings
{
    //
    // Welcome Message
    //

    public bool DisplayWelcomeMessage { get; set; } = true;
    public bool EnableBetaContent { get; set; }

    //
    // Blueprints Viewer UI
    //

    // must be set to zero or won't compile
    internal int SelectedRawDataType;
    internal int MaxRows = 20;
    internal int MaxSearchDepth = 3;

    //
    // SETTINGS UI TOGGLES
    //

    public bool DisplayGeneralRaceClassSubClassToggle { get; set; } = true;
    public bool DisplayRacesToggle { get; set; } = true;
    public bool DisplayClassesToggle { get; set; } = true;
    public bool DisplaySubclassesToggle { get; set; } = true;
    public bool DisplayFeatsToggle { get; set; } = true;
    public bool DisplayFightingStylesToggle { get; set; } = true;
    public SerializableDictionary<string, bool> DisplaySpellListsToggle { get; set; } = new();
    public bool DisplayCraftingToggle { get; set; }
    public bool DisplayMerchantsToggle { get; set; } = true;

    //
    // SETTINGS HIDDEN ON UI
    //

    public bool EnableDungeonMakerPro { get; set; } = true;
    public bool EnableMoveSorceryPointsBox { get; set; } = true;
    public bool EnableMultiLinePowerPanel { get; set; } = true;
    public bool EnableMultiLineSpellPanel { get; set; } = true;
    public bool EnableSameWidthFeatSelection { get; set; } = true;
    public bool EnableSortingClasses { get; set; } = true;
    public bool EnableSortingDeities { get; set; } = true;
    public bool EnableSortingDungeonMakerAssets { get; set; } = true;
    public bool EnableSortingFeats { get; set; } = true;
    public bool EnableSortingFightingStyles { get; set; } = true;
    public bool EnableSortingFutureFeatures { get; set; } = true;
    public bool EnableSortingRaces { get; set; } = true;
    public bool EnableSortingSubclasses { get; set; } = true;
    public bool EnableEnhancedCharacterInspection { get; set; } = true;
    public bool KeepCharactersPanelOpenAndHeroSelectedOnLevelUp { get; set; } = true;
    public bool KeepSpellsOpenSwitchingEquipment { get; set; } = true;

    //
    // Character - General
    //

    // Initial Choices
    public bool AddHelpActionToAllRaces { get; set; }
    public bool DisableSenseDarkVisionFromAllRaces { get; set; }
    public bool DisableSenseSuperiorDarkVisionFromAllRaces { get; set; }
    public bool EnableAlternateHuman { get; set; }
    public bool EnableFlexibleBackgrounds { get; set; }
    public bool EnableFlexibleRaces { get; set; }
    public bool EnableEpicPointsAndArray { get; set; }
    public int TotalFeatsGrantedFistLevel { get; set; }

    // Progression
    public bool EnablesAsiAndFeat { get; set; }
    public bool EnableFeatsAtEvenLevels { get; set; }
    public bool EnableLevel20 { get; set; }

    // Multiclass
    public bool EnableMinInOutAttributes { get; set; } = true;
    public bool EnableRelearnSpells { get; set; } = true;
    public bool DisplayAllKnownSpellsDuringLevelUp { get; set; } = true;
    public int MaxAllowedClasses { get; set; } = 1;

    // Visuals
    public bool OfferAdditionalLoreFriendlyNames { get; set; }
    public bool UnlockAllNpcFaces { get; set; }
    public bool AllowUnmarkedSorcerers { get; set; }
    public bool UnlockMarkAndTatoosForAllCharacters { get; set; }
    public bool UnlockEyeStyles { get; set; }
    public bool UnlockGlowingEyeColors { get; set; }
    public bool UnlockGlowingColorsForAllMarksAndTatoos { get; set; }

    //
    // Characters - Races, Classes & Subclasses
    //

    public bool EnableUnlimitedArcaneRecoveryOnWizardSpellMaster { get; set; }
    public bool EnableShortRestRechargeOfArcaneWeaponOnWizardArcaneFighter { get; set; }
    public int OverrideRogueConArtistImprovedManipulationSpellDc { get; set; } = 3;
    public int OverrideWizardMasterManipulatorArcaneManipulationSpellDc { get; set; } = 2;
    public int RaceSliderPosition { get; set; } = 4;
    public List<string> RaceEnabled { get; } = new();
    public int ClassSliderPosition { get; set; } = 4;
    public List<string> ClassEnabled { get; } = new();
    public int SubclassSliderPosition { get; set; } = 4;
    public List<string> SubclassEnabled { get; } = new();

    //
    // Characters - Feats
    //

    public int FeatSliderPosition { get; set; } = 4;
    public List<string> FeatEnabled { get; } = new();

    //
    // Characters - Fighting Styles
    //

    public int FightingStyleSliderPosition { get; set; } = 4;
    public List<string> FightingStyleEnabled { get; } = new();

    //
    // Characters - Spells
    //

    public SerializableDictionary<string, int> SpellListSliderPosition { get; set; } = new();
    public SerializableDictionary<string, List<string>> SpellListSpellEnabled { get; set; } = new();

    //
    // Gameplay - Rules
    //

    // SRD
    public bool UseOfficialAdvantageDisadvantageRules { get; set; }
    public bool AllowCrossbowsToUseBowFeatures { get; set; }
    public bool AddBleedingToLesserRestoration { get; set; }
    public bool BlindedConditionDontAllowAttackOfOpportunity { get; set; }
    public bool AllowTargetingSelectionWhenCastingChainLightningSpell { get; set; }
    public bool BestowCurseNoConcentrationRequiredForSlotLevel5OrAbove { get; set; }
    public bool EnableUpcastConjureElementalAndFey { get; set; }
    public bool OnlyShowMostPowerfulUpcastConjuredElementalOrFey { get; set; }
    public bool FixSorcererTwinnedLogic { get; set; }
    public bool FullyControlConjurations { get; set; }

    // House
    public bool ChangeSleetStormToCube { get; set; }
    public bool UseHeightOneCylinderEffect { get; set; }
    public bool RemoveConcentrationRequirementsFromAnySpell { get; set; }
    public bool RemoveHumanoidFilterOnHideousLaughter { get; set; }
    public bool RemoveRecurringEffectOnEntangle { get; set; }
    public bool AllowAnyClassToWearSylvanArmor { get; set; }
    public bool AllowDruidToWearMetalArmor { get; set; }
    public bool DisableAutoEquip { get; set; }
    public bool MakeAllMagicStaveArcaneFoci { get; set; }
    public int IncreaseSenseNormalVision { get; set; } = HouseFeatureContext.DEFAULT_VISION_RANGE;

    public bool QuickCastLightCantripOnWornItemsFirst { get; set; }

    // public bool UseHeightOneCylinderEffect { get; set; }
    public bool AddPickpocketableLoot { get; set; }
    public bool AllowStackedMaterialComponent { get; set; }
    public bool ScaleMerchantPricesCorrectly { get; set; }

    //
    // Gameplay - Items, Crafting & Merchants
    //

    // General
#if DEBUG
    public bool AddNewWeaponsAndRecipesToShops { get; set; } = true; // simplifies diags. creation (one less boot)
    public bool AddNewWeaponsAndRecipesToEditor { get; set; } = true;
#else
    public bool AddNewWeaponsAndRecipesToShops { get; set; }
    public bool AddNewWeaponsAndRecipesToEditor { get; set; }
#endif
    public bool RemoveAttunementRequirements { get; set; }
    public bool RemoveIdentifcationRequirements { get; set; }
    public bool ShowCraftingRecipeInDetailedTooltips { get; set; }
    public int TotalCraftingTimeModifier { get; set; }
    public int RecipeCost { get; set; } = 200;
    public int SetBeltOfDwarvenKindBeardChances { get; set; } = 50;

    // Crafting
    public List<string> CraftingInStore { get; } = new();
    public List<string> CraftingItemsInDM { get; } = new();
    public List<string> CraftingRecipesInDM { get; } = new();

    // Merchants
    public bool StockGorimStoreWithAllNonMagicalClothing { get; set; }
    public bool StockHugoStoreWithAdditionalFoci { get; set; }
    public bool EnableAdditionalFociInDungeonMaker { get; set; }
    public bool RestockAntiquarians { get; set; }
    public bool RestockArcaneum { get; set; }
    public bool RestockCircleOfDanantar { get; set; }
    public bool RestockTowerOfKnowledge { get; set; }

    //
    // Gameplay - Tools
    //

    // General
    public bool EnableSaveByLocation { get; set; }
    public bool EnableCharacterChecker { get; set; }
    public bool EnableRespec { get; set; }
    public bool EnableCheatMenu { get; set; }
    public bool OverrideMinMaxLevel { get; set; }
    public bool EnableTogglesToOverwriteDefaultTestParty { get; set; }
    public List<string> DefaultPartyHeroes = new();
    public bool NoExperienceOnLevelUp { get; set; }
    public int OverridePartySize { get; set; } = DungeonMakerContext.GAME_PARTY_SIZE;
    public int MultiplyTheExperienceGainedBy { get; set; } = 100;
    public int MaxBackupFilesPerLocationCampaign { get; set; } = 10;

    // Debug
    public bool DebugLogDefinitionCreation { get; set; }
    public bool DebugLogFieldInitialization { get; set; }
    public bool DebugDisableVerifyDefinitionNameIsNotInUse { get; set; }
#if DEBUG
    public bool DebugLogVariantMisuse { get; set; }
#endif

    // Faction Relations

    //
    // Interface - Dungeon Maker
    //

    public bool AllowDungeonsMaxLevel20 { get; set; }
    public bool AllowGadgetsAndPropsToBePlacedAnywhere { get; set; }
    public bool UnleashNpcAsEnemy { get; set; }
    public bool UnleashEnemyAsNpc { get; set; }
    public bool EnableDungeonMakerModdedContent { get; set; }
#if DEBUG
    public bool EnableExtraHighLevelMonsters { get; set; } = true; // simplifies diags. creation (one less boot)
#else
    public bool EnableExtraHighLevelMonsters { get; set; }
#endif

    //
    // Interface - Game UI
    //

    // Battle
    public bool DontFollowCharacterInBattle { get; set; }
    public int DontFollowMargin { get; set; } = 5;
    public bool AutoPauseOnVictory { get; set; }
    public float FasterTimeModifier { get; set; } = 1.5f;

    // Campaigns and Locations
    public bool FollowCharactersOnTeleport { get; set; }
    public bool EnableLogDialoguesToConsole { get; set; }
    public bool EnableAdditionalIconsOnLevelMap { get; set; }
    public bool MarkInvisibleTeleportersOnLevelMap { get; set; }
    public bool HideExitAndTeleporterGizmosIfNotDiscovered { get; set; }

    // Inventory and Items
    public bool EnableInventoryFilteringAndSorting { get; set; }
    public bool EnableInventoryTaintNonProficientItemsRed { get; set; }
    public bool EnableInvisibleCrownOfTheMagister { get; set; }
    public string EmpressGarbAppearance { get; set; } = "Normal";

    // Monsters
    public bool HideMonsterHitPoints { get; set; }
    public bool RemoveBugVisualModels { get; set; }

    // Spells
    public int MaxSpellLevelsPerLine { get; set; } = 4;

    //
    // Interface - Keyboard & Mouse
    //

    public bool EnableCancelEditOnRightMouseClick { get; set; }
    public bool EnableHotkeyToggleHud { get; set; }
    public bool EnableHotkeyToggleIndividualHud { get; set; }
    public bool EnableCharacterExport { get; set; }
    public bool EnableHotkeyDebugOverlay { get; set; }
    public bool EnableHotkeyZoomCamera { get; set; }
    public bool EnableTeleportParty { get; set; }
    public bool AltOnlyHighlightItemsInPartyFieldOfView { get; set; }
    public bool InvertAltBehaviorOnTooltips { get; set; }
    public bool EnableCtrlClickBypassMetamagicPanel { get; set; }
    public bool EnableCtrlClickBypassAttackReactionPanel { get; set; }
    public bool EnableIgnoreCtrlClickOnCriticalHit { get; set; }
    public bool EnableCtrlClickOnlySwapsMainHand { get; set; }

    //
    // Interface - Translations
    //

    public bool EnableOnTheFlyTranslations { get; set; }
    public string SelectedLanguageCode { get; set; } = "en";
    public string SelectedOverwriteLanguageCode { get; set; } = "off";
    public Translations.Engine TranslationEngine { get; set; } = Translations.Engine.Google;

    //
    // Encounters - General
    //

    public bool EnableEnemiesControlledByPlayer { get; set; }
    public bool EnableHeroesControlledByComputer { get; set; }
}
