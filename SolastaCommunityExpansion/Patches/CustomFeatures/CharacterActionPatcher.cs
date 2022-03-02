using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static SolastaCommunityExpansion.Models.ModContext;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    [HarmonyPatch(typeof(CharacterAction), "Execute")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterAction_Execute
    {
        public static CharacterActionExecute Handler { get; set; }

        internal static IEnumerator Postfix(IEnumerator values, CharacterAction __instance)
        {
            Handler?.Invoke(
                ref __instance,
                isPrefix: true);

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            Handler?.Invoke(
                ref __instance,
                isPrefix: false);
        }
    }
}
