using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class AfterRestActionItemPatcher
{
    [HarmonyPatch(typeof(AfterRestActionItem), "OnExecuteCb")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnExecuteCb_Patch
    {
        public static bool Prefix(AfterRestActionItem __instance)
        {
            //PATCH: replaces callback execution for bundled powers to show sub-power selection
            return PowersBundleContext.ExecuteAfterRestCb(__instance);
        }
    }

    [HarmonyPatch(typeof(AfterRestActionItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Bind_Patch
    {
        public static void Postfix(AfterRestActionItem __instance)
        {
            var activity = __instance.RestActivityDefinition;
            var hero = __instance.Hero;

            if (activity.functor != PowersBundleContext.UseCustomRestPowerFunctorName) { return; }

            var power = hero.UsablePowers.FirstOrDefault(usablePower =>
                usablePower.PowerDefinition.Name == activity.StringParameter);

            if (power == null) { return; }

            ServiceRepository.GetService<IGuiWrapperService>()
                .GetGuiPowerDefinition(power.PowerDefinition.Name)
                .SetupTooltip(__instance.GuiTooltip, hero);
        }
    }
}
