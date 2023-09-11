using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
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

    //PATCH: allows tweaking effects on heroes with extra ancestry types
    [HarmonyPatch(typeof(RuleDefinitions), nameof(TryGetAncestryDamageTypeFromCharacter))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TryGetAncestryDamageTypeFromCharacter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            ref bool __result,
            ulong guid,
            AncestryType ancestryType,
            ref string ancestryDamageType)
        {
            // reuse any Barbarian Claw effect with Path of the Elements
            if (!__result && ancestryType == AncestryType.BarbarianClaw)
            {
                __result = TryGetAncestryDamageTypeFromCharacter(
                    guid, (AncestryType)ExtraAncestryType.PathOfTheElements, out ancestryDamageType);
            }
        }
    }
}
