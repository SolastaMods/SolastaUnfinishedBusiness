using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

public static class DivinationFormPatcher
{
    [HarmonyPatch(typeof(DivinationForm), "Copy")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Copy_Patch
    {
        public static void Postfix([NotNull] DivinationForm __instance, DivinationForm reference)
        {
            //BUGFIX: it's missing 1 attribute copy as of 1.4.16
            __instance.silent = reference.silent;
        }
    }
}
