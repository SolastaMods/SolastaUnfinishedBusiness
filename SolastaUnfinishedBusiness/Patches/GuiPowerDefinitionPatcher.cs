using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class GuiPowerDefinitionPatcher
{
    [HarmonyPatch(typeof(GuiPowerDefinition), "EnumerateTags")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class EnumerateTags_Patch
    {
        public static void Postfix(GuiPowerDefinition __instance)
        {
            //PATCH: adds `Unfinished Business` tag to all CE powers
            CeContentPackContext.AddCeTag(__instance.BaseDefinition, __instance.TagsMap);
        }
    }

    [HarmonyPatch(typeof(GuiPowerDefinition), "EffectDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class EffectDescription_Getter_Patch
    {
        public static void Postfix(GuiPowerDefinition __instance, ref EffectDescription __result)
        {
            //PATCH: support for `ICustomMagicEffectBasedOnCaster` and `IModifySpellEffect` 
            // makes tooltips show modified effects
            __result = PowerBundle.ModifyMagicEffectGui(__result, __instance.PowerDefinition);
        }
    }
}
