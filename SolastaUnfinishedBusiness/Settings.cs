using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Displays;
using SolastaUnfinishedBusiness.Models;
using UnityModManagerNet;

namespace SolastaUnfinishedBusiness;

public sealed class Core;

public enum TagType
{
    QoL,
    T2014,
    T2024
}

[AttributeUsage(AttributeTargets.Property)]
public class Tag : Attribute
{
    public TagType Type { get; set; }
}

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
    public bool DisplayTabletop2024 { get; set; }
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
    public bool EnableCustomPortraits { get; set; } = true;
    public bool DisableMultilineSpellOffering { get; set; }
    public bool DisableUnofficialTranslations { get; set; } = true;

    //
    // Gameplay - Rules
    //

    public bool EnableEpicPointsAndArray { get; set; }
    [Tag(Type = TagType.T2014)] public bool EnableLevel20 { get; set; }
    [Tag(Type = TagType.T2014)] public bool EnableMulticlass { get; set; }
    public int MaxAllowedClasses { get; set; }
    public bool DisplayAllKnownSpellsDuringLevelUp { get; set; }
    public bool DisplayPactSlotsOnSpellSelectionPanel { get; set; }
    public bool EnableMinInOutAttributes { get; set; }
    [Tag(Type = TagType.QoL)] public bool EnableActionSwitching { get; set; }
    [Tag(Type = TagType.T2014)] public bool DontEndTurnAfterReady { get; set; }
    [Tag(Type = TagType.T2014)] public bool EnableProneAction { get; set; }
    [Tag(Type = TagType.T2014)] public bool EnableGrappleAction { get; set; }
    [Tag(Type = TagType.T2014)] public bool EnableHelpAction { get; set; }
    public bool EnableRespecAction { get; set; }
    [Tag(Type = TagType.T2014)] public bool EnableUnarmedMainAttackAction { get; set; }
    public bool EnableUnlimitedInventoryActions { get; set; }
    [Tag(Type = TagType.T2014)] public bool UseOfficialAdvantageDisadvantageRules { get; set; }
    public bool UseAlternateSpellPointsSystem { get; set; }
    [Tag(Type = TagType.T2024)] public bool UseWeaponMasterySystem { get; set; }
    [Tag(Type = TagType.T2024)] public bool UseWeaponMasterySystemAddWeaponTag { get; set; }
    public bool UseWeaponMasterySystemAddCleaveDamage { get; set; }
    public bool UseWeaponMasterySystemPushSave { get; set; }
    public bool UseOfficialFlankingRules { get; set; }
    public bool UseMathFlankingRules { get; set; }
    public bool UseOfficialFlankingRulesAlsoForReach { get; set; }
    public bool UseOfficialFlankingRulesAlsoForRanged { get; set; }
    public bool UseOfficialFlankingRulesButAddAttackModifier { get; set; }
    [Tag(Type = TagType.T2014)] public bool BlindedConditionDontAllowAttackOfOpportunity { get; set; }
    [Tag(Type = TagType.T2014)] public bool UseOfficialLightingObscurementAndVisionRules { get; set; }
    [Tag(Type = TagType.T2014)] public bool OfficialObscurementRulesInvisibleCreaturesCanBeTarget { get; set; }
    [Tag(Type = TagType.T2014)] public bool OfficialObscurementRulesCancelAdvDisPairs { get; set; }
    public bool OfficialObscurementRulesHeavilyObscuredAsProjectileBlocker { get; set; }
    public bool OfficialObscurementRulesMagicalDarknessAsProjectileBlocker { get; set; }
    [Tag(Type = TagType.T2014)] public bool OfficialObscurementRulesTweakMonsters { get; set; }
    [Tag(Type = TagType.T2014)] public bool KeepStealthOnHeroIfPerceivedDuringSurpriseAttack { get; set; }
    [Tag(Type = TagType.T2014)] public bool StealthDoesNotBreakWithSubtle { get; set; }
    [Tag(Type = TagType.T2014)] public bool StealthBreaksWhenAttackHits { get; set; }
    [Tag(Type = TagType.T2014)] public bool StealthBreaksWhenAttackMisses { get; set; }
    public bool StealthBreaksWhenCastingMaterial { get; set; }
    [Tag(Type = TagType.T2014)] public bool StealthBreaksWhenCastingVerbose { get; set; }
    public bool StealthBreaksWhenCastingSomatic { get; set; }
    [Tag(Type = TagType.T2014)] public bool AccountForAllDiceOnSavageAttack { get; set; }
    [Tag(Type = TagType.T2014)] public bool AddDexModifierToEnemiesInitiativeRoll { get; set; }
    [Tag(Type = TagType.T2014)] public bool EnemiesAlwaysRollInitiative { get; set; }
    public bool AllowFlightSuspend { get; set; }
    public bool FlightSuspendWingedBoots { get; set; }
    [Tag(Type = TagType.T2014)] public bool EnablePullPushOnVerticalDirection { get; set; }
    [Tag(Type = TagType.T2014)] public bool FullyControlConjurations { get; set; }
    public bool EnableHigherGroundRules { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableSurprisedToEnforceDisadvantage { get; set; }
    [Tag(Type = TagType.T2014)] public bool EnableTeleportToRemoveRestrained { get; set; }
    public bool EnableCharactersOnFireToEmitLight { get; set; }
    [Tag(Type = TagType.T2014)] public bool ColdResistanceAlsoGrantsImmunityToChilledCondition { get; set; }
    [Tag(Type = TagType.T2014)] public bool ColdImmunityAlsoGrantsImmunityToChilledAndFrozenCondition { get; set; }
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
    [Tag(Type = TagType.QoL)] public bool EnableAdditionalIconsOnLevelMap { get; set; }
    public bool HideExitsAndTeleportersGizmosIfNotDiscovered { get; set; }
    public bool EnableLogDialoguesToConsole { get; set; }
    public bool EnableSpeech { get; set; }
    public bool EnableSpeechOnNpcs { get; set; }
    public bool ForceModSpeechOnNpcs { get; set; }
    public int SpeechChoice { get; set; }
    public SerializableDictionary<int, (string, float)> SpeechVoices { get; set; } = [];
    public bool EnableHeroWithBestProficiencyToRollChoice { get; set; }
    public bool MarkInvisibleTeleportersOnLevelMap { get; set; }
    public bool EnableAlternateVotingSystem { get; set; }
    public bool EnableSumD20OnAlternateVotingSystem { get; set; }
    [Tag(Type = TagType.QoL)] public bool AllowMoreRealStateOnRestPanel { get; set; }
    [Tag(Type = TagType.QoL)] public bool EnableStatsOnHeroTooltip { get; set; }
    public bool EnableAdditionalBackstoryDisplay { get; set; }
    public bool EnableExtendedProficienciesPanelDisplay { get; set; }
    public bool HideMonsterHitPoints { get; set; }
    public bool RemoveBugVisualModels { get; set; }
    [Tag(Type = TagType.QoL)] public bool ShowButtonWithControlledMonsterInfo { get; set; }

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
    [Tag(Type = TagType.QoL)] public bool EnableDistanceOnTooltip { get; set; }
    [Tag(Type = TagType.QoL)] public bool ShowMotionFormPreview { get; set; }
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
    [Tag(Type = TagType.QoL)] public bool ScaleMerchantPricesCorrectly { get; set; }
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
    [Tag(Type = TagType.T2014)] public bool UseOfficialFoodRationsWeight { get; set; }
    public bool MakeAllMagicStaveArcaneFoci { get; set; }
    [Tag(Type = TagType.T2014)] public bool FixRingOfRegenerationHealRate { get; set; }
    public bool IgnoreHandXbowFreeHandRequirements { get; set; }

    [Tag(Type = TagType.T2024)] public bool EnablePotionsBonusAction2024 { get; set; }

    [Tag(Type = TagType.T2024)] public bool EnablePoisonsBonusAction2024 { get; set; }
    public bool KeepInvisibilityWhenUsingItems { get; set; }
    public bool AddCustomIconsToOfficialItems { get; set; }
    public bool DisableAutoEquip { get; set; }
    [Tag(Type = TagType.QoL)] public bool EnableInventoryFilteringAndSorting { get; set; }
    [Tag(Type = TagType.QoL)] public bool EnableInventoryTaintNonProficientItemsRed { get; set; }
    [Tag(Type = TagType.QoL)] public bool EnableInventoryTintKnownRecipesRed { get; set; }
    public bool EnableStackableAxesAndDaggers { get; set; }
    public bool EnableStackableArtItems { get; set; }
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

    public bool EnableBardScimitarSpecialization { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableBardicInspiration2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableBardCounterCharm2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableBardExpertiseOneLevelBefore2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableBardSuperiorInspiration2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableBardWordsOfCreation2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableBardMagicalSecrets2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool RemoveBardSongOfRest2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableBarbarianBrutalStrike2024 { get; set; }
    public bool EnableBarbarianFightingStyle { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableBarbarianInstinctivePounce2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableBarbarianPersistentRage2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableBarbarianPrimalKnowledge2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableBarbarianReckless2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableBarbarianRage2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableBarbarianRelentlessRage2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableClericBlessedStrikes2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableClericChannelDivinity2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableClericDivineOrder2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableClericSearUndead2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableClericToLearnDomainAtLevel3 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableDruidArchDruid2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableDruidToLearnCircleAtLevel3 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableDruidElementalFury2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableDruidMetalArmor2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableDruidPrimalOrder2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableDruidWeaponProficiency2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableDruidWildResurgence2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableDruidWildshape2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableFighterIndomitableSaving2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableFighterSkillOptions2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableFighterSecondWind2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableFighterStudiedAttacks2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableFighterTacticalMaster2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableFighterTacticalProgression2024 { get; set; }
    public bool EnableMonkAbundantKi { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableMonkBodyAndMind2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableMonkDeflectAttacks2024 { get; set; }
    public bool EnableMonkFightingStyle { get; set; }
    public bool EnableMonkHandwrapsOnGauntletSlot { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableMonkHeightenedFocus2024 { get; set; }
    public bool EnableMonkImprovedUnarmoredMovement { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableMonkSelfRestoration2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableMonkSuperiorDefense2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableMonkUncannyMetabolism2024 { get; set; }
    public bool EnableMonkKatanaSpecialization { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableMonkFocus2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableMonkMartialArts2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableMonkStunningStrike2024 { get; set; }
    public bool AddPaladinSmiteToggle { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnablePaladinAbjureFoes2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnablePaladinChannelDivinity2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnablePaladinLayOnHands2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnablePaladinRechargeLv20Feature { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnablePaladinRestoringTouch2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnablePaladinSmite2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnablePaladinSpellCastingAtLevel1 { get; set; }
    public bool ShowChannelDivinityOnPortrait { get; set; }
    public bool AddHumanoidFavoredEnemyToRanger { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableRangerDeftExplorer2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableRangerExpertise2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableRangerFavoredEnemy2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableRangerNatureShroud2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableRangerPreciseHunter2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableRangerRelentlessHunter2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableRangerRelentlessHunter2024AsNoConcentration { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableRangerRoving2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableRangerSpellCastingAtLevel1 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableRangerTireless2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool RemoveRangerPrimevalAwareness2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableRangerFeralSenses2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableRangerFoeSlayers2024 { get; set; }
    public bool EnableRogueScimitarSpecialization { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableRogueCunningStrike2024 { get; set; }
    public bool EnableRogueFightingStyle { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableRogueReliableTalent2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableRogueSlipperyMind2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableRogueSteadyAim2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool RemoveRogueBlindSense2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableSorcererArcaneApotheosis2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableSorcererInnateSorcery2024 { get; set; }
    [Tag(Type = TagType.T2014)] public bool EnableSorcererMagicalGuidance { get; set; }
    [Tag(Type = TagType.T2014)] public bool EnableSorcererQuickenedAction { get; set; }
    public bool HideQuickenedActionWhenMetamagicOff { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableSorcererSorcerousRestoration2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableSorcererMetamagic2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableSorcererOrigin2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableWarlockInvocationProgression2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableWarlockMagicalCunningAndImprovedEldritchMaster2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableWarlockToLearnPatronAtLevel3 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableWizardMemorizeSpell2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableWizardToLearnScholarAtLevel2 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableWizardToLearnSchoolAtLevel3 { get; set; }
    public bool EnableSignatureSpellsRelearn { get; set; }

    //
    // Characters - Backgrounds, Races & Subraces
    //

    public bool EnableFlexibleBackgrounds { get; set; }
    public bool EnableFlexibleRaces { get; set; }
    public bool ChangeDragonbornElementalBreathUsages { get; set; }
    [Tag(Type = TagType.T2014)] public bool EnableAlternateHuman { get; set; }
    [Tag(Type = TagType.T2014)] public bool UseOfficialSmallRacesDisWithHeavyWeapons { get; set; }
    public bool DisableSenseDarkVisionFromAllRaces { get; set; }
    public bool DisableSenseSuperiorDarkVisionFromAllRaces { get; set; }
    public bool AddDarknessPerceptiveToDarkRaces { get; set; }
    [Tag(Type = TagType.T2014)] public bool RaceLightSensitivityApplyOutdoorsOnly { get; set; }
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
    [Tag(Type = TagType.T2014)] public bool AccountForAllDiceOnFollowUpStrike { get; set; }
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
    [Tag(Type = TagType.T2024)] public bool EnablePreparedSpellsTables2024 { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableRitualOnAllCasters2024 { get; set; }
    public bool QuickCastLightCantripOnWornItemsFirst { get; set; }
    [Tag(Type = TagType.T2014)] public bool IllusionSpellsAutomaticallyFailAgainstTrueSightInRange { get; set; }
    public bool AllowTargetingSelectionWhenCastingChainLightningSpell { get; set; }
    public bool RemoveHumanoidFilterOnHideousLaughter { get; set; }
    public bool AddBleedingToLesserRestoration { get; set; }
    [Tag(Type = TagType.T2014)] public bool BestowCurseNoConcentrationRequiredForSlotLevel5OrAbove { get; set; }
    [Tag(Type = TagType.T2014)] public bool RemoveRecurringEffectOnEntangle { get; set; }
    public bool EnableUpcastConjureElementalAndFey { get; set; }
    public bool OnlyShowMostPowerfulUpcastConjuredElementalOrFey { get; set; }
    public bool ChangeSleetStormToCube { get; set; }
    public bool UseHeightOneCylinderEffect { get; set; }
    [Tag(Type = TagType.T2014)] public bool FixEldritchBlastRange { get; set; }
    public bool ModifyGravitySlam { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableOneDndBarkskinSpell { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableOneDndDamagingSpellsUpgrade { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableOneDndHealingSpellsUpgrade { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableOneDndDivineFavorSpell { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableOneDndGuidanceSpell { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableOneDndHideousLaughterSpell { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableOneDndHuntersMarkSpell { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableOneDndLesserRestorationSpell { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableOneDndMagicWeaponSpell { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableOneDndPowerWordStunSpell { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableOneDndSpareTheDyingSpell { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableOneDndSpiderClimbSpell { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableOneDndStoneSkinSpell { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableOneDndTrueStrikeCantrip { get; set; }
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
    [Tag(Type = TagType.T2014)] public bool EnableBardHealingBalladOnLongRest { get; set; }
    public bool EnableBg3AbjurationArcaneWard { get; set; }
    public bool EnableRogueStrSaving { get; set; }
    public bool RemoveSchoolRestrictionsFromShadowCaster { get; set; }
    public bool RemoveSchoolRestrictionsFromSpellBlade { get; set; }
    public int WildSurgeDieRollThreshold { get; set; } = 2;
    [Tag(Type = TagType.T2024)] public bool SwapAbjurationSavant { get; set; }
    [Tag(Type = TagType.T2024)] public bool SwapEvocationSavant { get; set; }
    [Tag(Type = TagType.T2024)] public bool SwapEvocationPotentCantripAndSculptSpell { get; set; }
    [Tag(Type = TagType.T2024)] public bool EnableMartialChampion2024 { get; set; }
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
