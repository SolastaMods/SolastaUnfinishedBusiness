using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;

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

                output += Gui.Format("{0}: {1}", trend.value.ToString("+0;-#"), Gui.FormatTrendInfo(trend));
            }

            __result = output;

            return false;
        }
    }
}
