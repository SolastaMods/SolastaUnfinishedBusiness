using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomSpecificBehaviors;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GuiSpellDefinitionPatcher
{
    [HarmonyPatch(typeof(GuiSpellDefinition), nameof(GuiSpellDefinition.EnumerateTags))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class EnumerateTags_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GuiSpellDefinition __instance)
        {
            //PATCH: adds `Unfinished Business` tag to all CE spells
            CeContentPackContext.AddCeTag(__instance.SpellDefinition, __instance.TagsMap);
        }
    }

    [HarmonyPatch(typeof(GuiSpellDefinition), nameof(GuiSpellDefinition.EffectDescription), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class EffectDescription_Getter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GuiSpellDefinition __instance, ref EffectDescription __result)
        {
            //PATCH: support for ICustomMagicEffectBasedOnCaster allowing to pick spell effect for GUI depending on caster properties
            __result = PowerBundle.ModifyMagicEffectGui(__result, __instance.SpellDefinition);
        }
    }
}
