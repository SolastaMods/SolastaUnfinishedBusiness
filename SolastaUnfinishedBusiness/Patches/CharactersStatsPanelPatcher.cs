using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterStatsPanelPatcher
{
    [HarmonyPatch(typeof(CharacterStatsPanel), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Refresh_Patch
    {
        [UsedImplicitly]
        public static void Postfix(CharacterStatsPanel __instance)
        {
            //PATCH: Format hit dice box to support MC scenarios (MULTICLASS)
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
}
