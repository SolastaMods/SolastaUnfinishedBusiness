using SolastaContentExpansion.Subclasses.Rogue;
using System.Collections.Generic;
using UnityModManagerNet;

namespace SolastaContentExpansion
{
    public class Core
    {

    }
    
    public class Settings : UnityModManager.ModSettings
    {
        public const string GUID = "b1ffaca74824486ea74a68d45e6b1925";

        public const int MIN_INITIAL_FEATS = 0;
        public const int MAX_INITIAL_FEATS = 10;

        /* This is in the settings so it can be disabled, but if a player has access to first level feats they almost definitely want this on. */
        public bool EnableFirstLevelCasterFeats = true;
        
        public bool EnableAlternateHuman = false;
        public bool EnablesAsiAndFeat = false;
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

        public List<string> FeatHidden = new List<string>();
        public List<string> SubclassHidden = new List<string>();

        private int rogueConArtistSpellDCBoost = 3;

        public int RogueConArtistSpellDCBoost
        {
            get => rogueConArtistSpellDCBoost; set
            {
                rogueConArtistSpellDCBoost = value;
                ConArtist.UpdateSpellDCBoost();
            }
        }

        /* Commands to allow the player to hide certain parts of the HUD */
        public const InputCommands.Id CTRL_C = (InputCommands.Id)44440000;
        public const InputCommands.Id CTRL_L = (InputCommands.Id)44440001;
        public const InputCommands.Id CTRL_M = (InputCommands.Id)44440002;
        public const InputCommands.Id CTRL_P = (InputCommands.Id)44440003;

        public bool OfferAdditionalNames = true;
        public bool InvertAltBehaviorOnTooltips = true;

        public int MaxSpellLevelsPerLine = 5;
        public float SpellPanelGapBetweenLines = 30f;

        public bool HideMonsterHitPoints;
    }
}
