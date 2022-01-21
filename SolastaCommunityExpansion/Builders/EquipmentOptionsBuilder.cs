using SolastaModApi.Infrastructure;
using static CharacterClassDefinition;

namespace SolastaCommunityExpansion.Builders
{
    public static class EquipmentOptionsBuilder
    {
        public static HeroEquipmentOption Option(ItemDefinition itemType, string optionType, int number)
        {
            var itemOption = new HeroEquipmentOption();
            itemOption.SetField("number", number);
            itemOption.SetField("optionType", optionType);
            itemOption.SetField("itemReference", itemType);
            return itemOption;
        }

        public static HeroEquipmentOption Option(string defaultChoice, string optionType, int number)
        {
            var itemOption = new HeroEquipmentOption();
            itemOption.SetField("number", number);
            itemOption.SetField("optionType", optionType);
            itemOption.SetField("defaultChoice", defaultChoice);
            return itemOption;
        }
    }
}
