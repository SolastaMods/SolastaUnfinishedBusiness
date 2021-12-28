namespace SolastaCommunityExpansion.Models
{
    internal static class EpicArrayContext
    {
        internal const int GAME_MAX_ATTRIBUTE = 15;
        internal const int GAME_BUY_POINTS = 27;

        internal const int MOD_MAX_ATTRIBUTE = 17;
        internal const int MOD_BUY_POINTS = 35;

        internal static void Load()
        {
            if (Main.Settings.EnableEpicArray)
            {
                AttributeDefinitions.PredeterminedRollScores = new int[6] { 17, 15, 13, 12, 10, 8 };
            }
            else
            {
                AttributeDefinitions.PredeterminedRollScores = new int[6] { 15, 14, 13, 12, 10, 8 };
            }
        }
    }
}
