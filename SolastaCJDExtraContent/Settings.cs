using UnityModManagerNet;

namespace SolastaCJDExtraContent
{
    public class Core
    {

    }
    
    public class Settings : UnityModManager.ModSettings
    {
        public const string GUID = "b1ffaca74824486ea74a68d45e6b1925";

        public const int MIN_INITIAL_FEATS = 0;
        public const int MAX_INITIAL_FEATS = 10;

        public int AllRacesInitialFeats = 0;
        public bool AlternateHuman = false;
        /* This is in the settings so it can be disabled, but if a player has access to first level feats they almost definitely want this on. */
        public bool EnableFirstLevelCasterFeats = true;

        public bool EnablesAsiAndFeat = false;
    }
}
