namespace SolastaCommunityExpansion.Models
{
    public class ActivePalyerCharacter
    {
        private static GameLocationCharacter _character = null;

        public static void Set(GameLocationCharacter character = null)
        {
            _character = character;
        }

        public static GameLocationCharacter Get()
        {
            return _character;
        }
    }
}
