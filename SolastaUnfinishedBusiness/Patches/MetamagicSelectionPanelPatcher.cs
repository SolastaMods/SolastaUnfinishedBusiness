using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

public static class MetamagicSelectionPanelPatcher
{
    [HarmonyPatch(typeof(MetamagicSelectionPanel), "Unbind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Unbind_Patch
    {
        public static void Prefix([NotNull] MetamagicSelectionPanel __instance)
        {
            //BUGFIX: leaked metamagic panel
            Gui.ReleaseChildrenToPool(__instance.metamagicOptionsTable.transform);
        }
    }
}
