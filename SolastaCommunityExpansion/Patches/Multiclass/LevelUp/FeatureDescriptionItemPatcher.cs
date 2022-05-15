using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Multiclass.LevelUp
{
    [HarmonyPatch(typeof(FeatureDescriptionItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FeatureDescriptionItem_Bind
    {
        public static void Postfix(GuiDropdown ___choiceDropdown)
        {
            var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();
            var currentLocalHeroCharacter = characterBuildingService.CurrentLocalHeroCharacter;
            var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(currentLocalHeroCharacter);

            if (!isClassSelectionStage)
            {
                return;
            }

            //___choiceDropdown.enabled = false;
            ___choiceDropdown.gameObject.SetActive(false);
        }
    }
}
