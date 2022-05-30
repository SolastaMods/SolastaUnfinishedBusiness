using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;
using static CharacterClassDefinition;

namespace SolastaCommunityExpansion.Builders;

public static class EquipmentOptionsBuilder
{
    public static HeroEquipmentOption Option(ItemDefinition itemType, string optionType, int number)
    {
        var itemOption = new HeroEquipmentOption();
        itemOption.SetNumber(number);
        itemOption.SetOptionType(optionType);
        itemOption.SetItemDefinition(itemType);
        return itemOption;
    }

    public static HeroEquipmentOption Option(string defaultChoice, string optionType, int number)
    {
        var itemOption = new HeroEquipmentOption();
        itemOption.SetNumber(number);
        itemOption.SetOptionType(optionType);
        itemOption.SetDefaultChoice(defaultChoice);
        return itemOption;
    }

    public static IEnumerable<HeroEquipmentOption> Column(params HeroEquipmentOption[] options)
    {
        return options.AsEnumerable();
    }
}
