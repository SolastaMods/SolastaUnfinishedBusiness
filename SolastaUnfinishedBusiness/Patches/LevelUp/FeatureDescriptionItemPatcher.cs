using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches.LevelUp;

//PATCH: Disables choices dropdown for features already taken on previous levels (MULTICLASS)
[HarmonyPatch(typeof(FeatureDescriptionItem), "Bind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class FeatureDescriptionItem_Bind
{
    public static void Postfix([NotNull] FeatureDescriptionItem __instance)
    {
        var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();
        var currentLocalHeroCharacter = characterBuildingService.CurrentLocalHeroCharacter;
        var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(currentLocalHeroCharacter);

        if (!isClassSelectionStage)
        {
            return;
        }

        __instance.choiceDropdown.gameObject.SetActive(false);
    }
}
