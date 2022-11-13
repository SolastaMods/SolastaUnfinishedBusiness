using static CharacterClassDefinition;

namespace SolastaUnfinishedBusiness.Builders;

internal static class EquipmentOptionsBuilder
{
    internal static HeroEquipmentOption Option(ItemDefinition itemType, string optionType, int number)
    {
        var itemOption = new HeroEquipmentOption
        {
            number = number, optionType = optionType, itemReference = itemType, defaultChoice = itemType.Name
        };

        return itemOption;
    }
}
