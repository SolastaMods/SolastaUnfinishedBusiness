using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

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
