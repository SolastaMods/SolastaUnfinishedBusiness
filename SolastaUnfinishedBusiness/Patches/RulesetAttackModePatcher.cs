using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

public static class RulesetAttackModePatcher
{
    [HarmonyPatch(typeof(RulesetAttackMode), "Copy")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Copy_Patch
    {
        public static void Postfix([NotNull] RulesetAttackMode __instance, RulesetAttackMode reference)
        {
            //BUGFIX: it's missing 3 attribute copies as of 1.4.16
            __instance.description = reference.description;
            __instance.formsDescription = reference.formsDescription;
            __instance.accountedProviders.AddRange(reference.accountedProviders);
        }
    }
}
