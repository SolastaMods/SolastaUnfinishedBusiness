using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomSpecificBehaviors;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GuiPowerDefinitionPatcher
{
    [HarmonyPatch(typeof(GuiPowerDefinition), nameof(GuiPowerDefinition.EnumerateTags))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class EnumerateTags_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GuiPowerDefinition __instance)
        {
            //PATCH: adds `Unfinished Business` tag to all CE powers
            CeContentPackContext.AddCeTag(__instance.BaseDefinition, __instance.TagsMap);
        }
    }

    [HarmonyPatch(typeof(GuiPowerDefinition), nameof(GuiPowerDefinition.EffectDescription), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class EffectDescription_Getter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GuiPowerDefinition __instance, ref EffectDescription __result)
        {
            //PATCH: support for `ICustomMagicEffectBasedOnCaster` and `IModifySpellEffect` 
            // makes tooltips show modified effects
            __result = PowerBundle.ModifyMagicEffectGui(__result, __instance.PowerDefinition);
        }
    }
}
