namespace SolastaUnfinishedBusiness.Interfaces;

public interface IActionItemDiceBox
{
    (RuleDefinitions.DieType type, int number, string format) GetDiceInfo(RulesetCharacter character);
}
