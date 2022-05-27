using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.CustomUI;
using SolastaModApi;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterPanel
{
    [HarmonyPatch(typeof(CharacterActionPanel), "RefreshActions")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterActionPanel_RefreshActions
    {
        internal static void Postfix(CharacterActionPanel __instance)
        {
            ExtraAttacksOnActionPanel.AddExtraAttakItems(__instance);
        }
    }
}
