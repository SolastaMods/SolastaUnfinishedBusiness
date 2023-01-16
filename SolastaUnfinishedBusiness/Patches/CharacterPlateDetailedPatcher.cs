using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterPlateDetailedPatcher
{
    //PATCH: Add classes progression to hero tooltip (MULTICLASS)
    [HarmonyPatch(typeof(CharacterPlateDetailed), nameof(CharacterPlateDetailed.OnPortraitShowed))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnPortraitShowed_Patch
    {
        [UsedImplicitly]
        public static void Postfix(CharacterPlateDetailed __instance)
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

            __instance.classLabel.Text = MulticlassGameUiContext.GetAllClassesLabel(guiCharacter, separator) ??
                                         __instance.classLabel.Text;
            __instance.classLabel.TMP_Text.fontSize = MulticlassGameUiContext.GetFontSize(classesCount);
        }
    }
}
