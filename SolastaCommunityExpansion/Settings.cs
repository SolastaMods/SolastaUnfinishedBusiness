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
        public bool DisplayAdventureLogToggle { get; set; }
        public bool DisplayBattleToggle { get; set; }
        public bool DisplayDungeonMakerToggle { get; set; }
        public bool DisplayItemToggle { get; set; }
        public bool DisplayHotkeysToggle { get; set; }
        public bool DisplayMonsterToggle { get; set; }
        public bool DisplaySpellToggle { get; set; }
        public bool DisplayCraftingToggle { get; set; }
        public bool DisplayMerchantsToggle { get; set; }
        public bool DisplayCampaignsAndLocationsToggle { get; set; }
        public bool DisplayDebugToggle { get; set; }

        //
        // SETTINGS HIDDEN ON UI
        //

        public bool AllowDynamicPowers { get; set; } = true;
        public bool AllowExtraKeyboardCharactersInCampaignNames { get; set; } = true;
        public bool AllowExtraKeyboardCharactersInLocationNames { get; set; } = true;
        public bool BugFixAttunementUnknownCharacter { get; set; } = true;
        public bool BugFixBestiarySorting { get; set; } = true;
        public bool BugFixButtonActivatorTriggerIssue { get; set; } = true;
        public bool BugFixCharacterPanelSorting { get; set; } = true;
        public bool BugFixExpandColorTables { get; set; } = true;
        public bool BugFixGameGadgetCheckIsEnabled { get; set; } = true;
        public bool BugFixItemFiltering { get; set; } = true;
        public bool BugFixNullRecipesOnGameSerialization { get; set; } = true;
        public bool BugFixOnCanSaveToggleChanged { get; set; } = true;
        public bool EnableCancelEditOnRightMouseClick { get; set; } = true;
        public bool EnableDungeonMakerRotationHotkeys { get; set; } = true;
        public bool EnableFirstLevelCasterFeats { get; set; } = true;
        public bool EnableMultiLinePowerPanel { get; set; } = true;
        public bool EnableMultiLineSpellPanel { get; set; } = true;
        public bool EnableSortingClasses { get; set; } = true;
        public bool EnableSortingDeities { get; set; } = true;
        public bool EnableSortingFeats { get; set; } = true;
        public bool EnableSortingFutureFeatures { get; set; } = true;
        public bool EnableSortingRaces { get; set; } = true;
        public bool EnableSortingSubclasses { get; set; } = true;
        public bool KeepCharactersPanelOpenAndHeroSelectedOnLevelUp { get; set; } = true;
        public bool KeepSpellsOpenSwitchingEquipment { get; set; } = true;
        public bool ShowAllAutoPreparedSpells { get; set; } = true;

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
        public bool AllowExtraKeyboardCharactersInNames { get; set; }
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
        public List<string> ClassEnabled { get; private set; } = new List<string>();
        public int SubclassSliderPosition { get; set; } = 1;
        public List<string> SubclassEnabled { get; private set; } = new List<string>();

        //
        // Characters - Feats
        //

        public int FeatPowerAttackModifier { get; set; } = 3;
        public int FeatSliderPosition { get; set; } = 1;
        public List<string> FeatEnabled { get; private set; } = new List<string>();

        //
        // Characters - Fighting Styles
        //

        public int FightingStyleSliderPosition { get; set; } = 1;
        public List<string> FightingStyleEnabled { get; private set; } = new List<string>();

        //
        // Characters - Powers
        //

        public List<string> PowerEnabled { get; private set; } = new List<string>();

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
        public bool AllowTargetingSelectionWhenCastingChainLightningSpell { get; set; }
        public bool BlindedConditionDontAllowAttackOfOpportunity { get; set; }
        public bool FullyControlConjurations { get; set; }
        public bool DismissControlledConjurationsWhenDeliberatelyDropConcentration { get; set; }
        public bool FixSorcererTwinnedLogic { get; set; }

        // House
        public bool AllowAnyClassToWearSylvanArmor { get; set; }
        public bool AllowDruidToWearMetalArmor { get; set; }
        public bool DisableAutoEquip { get; set; }
        public bool MakeAllMagicStaveArcaneFoci { get; set; }
        public bool IncreaseSenseNormalVision { get; set; }
        public bool AddPickpocketableLoot { get; set; }
        public bool AllowStackedMaterialComponent { get; set; }
        public bool ScaleMerchantPricesCorrectly { get; set; }

        public bool AddBleedingToLesserRestoration { get; set; }
        public bool QuickCastLightCantripOnWornItemsFirst { get; set; }


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
        public List<string> CraftingInStore { get; private set; } = new List<string>();
        public List<string> CraftingItemsInDM { get; private set; } = new List<string>();
        public List<string> CraftingRecipesInDM { get; private set; } = new List<string>();

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

        // Campaigns and Locations
        public bool AltOnlyHighlightItemsInPartyFieldOfView { get; set; }
        public bool EnableAdditionalIconsOnLevelMap { get; set; }
        public bool MarkInvisibleTeleportersOnLevelMap { get; set; }
        public bool HideExitAndTeleporterGizmosIfNotDiscovered { get; set; }
        public bool EnableSaveByLocation { get; set; }
        public bool EnableTelemaCampaign { get; set; }
        public bool EnableTeleportParty { get; set; }
        public bool FollowCharactersOnTeleport { get; set; }
        public bool OverrideMinMaxLevel { get; set; }
        public int OverridePartySize { get; set; } = Models.DungeonMakerContext.GAME_PARTY_SIZE;
        public int MaxBackupFilesPerLocationCampaign { get; set; } = 10;

        // Debug
        public bool EnableCheatMenu { get; set; }
        public bool EnableDebugOverlay { get; set; }

        // Experience
        public bool NoExperienceOnLevelUp { get; set; }
        public int MultiplyTheExperienceGainedBy { get; set; } = 100;

        // Faction Relations

        //
        // Game UI
        //

        // Adventure Log
        public bool EnableAdventureLogBanterLines { get; set; }
        public bool EnableAdventureLogDocuments { get; set; }
        public bool EnableAdventureLogLore { get; set; }
        public bool EnableAdventureLogTextFeedback { get; set; }
        public bool EnableAdventureLogPopups { get; set; }

        // Battle
        public bool DontFollowCharacterInBattle { get; set; }
        public int DontFollowMargin { get; set; } = 5;
        public bool AutoPauseOnVictory { get; set; }
        public bool PermanentlySpeedBattleUp { get; set; }
        public float BattleCustomTimeScale { get; set; } = 1.0f;

        // Dungeon Maker
        public bool AllowGadgetsToBePlacedAnywhere { get; set; }
        public bool AllowPropsToBePlacedAnywhere { get; set; }
        public bool UnleashNpcAsEnemy { get; set; }
        public bool UnleashEnemyAsNpc { get; set; }

        // Inventory and Items
        public bool EnableInventoryFilteringAndSorting { get; set; } = true;
        public bool EnableInvisibleCrownOfTheMagister { get; set; }
        public string EmpressGarbAppearance { get; set; } = "Normal";

        // Hotkeys
        public bool EnableCharacterExport { get; set; }
        public bool EnableHotkeysToToggleHud { get; set; }
        public bool InvertAltBehaviorOnTooltips { get; set; }

        // Monsters
        public bool HideMonsterHitPoints { get; set; }
        public bool RemoveBugVisualModels { get; set; }

        // Spells
        public int MaxSpellLevelsPerLine { get; set; } = 5;
        public float SpellPanelGapBetweenLines { get; set; } = 30f;
    }
}
