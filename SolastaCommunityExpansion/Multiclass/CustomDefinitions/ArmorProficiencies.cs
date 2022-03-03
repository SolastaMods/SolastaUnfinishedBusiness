using System.Collections.Generic;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionProficiencys;

namespace SolastaCommunityExpansion.Multiclass.CustomDefinitions
{
    internal sealed class ArmorProficiencyMulticlassBuilder : FeatureDefinitionProficiencyBuilder
    {
        private const string BarbarianArmorProficiencyMulticlassName = "BarbarianArmorProficiencyMulticlass";
        private const string BarbarianArmorProficiencyMulticlassGuid = "86558227b0cd4771b42978a60dc610db";

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
