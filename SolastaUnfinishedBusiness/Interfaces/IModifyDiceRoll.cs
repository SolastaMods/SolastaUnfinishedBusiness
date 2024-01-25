using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IModifyDiceRoll
{
    [UsedImplicitly]
    public void BeforeRoll(
        RollContext rollContext,
        RulesetCharacter rulesetCharacter,
        ref DieType dieType,
        ref AdvantageType advantageType);

    [UsedImplicitly]
    public void AfterRoll(
        RollContext rollContext,
        RulesetCharacter rulesetCharacter,
        ref int result);
}
