using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class NotificationItemPatcher
{
    //PATCH: better support for font on game UI
    [HarmonyPatch(typeof(NotificationItem), nameof(NotificationItem.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(NotificationItem __instance)
        {
            __instance.titleLabel.TMP_Text.autoSizeTextContainer = true;
            __instance.descriptionLabel.TMP_Text.autoSizeTextContainer = true;
        }
    }
}
