using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.BehaviorsGeneric;
using SolastaUnfinishedBusiness.BehaviorsSpecific;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class AfterRestActionItemPatcher
{
    [HarmonyPatch(typeof(AfterRestActionItem), nameof(AfterRestActionItem.OnExecuteCb))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnExecuteCb_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(AfterRestActionItem __instance)
        {
            //PATCH: replaces callback execution for bundled powers to show sub-power selection
            return PowerBundle.ExecuteAfterRestCb(__instance);
        }
    }

    [HarmonyPatch(typeof(AfterRestActionItem), nameof(AfterRestActionItem.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(AfterRestActionItem __instance)
        {
            var activity = __instance.RestActivityDefinition;
            var hero = __instance.Hero;

            if (activity.functor != PowerBundleContext.UseCustomRestPowerFunctorName)
            {
                return;
            }

            var power = hero.UsablePowers.FirstOrDefault(usablePower =>
                usablePower.PowerDefinition.Name == activity.StringParameter);

            if (power == null)
            {
                return;
            }

            //PATCH: use power tooltip for custom use power functors
            ServiceRepository.GetService<IGuiWrapperService>()
                .GetGuiPowerDefinition(power.PowerDefinition.Name)
                .SetupTooltip(__instance.GuiTooltip, hero);

            //PATCH: allow customized titles on use rest power
            var getTitle = power.PowerDefinition.GetFirstSubFeatureOfType<ModifyRestPowerTitleHandler>();

            if (getTitle != null)
            {
                __instance.titleLabel.Text = getTitle(hero);
            }
        }
    }
}
