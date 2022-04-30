using HarmonyLib;
using SolastaMulticlass.Models;

namespace SolastaMulticlass.Patches.LevelUp
{
    internal static class FeatureDescriptionItemPatcher
    {
        [HarmonyPatch(typeof(FeatureDescriptionItem), "Bind")]
        internal static class FeatureDescriptionItemBind
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
}
