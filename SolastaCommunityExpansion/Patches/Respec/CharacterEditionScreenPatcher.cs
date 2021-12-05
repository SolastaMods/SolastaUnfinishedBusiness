using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Respec
{
    // use this patch to track if Respec was aborted
    [HarmonyPatch(typeof(CharacterEditionScreen), "DoAbort")]
    internal static class CharacterEditionScreen_DoAbort
    {
        internal static void Postfix()
        {
            Functors.FunctorRespec.AbortRespec();
        }
    }
}
