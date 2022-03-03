using HarmonyLib;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Multiclass.Patches.HeroInspection
{
    internal static class CharacterPlateDetailedPatcher
    {
        [HarmonyPatch(typeof(CharacterPlateDetailed), "OnPortraitShowed")]
        internal static class CharacterPlateDetailedOnPortraitShowed
        {
            internal static void Postfix(CharacterPlateDetailed __instance)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                int classesCount;
                char separator;

                if (__instance.GuiCharacter.RulesetCharacterHero != null)
                {
                    separator = '\n';
                    classesCount = __instance.GuiCharacter.RulesetCharacterHero.ClassesAndLevels.Count;
                }
                else
                {
                    separator = '\\';
                    classesCount = __instance.GuiCharacter.Snapshot.Classes.Length;
                }

                var classLabel = __instance.GetField<CharacterPlateDetailed, GuiLabel>("classLabel");

                classLabel.Text = Models.MulticlassGameUiContext.GetAllClassesLabel(__instance.GuiCharacter, separator) ?? classLabel.Text;
                classLabel.TMP_Text.fontSize = Models.MulticlassGameUiContext.GetFontSize(classesCount);
            }
        }
    }
}
