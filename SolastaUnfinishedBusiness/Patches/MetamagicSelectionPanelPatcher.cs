using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

internal static class MetamagicSelectionPanelPatcher
{
    [HarmonyPatch(typeof(MetamagicSelectionPanel), "Unbind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Unbind_Patch
    {
        internal static void Prefix([NotNull] MetamagicSelectionPanel __instance)
        {
            //BUGFIX: leaked metamagic panel
            Gui.ReleaseChildrenToPool(__instance.metamagicOptionsTable.transform);
        }
    }
}
