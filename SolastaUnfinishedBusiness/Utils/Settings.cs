using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Models;
using UnityModManagerNet;

namespace SolastaUnfinishedBusiness.Utils;

internal sealed class Core
{
}

[Serializable]
[XmlRoot(ElementName = "Settings")]
internal class Settings : UnityModManager.ModSettings
{
    //
    // Welcome Message
    //

    internal bool DisplayWelcomeMessage { get; set; } = true;
    internal int EnableDiagsDump { get; set; }

    //
    // SETTINGS UI TOGGLES
    //

    internal bool DisplayGeneralRaceClassSubClassToggle { get; set; } = true;
    internal bool DisplayRacesToggle { get; set; } = true;
    internal bool DisplayClassesToggle { get; set; } = true;
    internal bool DisplaySubclassesToggle { get; set; } = true;
    internal bool DisplayFeatsToggle { get; set; } = true;
    internal bool DisplayFeatGroupsToggle { get; set; }
    internal bool DisplayFightingStylesToggle { get; set; } = true;
    internal bool DisplayCraftingToggle { get; set; }
    internal bool DisplayMerchantsToggle { get; set; } = true;
    internal SerializableDictionary<string, bool> DisplaySpellListsToggle { get; set; } = new();

    //
    // SETTINGS HIDDEN ON UI
    //

    internal bool EnableDisplaySorceryPointBoxSorcererOnly { get; set; } = true;
    internal bool EnableMultiLinePowerPanel { get; set; } = true;
    internal bool EnableMultiLineSpellPanel { get; set; } = true;
    internal bool EnableSameWidthFeatSelection { get; set; } = true;
    internal bool EnableSortingClasses { get; set; } = true;
    internal bool EnableSortingDeities { get; set; } = true;
    internal bool EnableSortingDungeonMakerAssets { get; set; } = true;
    internal bool EnableSortingFeats { get; set; } = true;
    internal bool EnableSortingFightingStyles { get; set; } = true;
    internal bool EnableSortingFutureFeatures { get; set; } = true;
    internal bool EnableSortingRaces { get; set; } = true;
    internal bool EnableSortingSubclasses { get; set; } = true;
    internal bool KeepCharactersPanelOpenAndHeroSelectedOnLevelUp { get; set; } = true;
    internal bool DontConsumeSlots { get; set; }

    //
    // Character - General
    //

    // Initial Choices
    internal bool AddHelpActionToAllRaces { get; set; }

    // internal bool DisableSenseDarkVisionFromAllRaces { get; set; }
    // internal bool DisableSenseSuperiorDarkVisionFromAllRaces { get; set; }
    internal bool EnableAlternateHuman { get; set; }
    internal bool EnableFlexibleBackgrounds { get; set; }
    internal bool EnableFlexibleRaces { get; set; }
    internal bool EnableEpicPointsAndArray { get; set; }
    internal int TotalFeatsGrantedFirstLevel { get; set; }

    // Progression
    internal bool EnablesAsiAndFeat { get; set; }
    internal bool EnableFeatsAtEvenLevels { get; set; }
    internal bool EnableLevel20 { get; set; }

    // Multiclass
    internal int MaxAllowedClasses { get; set; }
    internal bool EnableMinInOutAttributes { get; set; } = true;
    internal bool EnableRelearnSpells { get; set; }
    internal bool DisplayAllKnownSpellsDuringLevelUp { get; set; } = true;
    internal bool DisplayPactSlotsOnSpellSelectionPanel { get; set; } = true;

    // Visuals
    internal bool OfferAdditionalLoreFriendlyNames { get; set; }
    internal bool UnlockAllNpcFaces { get; set; }
    internal bool AllowUnmarkedSorcerers { get; set; }
    internal bool UnlockMarkAndTattoosForAllCharacters { get; set; }
    internal bool UnlockEyeStyles { get; set; }
    internal bool AddNewBrightEyeColors { get; set; }
    internal bool UnlockGlowingEyeColors { get; set; }
    internal bool UnlockGlowingColorsForAllMarksAndTattoos { get; set; }

    //
    // Characters - Races, Classes & Subclasses
    //

    internal int RaceSliderPosition { get; set; } = 4;
    internal List<string> RaceEnabled { get; } = new();
    internal int ClassSliderPosition { get; set; } = 4;
    internal List<string> ClassEnabled { get; } = new();
    internal int SubclassSliderPosition { get; set; } = 4;
    internal List<string> SubclassEnabled { get; } = new();

    //
    // Characters - Feats
    //

    internal int FeatSliderPosition { get; set; } = 4;
    internal List<string> FeatEnabled { get; } = new();

    //
    // Characters - Feat Groups
    //

    internal int FeatGroupSliderPosition { get; set; } = 4;
    internal List<string> FeatGroupEnabled { get; } = new();

    //
    // Characters - Fighting Styles
    //

    internal int FightingStyleSliderPosition { get; set; } = 4;
    internal List<string> FightingStyleEnabled { get; } = new();

    //
    // Characters - Spells
    //

    internal SerializableDictionary<string, int> SpellListSliderPosition { get; set; } = new();
    internal SerializableDictionary<string, List<string>> SpellListSpellEnabled { get; set; } = new();

    //
    // Gameplay - Rules
    //

    // SRD
    internal bool UseOfficialAdvantageDisadvantageRules { get; set; }
    internal bool AddBleedingToLesserRestoration { get; set; }
    internal bool BlindedConditionDontAllowAttackOfOpportunity { get; set; }
    internal bool AllowTargetingSelectionWhenCastingChainLightningSpell { get; set; }
    internal bool BestowCurseNoConcentrationRequiredForSlotLevel5OrAbove { get; set; }
    internal bool EnableUpcastConjureElementalAndFey { get; set; }
    internal bool OnlyShowMostPowerfulUpcastConjuredElementalOrFey { get; set; }
    internal bool FixSorcererTwinnedLogic { get; set; }
    internal bool FullyControlConjurations { get; set; }
    internal bool RemoveHumanoidFilterOnHideousLaughter { get; set; }
    internal bool RemoveRecurringEffectOnEntangle { get; set; }
    internal bool ChangeSleetStormToCube { get; set; }
    internal bool UseHeightOneCylinderEffect { get; set; }
    internal bool ApplySrdWeightToFoodRations { get; set; }

    // House
    internal bool AllowStackedMaterialComponent { get; set; }
    internal bool AllowAnyClassToWearSylvanArmor { get; set; }
    internal bool AllowDruidToWearMetalArmor { get; set; }
    internal bool MakeAllMagicStaveArcaneFoci { get; set; }
    internal int IncreaseSenseNormalVision { get; set; } = SrdAndHouseRulesContext.DefaultVisionRange;

    //
    // Gameplay - Items, Crafting & Merchants
    //

    // General
#if DEBUG
    internal bool AddNewWeaponsAndRecipesToShops { get; set; } = true; // simplifies diags. creation (one less boot)
    internal bool AddNewWeaponsAndRecipesToEditor { get; set; } = true;
#else
    internal bool AddNewWeaponsAndRecipesToShops { get; set; }
    internal bool AddNewWeaponsAndRecipesToEditor { get; set; }
#endif
#if DEBUG
    internal bool AddPickPocketableLoot { get; set; } = true; // simplifies diags. creation (one less boot)
#else
    internal bool AddPickPocketableLoot { get; set; }
#endif
    internal bool AllowAnyClassToUseArcaneShieldstaff { get; set; }
    internal bool RemoveAttunementRequirements { get; set; }
    internal bool RemoveIdentificationRequirements { get; set; }
    internal bool ShowCraftingRecipeInDetailedTooltips { get; set; }
    internal int TotalCraftingTimeModifier { get; set; }
    internal int RecipeCost { get; set; } = 200;
    internal int SetBeltOfDwarvenKindBeardChances { get; set; } = 50;

    // Crafting
    internal List<string> CraftingInStore { get; } = new();
    internal List<string> CraftingItemsInDm { get; } = new();
    internal List<string> CraftingRecipesInDm { get; } = new();

    // Merchants
    internal bool ScaleMerchantPricesCorrectly { get; set; }
    internal bool StockGorimStoreWithAllNonMagicalClothing { get; set; }
    internal bool StockHugoStoreWithAdditionalFoci { get; set; }
    internal bool EnableAdditionalFociInDungeonMaker { get; set; }
    internal bool RestockAntiquarians { get; set; }
    internal bool RestockArcaneum { get; set; }
    internal bool RestockCircleOfDanantar { get; set; }
    internal bool RestockTowerOfKnowledge { get; set; }

    //
    // Gameplay - Tools
    //

    // General
    internal bool EnableSaveByLocation { get; set; }
    internal bool EnableRespec { get; set; }
    internal bool EnableCheatMenu { get; set; }
    internal bool OverrideMinMaxLevel { get; set; }
    internal bool NoExperienceOnLevelUp { get; set; }
    internal int MultiplyTheExperienceGainedBy { get; set; } = 100;
    internal int OverridePartySize { get; set; } = DungeonMakerContext.GamePartySize;


    // Debug
    internal bool DebugLogDefinitionCreation { get; set; }
    internal bool DebugLogFieldInitialization { get; set; }
    internal bool DebugDisableVerifyDefinitionNameIsNotInUse { get; set; }
#if DEBUG
    internal bool DebugLogVariantMisuse { get; set; }
#endif

    // Faction Relations

    //
    // Interface - Dungeon Maker
    //

    internal bool AllowGadgetsAndPropsToBePlacedAnywhere { get; set; }
    internal bool UnleashNpcAsEnemy { get; set; }
    internal bool UnleashEnemyAsNpc { get; set; }
    internal bool EnableDungeonMakerModdedContent { get; set; }

    //
    // Interface - Game UI
    //

    // Battle
    internal bool DontFollowCharacterInBattle { get; set; }
    internal int DontFollowMargin { get; set; } = 5;
    internal bool AutoPauseOnVictory { get; set; }
    internal float FasterTimeModifier { get; set; } = 1.5f;

    // Campaigns and Locations
    internal bool FollowCharactersOnTeleport { get; set; }
    internal bool EnableAdditionalBackstoryDisplay { get; set; }
    internal bool EnableLogDialoguesToConsole { get; set; }
    internal bool EnableStatsOnHeroTooltip { get; set; }
    internal bool EnableAdditionalIconsOnLevelMap { get; set; }
    internal bool MarkInvisibleTeleportersOnLevelMap { get; set; }
    internal bool HideExitsAndTeleportersGizmosIfNotDiscovered { get; set; }

    // Inventory and Items
    internal bool DisableAutoEquip { get; set; }
    internal bool EnableInventoryFilteringAndSorting { get; set; }
    internal bool EnableInventoryTaintNonProficientItemsRed { get; set; }
    internal bool EnableInvisibleCrownOfTheMagister { get; set; }
    internal int EmpressGarbAppearanceIndex { get; set; }

    // Monsters
    internal bool HideMonsterHitPoints { get; set; }
    internal bool RemoveBugVisualModels { get; set; }

    // Spells
    internal int MaxSpellLevelsPerLine { get; set; } = 4;

    //
    // Interface - Keyboard & Mouse
    //

    internal bool EnableHotkeyToggleHud { get; set; }
    internal bool EnableCharacterExport { get; set; }
    internal bool EnableHotkeyDebugOverlay { get; set; }
    internal bool EnableTeleportParty { get; set; }
    internal bool AltOnlyHighlightItemsInPartyFieldOfView { get; set; }
    internal bool InvertAltBehaviorOnTooltips { get; set; }

    //
    // Interface - Translations
    //

    internal string SelectedLanguageCode { get; set; } = Translations.English;

    //
    // Encounters - General
    //

    internal bool EnableEnemiesControlledByPlayer { get; set; }
    internal bool EnableHeroesControlledByComputer { get; set; }
}
