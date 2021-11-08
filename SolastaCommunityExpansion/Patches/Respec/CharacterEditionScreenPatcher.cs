using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    // use this patch to track if Respec was aborted
    [HarmonyPatch(typeof(CharacterEditionScreen), "DoAbort")]
    internal static class CharacterEditionScreen_DoAbort
    {
        internal static void Postfix()
        {
            // should not check if Respec is enabled here otherwise users that disable respec in Mod UI while doing one and cancelling will get in big, big trouble ;-)
            Functors.FunctorRespec.AbortRespec();
        }
    }
}