using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Diagnostic
{
    // prevents TA trying to sync mod content over
    [HarmonyPatch(typeof(UserContent), "CheckContentPackAvailability")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class UserContent_CheckContentPackAvailability
    {
        public static bool Prefix(BaseDefinition baseDefinition, ref bool __result)
        {
            if (baseDefinition.ContentPack == CeContentPackContext.CeContentPack)
            {
                __result = true;

                return false;
            }

            return true;
        }
    }
}
