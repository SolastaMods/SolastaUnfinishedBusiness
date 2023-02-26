namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IChangeDieRoll
{
    public void ChangeDieRoll(
        RuleDefinitions.DieType dieType,
        RuleDefinitions.AdvantageType advantageType,
        ref int firstRoll,
        ref int secondRoll,
        float rollAlterationScore,
        RulesetActor actor,
        RuleDefinitions.RollContext rollContext,
        ref int result);
}
