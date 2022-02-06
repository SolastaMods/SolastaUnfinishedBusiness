using HarmonyLib;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Multiclass.Patches.HeroInspection
{
    internal static class CharactersStatsPanelPatcher
    {
        [HarmonyPatch(typeof(CharacterStatsPanel), "Refresh")]
        internal static class CharacterStatsPanelRefresh
        {
            internal static void Postfix(CharacterStatsPanel __instance)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                var hitDiceBox = __instance.GetField<CharacterStatsPanel, CharacterStatBox>("hitDiceBox");

                if (hitDiceBox.Activated)
                {
                    var guiCharacter = __instance.GetField<CharacterStatsPanel, GuiCharacter>("guiCharacter");

                    hitDiceBox.ValueLabel.Text = Models.GameUiContext.GetAllClassesHitDiceLabel(guiCharacter, out var dieTypeCount);
                    hitDiceBox.ValueLabel.TMP_Text.fontSize = Models.GameUiContext.GetFontSize(dieTypeCount);
                }
            }
        }
    }
}
