using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GuiItemDefinitionPatcher
{
    [HarmonyPatch(typeof(GuiItemDefinition), nameof(GuiItemDefinition.EnumerateTags))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class EnumerateTags_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GuiItemDefinition __instance)
        {
            var tags = __instance.itemTags;
            var item = __instance.ItemDefinition;

            //PATCH: adds `Polearm` tag to appropriate weapons
            CustomWeaponsContext.AddPolearmWeaponTag(item, tags);

            //PATCH: adds `Unfinished Business` tag to all CE items
            CeContentPackContext.AddCeTag(item, tags);
        }
    }
}
