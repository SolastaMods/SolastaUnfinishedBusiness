using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders
{
    public static class DiceByRankBuilder
    {
        public static DiceByRank BuildDiceByRank(int rank, int dice)
        {
            var diceByRank = new DiceByRank();
            diceByRank.SetRank(rank);
            diceByRank.SetDiceNumber(dice);
            return diceByRank;
        }
    }
}
