using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomUI;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterPanel;

internal static class ActiveCharacterPanelPatcher
{
    [HarmonyPatch(typeof(ActiveCharacterPanel), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ActiveCharacterPanel_Refresh
    {
        internal static void Postfix(ActiveCharacterPanel __instance)
        {
            var character = __instance.GuiCharacter?.RulesetCharacter;
            if (character == null)
            {
                return;
            }

            var prefab = __instance.GetField<RectTransform>("sorceryPointsBox").gameObject;
            var layout = __instance.transform.Find("RightLayout");

            //Hide all custom
            for (var i = 0; i < layout.childCount; i++)
            {
                var child = layout.GetChild(i);
                if (child.name.StartsWith("CustomPool("))
                {
                    child.gameObject.SetActive(false);
                }
            }

            //display elevant custom
            var pools = character.GetSubFeaturesByType<ICusomPortraitPointPoolProvider>();
            foreach (var provider in pools)
            {
                CusomPortraitPointPool.Setup(provider, character, prefab, layout);
            }
        }
    }
}