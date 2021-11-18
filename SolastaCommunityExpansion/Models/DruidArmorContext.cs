using SolastaModApi;

namespace SolastaCommunityExpansion.Models
{
    internal static class DruidArmorContext
    {
        internal static void Switch(bool active)
        {
            if (active)
            {
                DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyDruidArmor.ForbiddenItemTags.Clear();
            }
            else
            {
                if (!DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyDruidArmor.ForbiddenItemTags.Contains(TagsDefinitions.ItemTagMetal))
                {
                    DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyDruidArmor.ForbiddenItemTags.Add(TagsDefinitions.ItemTagMetal);
                }
            }
        }

        internal static void Load()
        {
            Switch(Main.Settings.DruidNoMetalRestriction);
        }
    }
}
