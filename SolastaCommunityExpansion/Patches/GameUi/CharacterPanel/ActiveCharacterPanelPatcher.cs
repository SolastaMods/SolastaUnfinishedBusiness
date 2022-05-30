using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomUI;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterPanel;

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

        var poolPrefab = __instance.sorceryPointsBox.gameObject;
        var concentrationPrefab = __instance.concentrationGroup.gameObject;
        var layout = __instance.transform.Find("RightLayout");

        // Hide all custom controls
        for (var i = 0; i < layout.childCount; i++)
        {
            var child = layout.GetChild(i);
            if (child.name.StartsWith("CustomPool(") || child.name.StartsWith("CustomConcentration("))
            {
                child.gameObject.SetActive(false);
            }
        }

        // setup/update relevant custom controls
        var pools = character.GetSubFeaturesByType<ICusomPortraitPointPoolProvider>();
        foreach (var provider in pools)
        {
            CusomPortraitPointPool.Setup(provider, character, poolPrefab, layout);
        }

        var concentrations = character.GetSubFeaturesByType<ICusomConcentrationProvider>();
        foreach (var provider in concentrations)
        {
            CustomConcentrationControl.Setup(provider, character, concentrationPrefab, layout);
        }
    }
}
