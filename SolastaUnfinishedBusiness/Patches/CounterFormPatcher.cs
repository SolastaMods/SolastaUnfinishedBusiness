using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

public static class CounterFormPatcher
{
    [HarmonyPatch(typeof(CounterForm), "Copy")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Copy_Patch
    {
        public static void Postfix([NotNull] CounterForm __instance, CounterForm reference)
        {
            //BUGFIX: it's missing 1 attribute copy as of 1.4.16
            __instance.abilityToAdd = reference.abilityToAdd;
        }
    }
}
