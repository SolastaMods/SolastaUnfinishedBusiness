using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityModManagerNet;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace SolastaCommunityExpansion
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class Core
    {
    }

    [Serializable]
    [XmlRoot(ElementName = "Settings")]
    public class Settings : UnityModManager.ModSettings
    {
        public const string GUID = "b1ffaca74824486ea74a68d45e6b1925";

        //
        // Blueprints Viewer UI
        //

        internal int SelectedRawDataType = 0;
        internal int MaxRows = 20;
        internal int MaxSearchDepth = 3;

        //
        // SETTINGS UI TOGGLES
        //

        public bool DisplayInitialChoicesToggle { get; set; }
        public bool DisplayMiscellaneousToggle { get; set; }
        public bool DisplayProgressionToggle { get; set; }
        public bool DisplayFaceUnlockSettings { get; set; }
        public bool DisplayClassesToggle { get; set; }
        public bool DisplaySubclassesToggle { get; set; }
        public bool DisplayFeatsToggle { get; set; }
        public bool DisplayFightingStylesToggle { get; set; }
        public bool DisplaySrdRulesToggle { get; set; }
        public bool DisplayHouseRulesToggle { get; set; }
        public bool DisplayAdventureLogToggle { get; set; }
        public bool DisplayBattleToggle { get; set; }
        public bool DisplayCampaignToggle { get; set; }
        public bool DisplayItemToggle { get; set; }
        public bool DisplayMonsterToggle { get; set; }
        public bool DisplaySpellToggle { get; set; }
        public bool DisplayCraftingToggle { get; set; }
        public bool DisplayMerchantsToggle { get; set; }

        //
        // SETTINGS HIDDEN ON UI
        //

        public bool AllowDynamicPowers { get; set; } = true;
        public bool BugFixBestiarySorting { get; set; } = true;
        public bool BugFixButtonActivatorTriggerIssue { get; set; } = true;
        public bool BugFixCharacterPanelSorting { get; set; } = true;
        public bool BugFixExpandColorTables { get; set; } = true;
        public bool BugFixNullRecipesOnGameSerialization { get; set; } = true;
        public bool EnableCancelEditOnRightMouseClick { get; set; } = true;
        public bool EnableDungeonMakerPro { get; set; } = true;
        public bool EnableDungeonMakerRotationHotkeys { get; set; } = true;
        public bool EnableFirstLevelCasterFeats { get; set; } = true;
        public bool EnableMultiLinePowerPanel { get; set; } = true;
        public bool EnableMultiLineSpellPanel { get; set; } = true;
        public bool EnableSortingClasses { get; set; } = true;
        public bool EnableSortingDeities { get; set; } = true;
        public bool EnableSortingDungeonMakerAssets { get; set; } = true;
        public bool EnableSortingFeats { get; set; } = true;
        public bool EnableSortingFutureFeatures { get; set; } = true;
        public bool EnableSortingRaces { get; set; } = true;
        public bool EnableSortingSubclasses { get; set; } = true;
        public bool KeepCharactersPanelOpenAndHeroSelectedOnLevelUp { get; set; } = true;
        public bool KeepSpellsOpenSwitchingEquipment { get; set; } = true;
        public bool ShowAllAutoPreparedSpells { get; set; } = true;
        public bool SupportAutoPreparedSpellsOnSubclassCasters { get; set; } = true;

        //
        // Character - General
        //

        // Initial Choices
        public bool AllowDisplayAllUnofficialContent { get; set; }
        public bool AddHelpActionToAllClasses { get; set; }
        public bool DisableSenseDarkVisionFromAllRaces { get; set; }
        public bool DisableSenseSuperiorDarkVisionFromAllRaces { get; set; }
        public bool EnableAlternateHuman { get; set; }
        public bool EnableFlexibleBackgrounds { get; set; }
        public bool EnableFlexibleRaces { get; set; }
        public bool EnableEpicPoints { get; set; }
        public bool EnableEpicArray { get; set; }
        public int TotalFeatsGrantedFistLevel { get; set; }

        // Miscellaneous
        public bool AllowExtraKeyboardCharactersInAllNames { get; set; }
        public bool OfferAdditionalLoreFriendlyNames { get; set; }

        // Progression
        public bool EnablesAsiAndFeat { get; set; }
        public bool EnableLevel20 { get; set; }
        public bool EnableRespec { get; set; }

        // Visuals
        public bool UnlockAllNpcFaces { get; set; }
        public bool AllowUnmarkedSorcerers { get; set; }
        public bool UnlockMarkAndTatoosForAllCharacters { get; set; }
        public bool UnlockEyeStyles { get; set; }
        public bool UnlockGlowingEyeColors { get; set; }
        public bool UnlockGlowingColorsForAllMarksAndTatoos { get; set; }

        //
        // Characters - Classes & Subclasses
        //

        public bool EnableUnlimitedArcaneRecoveryOnWizardSpellMaster { get; set; }
        public bool EnableShortRestRechargeOfArcaneWeaponOnWizardArcaneFighter { get; set; }
        public int OverrideRogueConArtistImprovedManipulationSpellDc { get; set; } = 3;
        public int OverrideWizardMasterManipulatorArcaneManipulationSpellDc { get; set; } = 2;
        public int ClassSliderPosition { get; set; } = 1;
        public List<string> ClassEnabled { get; } = new List<string>();
        public int SubclassSliderPosition { get; set; } = 1;
        public List<string> SubclassEnabled { get; } = new List<string>();

        //
        // Characters - Feats
        //

        public int FeatPowerAttackModifier { get; set; } = 3;
        public int FeatSliderPosition { get; set; } = 1;
        public List<string> FeatEnabled { get; } = new List<string>();

        //
        // Characters - Fighting Styles
        //

        public int FightingStyleSliderPosition { get; set; } = 1;
        public List<string> FightingStyleEnabled { get; } = new List<string>();

        //
        // Characters - Powers
        //

        public List<string> PowerEnabled { get; } = new List<string>();

        //
        // Characters - Spells
        //

        public Utils.SerializableDictionary<string, List<string>> SpellSpellListEnabled { get; set; } = new Utils.SerializableDictionary<string, List<string>>();

        //
        // Encounters - General
        //

        public bool EnableEnemiesControlledByPlayer { get; set; }
        public bool EnableHeroesControlledByComputer { get; set; }

        //
        // Gameplay - Rules
        //

        // SRD
        public bool UseOfficialAdvantageDisadvantageRules { get; set; }
        public bool UseOfficialCombatSurpriseRules { get; set; }
        public bool RollDifferentStealthChecksForEachCharacterPair { get; set; }
        public bool EnablePowerAid { get; set; }
        public bool AddBleedingToLesserRestoration { get; set; }
        public bool BugFixCalmEmotionsOnAlly { get; set; } = true;
        public bool BlindedConditionDontAllowAttackOfOpportunity { get; set; }
        public bool AllowTargetingSelectionWhenCastingChainLightningSpell { get; set; }
        public bool FixSorcererTwinnedLogic { get; set; }
        public bool EnableUpcastConjureElementalAndFey { get; set; }
        public bool FullyControlConjurations { get; set; }
        public bool DismissControlledConjurationsWhenDeliberatelyDropConcentration { get; set; }
        public bool OnlyShowMostPowerfulUpcastConjuredElementalOrFey { get; set; }
        public bool BugFixWandOfFear { get; set; } = true;

        // House
        public bool AllowAnyClassToWearSylvanArmor { get; set; }
        public bool AllowDruidToWearMetalArmor { get; set; }
        public bool DisableAutoEquip { get; set; }
        public bool MakeAllMagicStaveArcaneFoci { get; set; }
        public bool IncreaseSenseNormalVision { get; set; }
        public bool AddPickpocketableLoot { get; set; }
        public bool AllowStackedMaterialComponent { get; set; }
        public bool ScaleMerchantPricesCorrectly { get; set; }
        public bool QuickCastLightCantripOnWornItemsFirst { get; set; }
        public int MultiplyTheExperienceGainedBy { get; set; } = 100;

        //
        // Gameplay - Campaigns and Locations
        //

        public bool EnableSaveByLocation { get; set; }
        public bool EnableTelemaCampaign { get; set; }
        public bool EnableTeleportParty { get; set; }
        public bool FollowCharactersOnTeleport { get; set; }
        public bool OverrideMinMaxLevel { get; set; }
        public int OverridePartySize { get; set; } = Models.DungeonMakerContext.GAME_PARTY_SIZE;
        public int MaxBackupFilesPerLocationCampaign { get; set; } = 10;

        //
        // Gameplay - Items, Crafting & Merchants
        //

        // General
        public bool RemoveAttunementRequirements { get; set; }
        public bool RemoveIdentifcationRequirements { get; set; }
        public bool ShowCraftingRecipeInDetailedTooltips { get; set; }
        public int RecipeCost { get; set; } = 200;

        public int SetBeltOfDwarvenKindBeardChances { get; set; } = 50;

        // Crafting
        public List<string> CraftingInStore { get; } = new List<string>();
        public List<string> CraftingItemsInDM { get; } = new List<string>();
        public List<string> CraftingRecipesInDM { get; } = new List<string>();

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

        // Debug
        public bool EnableBlueprintsPanel { get; set; }
        public bool EnablePatchesPanel { get; set; }
        public bool EnableCheatMenu { get; set; }
        public bool NoExperienceOnLevelUp { get; set; }

        public bool ShowTADefinitionsWithMissingGuiPresentation { get; set; }
        public bool ShowCEDefinitionsWithMissingGuiPresentation { get; set; } = true;

        // Faction Relations

        //
        // Interface - Dungeon Maker
        //

        public bool AllowDungeonsMaxLevel20 { get; set; }
        public bool AllowGadgetsToBePlacedAnywhere { get; set; }
        public bool AllowPropsToBePlacedAnywhere { get; set; }
        public bool UnleashNpcAsEnemy { get; set; }
        public bool UnleashEnemyAsNpc { get; set; }
        public bool EnableDungeonMakerModdedContent { get; set; }

        //
        // Interface - Game UI
        //

        // Adventure Log
        public bool EnableAdventureLogBanterLines { get; set; }
        public bool EnableAdventureLogDocuments { get; set; }
        public bool EnableAdventureLogTextFeedback { get; set; }
        public bool EnableAdventureLogPopups { get; set; }

        // Battle
        public bool DontFollowCharacterInBattle { get; set; }
        public int DontFollowMargin { get; set; } = 5;
        public bool AutoPauseOnVictory { get; set; }

        // Campaigns and Locations
        public bool EnableAdditionalIconsOnLevelMap { get; set; }
        public bool MarkInvisibleTeleportersOnLevelMap { get; set; }
        public bool HideExitAndTeleporterGizmosIfNotDiscovered { get; set; }

        // Inventory and Items
        public bool EnableInventoryFilteringAndSorting { get; set; } = true;
        public bool EnableInvisibleCrownOfTheMagister { get; set; }
        public string EmpressGarbAppearance { get; set; } = "Normal";

        // Monsters
        public bool HideMonsterHitPoints { get; set; }
        public bool RemoveBugVisualModels { get; set; }

        // Spells
        public int MaxSpellLevelsPerLine { get; set; } = 5;
        public float SpellPanelGapBetweenLines { get; set; } = 30f;

        //
        // Interface - Hotkeys
        //

        public bool EnableCharacterExport { get; set; }
        public bool EnableHotkeyDebugOverlay { get; set; }
        public bool EnableHotkeyZoomCamera { get; set; }
        public bool EnableHotkeyToggleHud { get; set; }
        public bool EnableHotkeyToggleIndividualHud { get; set; }
        public bool AltOnlyHighlightItemsInPartyFieldOfView { get; set; }
        public bool InvertAltBehaviorOnTooltips { get; set; }
    }
}
