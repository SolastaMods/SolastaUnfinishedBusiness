using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Respec
{
    // use this patch to track if Respec was aborted
    //
    // this patch shouldn't be protected
    //
    [HarmonyPatch(typeof(CharacterEditionScreen), "DoAbort")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterEditionScreen_DoAbort
    {
        internal static void Postfix()
        {
            Models.RespecContext.FunctorRespec.AbortRespec();
        }
    }
}
