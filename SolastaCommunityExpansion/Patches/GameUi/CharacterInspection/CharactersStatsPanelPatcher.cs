using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection
{
    internal static class CharactersStatsPanelPatcher
    {
        [HarmonyPatch(typeof(CharacterStatsPanel), "Refresh")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class CharacterStatsPanel_Refresh
        {
            internal static void Postfix(CharacterStatBox ___hitDiceBox, GuiCharacter ___guiCharacter)
            {
                if (___hitDiceBox.Activated && ___guiCharacter.RulesetCharacterHero.ClassesAndLevels.Count > 1)
                {
                    ___hitDiceBox.ValueLabel.Text = MulticlassGameUiContext.GetAllClassesHitDiceLabel(___guiCharacter, out var dieTypeCount);
                    ___hitDiceBox.ValueLabel.TMP_Text.fontSize = MulticlassGameUiContext.GetFontSize(dieTypeCount);
                }
            }
        }
    }
}
