namespace SolastaCommunityExpansion.Builders;

public static class DiceByRankBuilder
{
    public static DiceByRank BuildDiceByRank(int rank, int dice)
    {
        var diceByRank = new DiceByRank();
        diceByRank.rank = rank;
        diceByRank.diceNumber = dice;
        return diceByRank;
    }
}
