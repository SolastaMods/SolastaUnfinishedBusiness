using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IModifyDiceRollHitDice
{
    [UsedImplicitly]
    public void BeforeRoll(
        RulesetCharacterHero __instance,
        ref DieType die,
        ref int modifier,
        ref AdvantageType advantageType,
        ref bool healKindred,
        ref bool isBonus);
}
