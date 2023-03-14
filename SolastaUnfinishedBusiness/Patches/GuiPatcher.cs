using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using static FeatureDefinitionAttributeModifier;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GuiPatcher
{
    //PATCH: always displays a sign on attribute modifiers
    [HarmonyPatch(typeof(Gui), nameof(Gui.FormatTrendsList))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class FormatTrendsList_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            ref string __result,
            string output,
            List<RuleDefinitions.TrendInfo> trends,
            bool ignoreZero)
        {
            foreach (var trend in trends
                         .Where(trend => trend.value != 0 || !ignoreZero))
            {
                if (!string.IsNullOrEmpty(output))
                {
                    output += "\n";
                }

                var fixAdditive =
                    trend.attributeModifier?.operation is AttributeModifierOperation.Additive or
                        AttributeModifierOperation.AddConditionAmount or
                        AttributeModifierOperation.AddProficiencyBonus or
                        AttributeModifierOperation.AddSurroundingEnemies or
                        AttributeModifierOperation.AddAbilityScoreBonus or
                        AttributeModifierOperation.AddHalfProficiencyBonus;

                var value = fixAdditive || trend.additive ? trend.value.ToString("+0;-#") : trend.value.ToString();

                output += Gui.Format("{0}: {1}", value, Gui.FormatTrendInfo(trend));
            }

            __result = output;

            return false;
        }
    }
}
