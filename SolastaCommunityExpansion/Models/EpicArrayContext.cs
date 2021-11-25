namespace SolastaCommunityExpansion.Models
{
    internal static class EpicArrayContext
    {
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
