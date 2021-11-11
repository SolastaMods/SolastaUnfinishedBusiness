using SolastaCommunityExpansion.Feats;
using SolastaCommunityExpansion.Subclasses.Rogue;
using SolastaCommunityExpansion.Subclasses.Wizard;
using System.Collections.Generic;
using UnityModManagerNet;

namespace SolastaCommunityExpansion
{
    public class Core
    {

    }
    
    public class Settings : UnityModManager.ModSettings
    {
        //
        // TODO: Reorganize the order of these settings in code per viewers on UI to simplify maintenance
        //

        public const string GUID = "b1ffaca74824486ea74a68d45e6b1925";

        public const int MIN_INITIAL_FEATS = 0;
        public const int MAX_INITIAL_FEATS = 10;

        /* This is in the settings so it can be disabled, but if a player has access to first level feats they almost definitely want this on. */
        public bool EnableFirstLevelCasterFeats = true;
        // If this is off only the first auto prepared spells group is used during level up
        public bool ShowAllAutoPreparedSpells = true;
        public bool FutureFeatureSorting = true;
        public bool MultiLineSpellPanel = true;
        public bool MultiLinePowerPanel = true;
        public bool KeepSpellsOpenSwitchingEquipment = true;
        public bool BugFixExpandColorTables = true;

        public bool EnableEpicPoints = false;
        public bool EnableAlternateHuman = false;
        public bool EnablesAsiAndFeat = false;
        public bool EnableLevel20 = false;
        public bool EnableFlexibleBackgrounds = false;
        public bool EnableFlexibleRaces = false;

        public bool DisableSenseDarkVisionFromAllRaces = false;
        public bool DisableSenseSuperiorDarkVisionFromAllRaces = false;

        public int AllRacesInitialFeats = 0;

        public List<string> InStore = new List<string>();

        private int recipeCost = 200;
        public int RecipeCost
        {
            get => recipeCost; set
            {
                recipeCost = value;
                Models.ItemCraftingContext.UpdateRecipeCost();
            }
        }

        public List<string> FeatEnabled= new List<string>();
        public List<string> SubclassEnabled= new List<string>();
        public List<string> FightingStyleEnabled;

        public int FeatSliderPosition = 1;
        public int SubclassSliderPosition = 1;
        public int FightingStyleSliderPosition = 1;

        private int rogueConArtistSpellDCBoost = 3;

        public int RogueConArtistSpellDCBoost
        {
            get => rogueConArtistSpellDCBoost; set
            {
                rogueConArtistSpellDCBoost = value;
                ConArtist.UpdateSpellDCBoost();
            }
        }

        private int masterManipulatorSpellDCBoost = 2;

        public int MasterManipulatorSpellDCBoost
        {
            get => masterManipulatorSpellDCBoost; set
            {
                masterManipulatorSpellDCBoost = value;
                MasterManipulator.UpdateSpellDCBoost();
            }
        }

        private int featPowerAttackModifier = 3;

        public int FeatPowerAttackModifier
        {
            get => featPowerAttackModifier; set
            {
                featPowerAttackModifier = value;
                AcehighFeats.UpdatePowerAttackModifier();
            }
        }

        /* Commands to allow the player to hide certain parts of the HUD */
        public const InputCommands.Id CTRL_C = (InputCommands.Id)44440000;
        public const InputCommands.Id CTRL_L = (InputCommands.Id)44440001;
        public const InputCommands.Id CTRL_M = (InputCommands.Id)44440002;
        public const InputCommands.Id CTRL_P = (InputCommands.Id)44440003;

        public const RestActivityDefinition.ActivityCondition ActivityConditionCanRespec = (RestActivityDefinition.ActivityCondition)(int)-1001;

        public bool EnableRespec = false;

        public bool NoExperienceOnLevelUp = false;

        public bool OfferAdditionalNames = false;
        public bool InvertAltBehaviorOnTooltips = false;

        public int MaxSpellLevelsPerLine = 5;
        public float SpellPanelGapBetweenLines = 30f;

        /* Faster Time Scale */
        public float CustomTimeScale = 1.5f;
        public bool PermanentSpeedUp = false;

        public bool AutoPauseOnVictory;
        public bool HideMonsterHitPoints;
        public bool SpellMasterUnlimitedArcaneRecovery = false;
        public bool PickPocketEnabled = false;

        public bool DisableAutoEquip;
        public bool ExactMerchantCostScaling;
        public bool NoIdentification;
        public bool NoAttunement;
        public bool SetMaxFactionRelations;

        public bool EnableSRDAdvantageRules = false;
        public bool EnableConditionBlindedShouldNotAllowOpportunityAttack = false;
        public bool AllowExtraKeyboardCharactersInNames = false;
        public bool DruidNoMetalRestriction;
        public bool RecipeTooltipShowsRecipe;
        public bool EnableCheatMenuDuringGameplay = false;
    }
}
