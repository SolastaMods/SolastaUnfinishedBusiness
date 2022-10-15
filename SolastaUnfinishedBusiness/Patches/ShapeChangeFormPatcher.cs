using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

public static class ShapeChangeFormPatcher
{
    [HarmonyPatch(typeof(ShapeChangeForm), "Copy")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Copy_Patch
    {
        public static void Postfix([NotNull] ShapeChangeForm __instance, ShapeChangeForm reference)
        {
            //BUGFIX: it's missing 1 attribute copy as of 1.4.16
            __instance.specialSubstituteCondition = reference.specialSubstituteCondition;
        }
    }
}
