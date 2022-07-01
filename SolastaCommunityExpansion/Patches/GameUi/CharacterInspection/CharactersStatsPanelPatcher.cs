using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection;

[HarmonyPatch(typeof(CharacterStatsPanel), "Refresh")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterStatsPanel_Refresh
{
    internal static void Postfix(CharacterStatsPanel __instance)
    {
        if (!__instance.hitDiceBox.Activated ||
            __instance.guiCharacter.RulesetCharacterHero.ClassesAndLevels.Count <= 1)
        {
            return;
        }

        __instance.hitDiceBox.ValueLabel.Text =
            MulticlassGameUiContext.GetAllClassesHitDiceLabel(__instance.guiCharacter, out var dieTypeCount);
        __instance.hitDiceBox.ValueLabel.TMP_Text.fontSize = MulticlassGameUiContext.GetFontSize(dieTypeCount);
    }
}
