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
        public bool AllowNonCastersMagicalFeats = false;

        public bool EnablesAsiAndFeat = false;
    }
}
