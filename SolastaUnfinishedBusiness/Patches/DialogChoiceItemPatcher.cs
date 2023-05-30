using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class DialogChoiceItemPatcher
{
    //PATCH: better support for font on game UI
    [HarmonyPatch(typeof(DialogChoiceItem), nameof(DialogChoiceItem.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(DialogChoiceItem __instance)
        {
            __instance.labelHighlighter.TargetLabel.TMP_Text.overflowMode = TMPro.TextOverflowModes.Overflow;
        }
    }
}
