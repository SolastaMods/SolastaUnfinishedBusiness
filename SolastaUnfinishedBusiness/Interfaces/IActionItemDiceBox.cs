namespace SolastaUnfinishedBusiness.Interfaces;

internal interface IActionItemDiceBox
{
    (RuleDefinitions.DieType type, int number, string format) GetDiceInfo(RulesetCharacter character);
}
