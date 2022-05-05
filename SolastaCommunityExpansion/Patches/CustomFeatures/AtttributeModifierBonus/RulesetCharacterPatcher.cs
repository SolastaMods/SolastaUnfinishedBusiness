using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.AtttributeModifierBonus
{
    // non stacked AC
    [HarmonyPatch(typeof(RulesetCharacter), "SortArmorClassModifierTrends")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_RefreshArmorClass
    {
        public static void Prefix(RulesetAttribute armorClassAttribute)
        {
            var activeModifiers = armorClassAttribute.ActiveModifiers
                .Where(x => x.Tags.Contains(AttributeDefinitions.TagClass) 
                    || x.Tags.Contains(AttributeDefinitions.TagFeat));

            if (!activeModifiers.Any())
            {
                return;
            }

            RulesetAttributeModifier maxModifier = new RulesetAttributeModifier();

            foreach (var modifier in activeModifiers)
            {
                if (modifier.Value > maxModifier.Value)
                {
                    maxModifier = modifier;
                }
            }

            armorClassAttribute.ActiveModifiers
                .RemoveAll(x => x.Tags.Contains(AttributeDefinitions.TagClass)
                    || x.Tags.Contains(AttributeDefinitions.TagFeat));
            armorClassAttribute.ActiveModifiers.Add(maxModifier);
        }
    }
}
