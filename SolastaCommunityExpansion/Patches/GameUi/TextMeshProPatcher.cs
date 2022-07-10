using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Infrastructure;
using TMPro;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi;

[HarmonyPatch(typeof(TextMeshProUGUI), "Awake")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class TextMeshProUGUI_Awake
{
    internal static void Postfix(TextMeshProUGUI __instance)
    {
        var rectTransform = __instance.GetField<RectTransform>("m_rectTransform");
        var scale = Main.Settings.ScaleGameFontSizeBy / 100f;

        rectTransform.localScale = new Vector3(scale, scale, scale);
    }
}
