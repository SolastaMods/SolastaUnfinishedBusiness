using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomSpecificBehaviors;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class InvocationSelectionPanelPatcher
{
    [HarmonyPatch(typeof(InvocationSelectionPanel), nameof(InvocationSelectionPanel.OnInvocationSelected))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnInvocationSelected_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(InvocationSelectionPanel __instance, InvocationActivationBox invocationActivationBox)
        {
            //PATCH: used by Power Bundles feature
            //if the activated invocation is a power bundle, this tries to replace activation with sub-power selector and
            //then activates bundled power according to selected subspell.
            //returns false and skips base method if it does
            return PowerBundle.InvocationPowerActivated(invocationActivationBox, __instance.InvocationSelected);
        }
    }
}
