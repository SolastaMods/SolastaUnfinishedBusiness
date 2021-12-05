using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    // use this patch to track if Respec was aborted
    [HarmonyPatch(typeof(CharacterEditionScreen), "DoAbort")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterEditionScreen_DoAbort
    {
        internal static void Postfix()
        {
            Functors.FunctorRespec.AbortRespec();
        }
    }
}
