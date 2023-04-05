using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RuleDefinitionsPatcher
{
    [HarmonyPatch(typeof(RuleDefinitions), nameof(RuleDefinitions.ComputeAdvantage))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeAdvantage_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            [NotNull] List<RuleDefinitions.TrendInfo> trends,
            ref RuleDefinitions.AdvantageType __result)
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
                __result = RuleDefinitions.AdvantageType.None;
            }
            else if (hasAdvantage)
            {
                __result = RuleDefinitions.AdvantageType.Advantage;
            }
            else
            {
                __result = RuleDefinitions.AdvantageType.Disadvantage;
            }
        }
    }

    //PATCH: allows tweaking effects on heroes with extra ancestry types
    [HarmonyPatch(typeof(RuleDefinitions), nameof(RuleDefinitions.TryGetAncestryDamageTypeFromCharacter))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TryGetAncestryDamageTypeFromCharacter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            ref bool __result,
            ulong guid,
            RuleDefinitions.AncestryType ancestryType,
            ref string ancestryDamageType)
        {
            // reuse any Barbarian Claw effect with Path of the Elements
            if (!__result && ancestryType == RuleDefinitions.AncestryType.BarbarianClaw)
            {
                __result = RuleDefinitions.TryGetAncestryDamageTypeFromCharacter(
                    guid, (RuleDefinitions.AncestryType)ExtraAncestryType.PathOfTheElements, out ancestryDamageType);
            }
        }
    }
}
