using HarmonyLib;
using SolastaCommunityExpansion;
using SolastaMulticlass.Models;

namespace SolastaMulticlass.Patches.GameUi
{
    internal static class CharacterPlateDetailedPatcher
    {
        [HarmonyPatch(typeof(CharacterPlateDetailed), "OnPortraitShowed")]
        internal static class CharacterPlateDetailedOnPortraitShowed
        {
            internal static void Postfix(CharacterPlateDetailed __instance, GuiLabel ___classLabel)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

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
