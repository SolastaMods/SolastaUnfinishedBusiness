using HarmonyLib;
using SolastaMulticlass.Models;

namespace SolastaMulticlass.Patches.GameUi
{
    internal static class CharactersStatsPanelPatcher
    {
        [HarmonyPatch(typeof(CharacterStatsPanel), "Refresh")]
        internal static class CharacterStatsPanelRefresh
        {
            internal static void Postfix(CharacterStatBox ___hitDiceBox, GuiCharacter ___guiCharacter)
            {
                if (___hitDiceBox.Activated)
                {
                    ___hitDiceBox.ValueLabel.Text = MulticlassGameUiContext.GetAllClassesHitDiceLabel(___guiCharacter, out var dieTypeCount);
                    ___hitDiceBox.ValueLabel.TMP_Text.fontSize = MulticlassGameUiContext.GetFontSize(dieTypeCount);
                }
            }
        }
    }
}
