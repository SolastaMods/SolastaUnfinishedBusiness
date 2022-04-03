using SolastaCommunityExpansion.Level20.Classes;
using SolastaCommunityExpansion.Level20.Races;
using SolastaCommunityExpansion.Level20.SubClasses;
using static SolastaCommunityExpansion.Level20.SpellsHelper;

namespace SolastaCommunityExpansion.Models
{
    internal static class Level20Context
    {
        public const int MOD_MIN_LEVEL = 1;
        public const int MOD_MAX_LEVEL = 20;
        public const int GAME_MAX_LEVEL = 13;
        public const int MAX_SPELL_LEVEL = 9;
        public const int MAX_CHARACTER_EXPERIENCE = 355000;

        internal static void Load()
        {
            //
            // should not be protected to avoid issues on MP or loading heroes
            //
            UpdateSpellLists();

            ElfHighBuilder.Load();

            BarbarianBuilder.Load();
            ClericBuilder.Load();
            DruidBuilder.Load();
            FighterBuilder.Load();
            PaladinBuilder.Load();
            RangerBuilder.Load();
            RogueBuilder.Load();
            SorcererBuilder.Load();
            WizardBuilder.Load();

            ConArtistBuilder.Load();
            MartialSpellBladeBuilder.Load();
            ShadowcasterBuilder.Load();
            SpellShieldBuilder.Load();
        }
    }
}
