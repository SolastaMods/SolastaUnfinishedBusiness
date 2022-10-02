using static CharacterClassDefinition;

namespace SolastaUnfinishedBusiness.Builders;

internal static class EquipmentOptionsBuilder
{
    internal static HeroEquipmentOption Option(ItemDefinition itemType, string optionType, int number)
    {
        var itemOption = new HeroEquipmentOption { number = number, optionType = optionType, itemReference = itemType };
        return itemOption;
    }

#if false
    internal static HeroEquipmentOption Option(string defaultChoice, string optionType, int number)
    {
        var itemOption =
            new HeroEquipmentOption { number = number, optionType = optionType, defaultChoice = defaultChoice };
        return itemOption;
    }

    internal static IEnumerable<HeroEquipmentOption> Column(params HeroEquipmentOption[] options)
    {
        return options.AsEnumerable();
    }
#endif
}
