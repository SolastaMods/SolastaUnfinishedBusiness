namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IActionItemDiceBox
{
    (RuleDefinitions.DieType type, int number, string format) GetDiceInfo(RulesetCharacter character);
}
