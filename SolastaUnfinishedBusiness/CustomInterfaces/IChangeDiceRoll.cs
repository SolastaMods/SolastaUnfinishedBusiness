using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IChangeDiceRoll
{
    [UsedImplicitly]
    public bool IsValid(RuleDefinitions.RollContext rollContext, RulesetCharacter rulesetCharacter);

    [UsedImplicitly]
    public void BeforeRoll(
        RuleDefinitions.RollContext rollContext,
        RulesetCharacter rulesetCharacter,
        ref RuleDefinitions.DieType dieType,
        ref RuleDefinitions.AdvantageType advantageType);

    [UsedImplicitly]
    public void AfterRoll(
        RuleDefinitions.RollContext rollContext,
        RulesetCharacter rulesetCharacter,
        ref int firstRoll,
        ref int secondRoll);
}
