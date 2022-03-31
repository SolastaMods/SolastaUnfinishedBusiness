using System;
using SolastaModApi;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionProficiencys;

namespace SolastaMulticlass.CustomDefinitions
{
    internal sealed class ArmorProficiencyMulticlassBuilder : BaseDefinitionBuilder<FeatureDefinitionProficiency>
    {
        private const string BarbarianArmorProficiencyMulticlassName = "BarbarianArmorProficiencyMulticlass";
        private const string BarbarianArmorProficiencyMulticlassGuid = "86558227b0cd4771b42978a60dc610db";

        private const string FighterArmorProficiencyMulticlassName = "FighterArmorProficiencyMulticlass";
        private const string FighterArmorProficiencyMulticlassGuid = "5df5ec907a424fccbfec103344421b51";

        private const string PaladinArmorProficiencyMulticlassName = "PaladinArmorProficiencyMulticlass";
        private const string PaladinArmorProficiencyMulticlassGuid = "69b18e44aabd4acca702c05f9d6c7fcb";

        private const string WardenArmorProficiencyMulticlassName = "WardenArmorProficiencyMulticlass";
        private const string WardenArmorProficiencyMulticlassGuid = "19666e846975401b819d1ae72c5d27ac";

        [Obsolete]
        private ArmorProficiencyMulticlassBuilder(string name, string guid, string title, params string[] proficienciesToReplace) : base(ProficiencyFighterArmor, name, guid)
        {
            Definition.Proficiencies.SetRange(proficienciesToReplace);
            Definition.GuiPresentation.Title = title;
        }

        [Obsolete]
        private static FeatureDefinitionProficiency CreateAndAddToDB(string name, string guid, string title, params string[] proficienciesToReplace)
        {
            return new ArmorProficiencyMulticlassBuilder(name, guid, title, proficienciesToReplace).AddToDB();
        }

        [Obsolete]
        internal static readonly FeatureDefinitionProficiency BarbarianArmorProficiencyMulticlass =
            CreateAndAddToDB(BarbarianArmorProficiencyMulticlassName, BarbarianArmorProficiencyMulticlassGuid, "Feature/&BarbarianArmorProficiencyTitle",
                EquipmentDefinitions.ShieldCategory
            );
        [Obsolete]
        internal static readonly FeatureDefinitionProficiency FighterArmorProficiencyMulticlass =
            CreateAndAddToDB(FighterArmorProficiencyMulticlassName, FighterArmorProficiencyMulticlassGuid, "Feature/&FighterArmorProficiencyTitle",
                EquipmentDefinitions.LightArmorCategory,
                EquipmentDefinitions.MediumArmorCategory,
                EquipmentDefinitions.ShieldCategory
            );
        [Obsolete]
        internal static readonly FeatureDefinitionProficiency PaladinArmorProficiencyMulticlass =
            CreateAndAddToDB(PaladinArmorProficiencyMulticlassName, PaladinArmorProficiencyMulticlassGuid, "Feature/&PaladinArmorProficiencyTitle",
                EquipmentDefinitions.LightArmorCategory,
                EquipmentDefinitions.MediumArmorCategory,
                EquipmentDefinitions.ShieldCategory
            );
        [Obsolete]
        internal static readonly FeatureDefinitionProficiency WardenArmorProficiencyMulticlass =
            CreateAndAddToDB(WardenArmorProficiencyMulticlassName, WardenArmorProficiencyMulticlassGuid, "Feature/&WardenArmorProficiencyTitle",
                EquipmentDefinitions.LightArmorCategory,
                EquipmentDefinitions.MediumArmorCategory,
                EquipmentDefinitions.ShieldCategory
            );
    }
}
