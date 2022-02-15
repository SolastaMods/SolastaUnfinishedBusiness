using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionProficiencys;

namespace SolastaCommunityExpansion.Multiclass.CustomDefinitions
{
    //internal static class ArmorProficiencies
    //{
    //    internal static readonly FeatureDefinitionProficiency BarbarianArmorProficiencyMulticlass = new FeatureDefinitionProficiencyBuilder(
    //        "BarbarianArmorProficiencyMulticlass",
    //        "5dffec907a424fccbfec103344421b51",
    //        RuleDefinitions.ProficiencyType.Armor,
    //        new string[] { EquipmentDefinitions.ShieldCategory },
    //        new GuiPresentationBuilder("Feature/&BarbarianArmorProficiencyTitle").Build())
    //        .AddToDB();

    //    internal static readonly FeatureDefinitionProficiency FighterArmorProficiencyMulticlass = new FeatureDefinitionProficiencyBuilder(
    //        "FighterArmorProficiencyMulticlass",
    //        "5df5ec907a424fccbfec103344421b51",
    //        RuleDefinitions.ProficiencyType.Armor,
    //        new string[] { EquipmentDefinitions.LightArmorCategory, EquipmentDefinitions.MediumArmorCategory, EquipmentDefinitions.ShieldCategory },
    //        new GuiPresentationBuilder("Feature/&FighterArmorProficiencyTitle").Build())
    //        .AddToDB();

    //    internal static readonly FeatureDefinitionProficiency PaladinArmorProficiencyMulticlass = new FeatureDefinitionProficiencyBuilder(
    //        "PaladinArmorProficiencyMulticlass",
    //        "69b18e44aabd4acca702c05f9d6c7fcb",
    //        RuleDefinitions.ProficiencyType.Armor,
    //        new string[] { EquipmentDefinitions.LightArmorCategory, EquipmentDefinitions.MediumArmorCategory, EquipmentDefinitions.ShieldCategory },
    //        new GuiPresentationBuilder("Feature/&PaladinArmorProficiencyTitle").Build())
    //        .AddToDB();

    //    internal static readonly FeatureDefinitionProficiency WardenArmorProficiencyMulticlass = new FeatureDefinitionProficiencyBuilder(
    //        "WardenArmorProficiencyMulticlass",
    //        "19666e846975401b819d1ae72c5d27ac",
    //        RuleDefinitions.ProficiencyType.Armor,
    //        new string[] { EquipmentDefinitions.LightArmorCategory, EquipmentDefinitions.MediumArmorCategory, EquipmentDefinitions.ShieldCategory },
    //        new GuiPresentationBuilder("Feature/&WardenArmorProficiencyTitle").Build())
    //        .AddToDB();
    //}

    internal sealed class ArmorProficiencyMulticlassBuilder : DefinitionBuilder<FeatureDefinitionProficiency>
    {
        private const string BarbarianArmorProficiencyMulticlassName = "BarbarianArmorProficiencyMulticlass";
        private const string BarbarianArmorProficiencyMulticlassGuid = "5dffec907a424fccbfec103344421b51";

        private const string FighterArmorProficiencyMulticlassName = "FighterArmorProficiencyMulticlass";
        private const string FighterArmorProficiencyMulticlassGuid = "5df5ec907a424fccbfec103344421b51";

        private const string PaladinArmorProficiencyMulticlassName = "PaladinArmorProficiencyMulticlass";
        private const string PaladinArmorProficiencyMulticlassGuid = "69b18e44aabd4acca702c05f9d6c7fcb";

        private const string WardenArmorProficiencyMulticlassName = "WardenArmorProficiencyMulticlass";
        private const string WardenArmorProficiencyMulticlassGuid = "19666e846975401b819d1ae72c5d27ac";

        private ArmorProficiencyMulticlassBuilder(string name, string guid, string title, List<string> proficienciesToReplace) : base(ProficiencyFighterArmor, name, guid)
        {
            Definition.Proficiencies.SetRange(proficienciesToReplace);
            Definition.GuiPresentation.Title = title;
        }

        private static FeatureDefinitionProficiency CreateAndAddToDB(string name, string guid, string title, List<string> proficienciesToReplace)
        {
            return new ArmorProficiencyMulticlassBuilder(name, guid, title, proficienciesToReplace).AddToDB();
        }

        internal static readonly FeatureDefinitionProficiency BarbarianArmorProficiencyMulticlass =
            CreateAndAddToDB(BarbarianArmorProficiencyMulticlassName, BarbarianArmorProficiencyMulticlassGuid, "Feature/&BarbarianArmorProficiencyTitle", new List<string> {
                EquipmentDefinitions.ShieldCategory
            });

        internal static readonly FeatureDefinitionProficiency FighterArmorProficiencyMulticlass =
            CreateAndAddToDB(FighterArmorProficiencyMulticlassName, FighterArmorProficiencyMulticlassGuid, "Feature/&FighterArmorProficiencyTitle", new List<string> {
                EquipmentDefinitions.LightArmorCategory,
                EquipmentDefinitions.MediumArmorCategory,
                EquipmentDefinitions.ShieldCategory
            });

        internal static readonly FeatureDefinitionProficiency PaladinArmorProficiencyMulticlass =
            CreateAndAddToDB(PaladinArmorProficiencyMulticlassName, PaladinArmorProficiencyMulticlassGuid, "Feature/&PaladinArmorProficiencyTitle", new List<string> {
                EquipmentDefinitions.LightArmorCategory,
                EquipmentDefinitions.MediumArmorCategory,
                EquipmentDefinitions.ShieldCategory
            });

        internal static readonly FeatureDefinitionProficiency WardenArmorProficiencyMulticlass =
            CreateAndAddToDB(WardenArmorProficiencyMulticlassName, WardenArmorProficiencyMulticlassGuid, "Feature/&WardenArmorProficiencyTitle", new List<string> {
                EquipmentDefinitions.LightArmorCategory,
                EquipmentDefinitions.MediumArmorCategory,
                EquipmentDefinitions.ShieldCategory
            });
    }
}
