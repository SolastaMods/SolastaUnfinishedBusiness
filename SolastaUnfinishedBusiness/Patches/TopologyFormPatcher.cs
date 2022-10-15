using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

public static class TopologyFormPatcher
{
    [HarmonyPatch(typeof(TopologyForm), "Copy")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Copy_Patch
    {
        public static void Postfix([NotNull] TopologyForm __instance, TopologyForm reference)
        {
            //BUGFIX: it's missing 1 attribute copy as of 1.4.16
            __instance.impactsFlyingCharacters = reference.impactsFlyingCharacters;
        }
    }
}
