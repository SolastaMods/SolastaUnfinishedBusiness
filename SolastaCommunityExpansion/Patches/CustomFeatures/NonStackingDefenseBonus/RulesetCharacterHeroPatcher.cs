using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.NonStackingDefenseBonus
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "RefreshArmorClass")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_RefreshArmorClass
    {
        internal static void Postfix(RulesetCharacterHero __instance)
        {
            if (!Main.Settings.EnableNonStackingDefenseFromAttributeModifiers)
            {
                return;
            }

            var armorClassAttribute = __instance.GetAttribute(AttributeDefinitions.ArmorClass);
            RulesetAttributeModifier modifierToKeep = null;

            // get a tab on the best AddAbilityScoreBonus modifier
            foreach (var modifier in armorClassAttribute.ActiveModifiers.Where(x => x.Operation == FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddAbilityScoreBonus))
            {
                if (modifierToKeep == null || modifierToKeep.Value < modifier.Value)
                {
                    modifierToKeep = modifier;
                }
            }

            // remove all other AddAbilityScoreBonus modifiers
            armorClassAttribute.ActiveModifiers
                .RemoveAll(x =>
                    x.Operation == FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddAbilityScoreBonus
                    && x != modifierToKeep);
        }
    }
}
