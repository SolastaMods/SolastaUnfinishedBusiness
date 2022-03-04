using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public static class DiceByRankBuilder
    {
        public static DiceByRank BuildDiceByRank(int rank, int dice)
        {
            DiceByRank diceByRank = new DiceByRank();
            diceByRank.SetRank(rank);
            diceByRank.SetDiceNumber(dice);
            return diceByRank;
        }
    }
}
