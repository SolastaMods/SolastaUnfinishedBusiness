using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityModManagerNet;

namespace SolastaCommunityExpansion
{
    public class Core
    {

    }

    [Serializable]
    [XmlRoot(ElementName = "Settings")]
    public class Settings : UnityModManager.ModSettings
    {
        //
        // TODO: Reorganize the order of these settings in code per viewers on UI to simplify maintenance
        // NOTE: xml deserialization expects properties to match the order of the xml, so reorganizing will 
        // temporarily lose settings.
        //

        public const string GUID = "b1ffaca74824486ea74a68d45e6b1925";

        public const int MIN_INITIAL_FEATS = 0;
        public const int MAX_INITIAL_FEATS = 10;

        public const int GAME_MAX_ATTRIBUTE = 15;
        public const int GAME_BUY_POINTS = 27;

        public const int MOD_MAX_ATTRIBUTE = 17;
        public const int MOD_BUY_POINTS = 35;

        /* This is in the settings so it can be disabled, but if a player has access to first level feats they almost definitely want this on. */
        public bool EnableFirstLevelCasterFeats { get; set; } = true;
        // If this is off only the first auto prepared spells group is used during level up
        public bool ShowAllAutoPreparedSpells { get; set; } = true;
        public bool FutureFeatureSorting { get; set; } = true;
        public bool MultiLineSpellPanel { get; set; } = true;
        public bool MultiLinePowerPanel { get; set; } = true;
        public bool KeepSpellsOpenSwitchingEquipment { get; set; } = true;
        public bool BugFixExpandColorTables { get; set; } = true;
        public bool AllowDynamicPowers { get; set; } = true;

        public bool EnableEpicPoints { get; set; }
        public bool EnableEpicArray { get; set; }
        public bool EnableAlternateHuman { get; set; }
        public bool EnablesAsiAndFeat { get; set; }
        public bool EnableLevel20 { get; set; }
        public bool EnableFlexibleBackgrounds { get; set; }
        public bool EnableFlexibleRaces { get; set; }

        public bool DisableSenseDarkVisionFromAllRaces { get; set; }
        public bool DisableSenseSuperiorDarkVisionFromAllRaces { get; set; }
        public bool IncreaseNormalVisionSenseRange { get; set; }

        public int AllRacesInitialFeats { get; set; }

        public List<string> InStore { get; private set; } = new List<string>();
        public List<string> ItemsInDM { get; private set; } = new List<string>();
        public List<string> RecipesInDM { get; private set; } = new List<string>();

        public int RecipeCost { get; set; } = 200;

        public List<string> FeatEnabled { get; private set; } = new List<string>();
        public List<string> SubclassEnabled { get; private set; } = new List<string>();
        public List<string> FightingStyleEnabled { get; private set; } = new List<string>();

        public int FeatSliderPosition { get; set; } = 1;
        public int SubclassSliderPosition { get; set; } = 1;
        public int FightingStyleSliderPosition { get; set; } = 1;

        public int RogueConArtistSpellDCBoost { get; set; } = 3;
        public int MasterManipulatorSpellDCBoost { get; set; } = 2;
        public int FeatPowerAttackModifier { get; set; } = 3;

        /* Commands to allow the player to hide certain parts of the HUD */
        public const InputCommands.Id CTRL_C = (InputCommands.Id)44440000;
        public const InputCommands.Id CTRL_L = (InputCommands.Id)44440001;
        public const InputCommands.Id CTRL_M = (InputCommands.Id)44440002;
        public const InputCommands.Id CTRL_P = (InputCommands.Id)44440003;

        /* Character Export hotkey */
        public const InputCommands.Id CTRL_E = (InputCommands.Id)44440004;

        public const RestActivityDefinition.ActivityCondition ActivityConditionCanRespec = (RestActivityDefinition.ActivityCondition)(-1001);

        public bool EnableRespec { get; set; }

        public bool NoExperienceOnLevelUp { get; set; }

        public bool OfferAdditionalNames { get; set; }
        public bool InvertAltBehaviorOnTooltips { get; set; }
        public bool EnableCharacterExport { get; set; }

        public int MaxSpellLevelsPerLine { get; set; } = 5;
        public float SpellPanelGapBetweenLines { get; set; } = 30f;

        /* Faster Time Scale */
        public float CustomTimeScale { get; set; } = 1.0f;
        public bool PermanentSpeedUp { get; set; }

        public bool AutoPauseOnVictory { get; set; }
        public bool HideMonsterHitPoints { get; set; }
        public bool SpellMasterUnlimitedArcaneRecovery { get; set; }
        public bool PickPocketEnabled { get; set; }

        public bool DisableAutoEquip { get; set; }
        public bool ExactMerchantCostScaling { get; set; }
        public bool NoIdentification { get; set; }
        public bool NoAttunement { get; set; }

        public bool EnableSRDAdvantageRules { get; set; }
        public bool EnableSRDCombatSurpriseRules { get; set; }
        public bool EnableSRDCombatSurpriseRulesManyRolls { get; set; }
        public bool EnableConditionBlindedShouldNotAllowOpportunityAttack { get; set; }
        public bool AllowExtraKeyboardCharactersInNames { get; set; }
        public bool DruidNoMetalRestriction { get; set; }
        public bool RecipeTooltipShowsRecipe { get; set; }
        public bool EnableCheatMenuDuringGameplay { get; set; }

        public bool EnableHudToggleElementsHotkeys { get; set; }

        public int ExperienceModifier { get; set; } = 100;

        public bool EnableFeatsSorting { get; set; } = true;

        public bool EnableInventoryFilterAndSort { get; set; } = true;

        public bool RemoveBugVisualModels { get; set; }

        public bool FullyControlAlliedConjurations { get; set; }
        public bool DismissControlledConjurationsWhenDeliberatelyDropConcentration { get; set; }

        public bool EnableFaceUnlockEyeStyles { get; set; }
        public bool EnableFaceUnlockGlowingEyes { get; set; }
        public bool EnableFaceUnlockGlowingBodyDecorations { get; set; }
        public bool EnableFaceUnlockNpcs { get; set; }
        public bool EnableFaceUnlockMarkingsForAll { get; set; }
        public bool EnableFaceUnlockUnmarkedSorcerers { get; set; }

        public bool EnableSaveByLocation { get; set; }

        public bool EnableUniversalSylvanArmor { get; set; }
        public bool EnableInvisibleCrownOfTheMagister { get; set; }
        public bool EnableClothingGorimStock { get; set; }
        public bool EnableRestockAntiquarians { get; set; }
        public bool EnableRestockArcaneum { get; set; }
        public bool EnableRestockCircleOfDanantar { get; set; }
        public bool EnableRestockTowerOfKnowledge { get; set; }

        public int BeltOfDwarvenKindBeardChances { get; set; } = 50;

        public bool EnableMagicStaffFoci { get; set; }
        public bool CreateAdditionalFoci { get; set; }
        public bool EnableAdditionalFociDungeonMaker { get; set; }

        public string EmpressGarbSkin { get; set; } = "Normal";

        public bool DontFollowCharacterInBattle { get; set; }
        public int DontFollowMargin { get; set; } = 5;

        public bool EnableAdventureLogBanterLines { get; set; }
        public bool EnableAdventureLogDocuments { get; set; }
        public bool EnableAdventureLogLore { get; set; }
        public bool EnableAdventureLogTextFeedback { get; set; }
        public bool EnableAdventureLogPopups { get; set; }

        public const int DUNGEON_MIN_LEVEL = 1;
        public const int DUNGEON_MAX_LEVEL = 20;

        public const int GAME_PARTY_SIZE = 4;

        public const int MIN_PARTY_SIZE = 1;
        public const int MAX_PARTY_SIZE = 6;

        public const float ADVENTURE_PANEL_DEFAULT_SCALE = 0.75f;
        public const float REST_PANEL_DEFAULT_SCALE = 0.8f;
        public const float PARTY_CONTROL_PANEL_DEFAULT_SCALE = 0.95f;
        public const float VICTORY_MODAL_DEFAULT_SCALE = 0.85f;
        public const float REVIVE_PARTY_CONTROL_PANEL_DEFAULT_SCALE = 0.85f;

        public bool EnableDungeonLevelBypass { get; set; }
        public int UserDungeonsPartySize { get; set; } = GAME_PARTY_SIZE;

        public bool EnableTelemaCampaign { get; set; }

        public bool FlexibleGadgetsPlacement { get; set; }
        public bool FlexiblePropsPlacement { get; set; }

        public bool DungeonMakerEditorBetterTooltips { get; set; }
        public bool UnleashAllMonsters { get; set; }
        public bool UnleashAllNPCs { get; set; }
        public int maxBackupFiles { get; set; } = 10;

        public const int MAX_ENCOUNTER_CHARACTERS = 16;
        public const int PLAYER_CONTROLLER_ID = 1;

        public const InputCommands.Id CTRL_SHIFT_E = (InputCommands.Id)44440005;
        public bool EnableHeroesControlledByComputer { get; set; }
        public bool EnableEnemiesControlledByPlayer { get; set; }

        public bool ArcaneFighterEnchantWeaponRechargeShortRest { get; set; }
    }
}
