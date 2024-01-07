using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RuleDefinitionsPatcher
{
    [HarmonyPatch(typeof(RuleDefinitions), nameof(ComputeAdvantage))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeAdvantage_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            [NotNull] List<TrendInfo> trends,
            ref AdvantageType __result)
        {
            //PATCH: Apply SRD setting `UseOfficialAdvantageDisadvantageRules`
            if (!Main.Settings.UseOfficialAdvantageDisadvantageRules)
            {
                return;
            }

            var hasAdvantage = trends.Any(t => t.value > 0);
            var hasDisadvantage = trends.Any(t => t.value < 0);

            if (!(hasAdvantage ^ hasDisadvantage))
            {
                __result = AdvantageType.None;
            }
            else if (hasAdvantage)
            {
                __result = AdvantageType.Advantage;
            }
            else
            {
                __result = AdvantageType.Disadvantage;
            }
        }
    }
}
