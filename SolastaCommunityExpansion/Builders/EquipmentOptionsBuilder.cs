using System.Collections.Generic;
using System.Linq;
using static CharacterClassDefinition;

namespace SolastaCommunityExpansion.Builders;

public static class EquipmentOptionsBuilder
{
    public static HeroEquipmentOption Option(ItemDefinition itemType, string optionType, int number)
    {
        var itemOption = new HeroEquipmentOption {number = number, optionType = optionType, itemReference = itemType};
        return itemOption;
    }

    public static HeroEquipmentOption Option(string defaultChoice, string optionType, int number)
    {
        var itemOption =
            new HeroEquipmentOption {number = number, optionType = optionType, defaultChoice = defaultChoice};
        return itemOption;
    }

    public static IEnumerable<HeroEquipmentOption> Column(params HeroEquipmentOption[] options)
    {
        return options.AsEnumerable();
    }
}
