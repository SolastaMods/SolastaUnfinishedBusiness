using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection
{
    internal static class CharacterPlateDetailedPatcher
    {
        [HarmonyPatch(typeof(CharacterPlateDetailed), "OnPortraitShowed")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class CharacterPlateDetailed_OnPortraitShowed
        {
            internal static void Postfix(CharacterPlateDetailed __instance, GuiLabel ___classLabel)
            {
                int classesCount;
                char separator;
                var guiCharacter = __instance.GuiCharacter;

                if (guiCharacter.RulesetCharacterHero != null)
                {
                    separator = '\n';
                    classesCount = guiCharacter.RulesetCharacterHero.ClassesAndLevels.Count;
                }
                else
                {
                    separator = '\\';
                    classesCount = guiCharacter.Snapshot.Classes.Length;
                }

                ___classLabel.Text = MulticlassGameUiContext.GetAllClassesLabel(guiCharacter, separator) ?? ___classLabel.Text;
                ___classLabel.TMP_Text.fontSize = MulticlassGameUiContext.GetFontSize(classesCount);
            }
        }
    }
}
