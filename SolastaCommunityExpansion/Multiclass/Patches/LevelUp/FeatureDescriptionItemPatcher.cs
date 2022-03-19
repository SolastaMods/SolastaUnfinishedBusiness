using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Multiclass.Models;
using SolastaModApi.Infrastructure;
using TMPro;

namespace SolastaCommunityExpansion.Multiclass.Patches.LevelUp
{
    internal static class FeatureDescriptionItemPatcher
    {
        [HarmonyPatch(typeof(FeatureDescriptionItem), "Bind")]
        internal static class FeatureDescriptionItemBind
        {
            public static void DisableDropdownIfMulticlass(FeatureDescriptionItem featureDescriptionItem)
            {
                var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();
                var hero = characterBuildingService.CurrentLocalHeroCharacter;
                var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);
                var isMulticlass = LevelUpContext.IsMulticlass(hero);

                if (!(isClassSelectionStage && isMulticlass))
                {
                    return;
                }

                var choiceDropdown = featureDescriptionItem.GetField<FeatureDescriptionItem, GuiDropdown>("choiceDropdown");

                choiceDropdown.gameObject.SetActive(false);
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    foreach (var instruction in instructions)
                    {
                        yield return instruction;
                    }

                    yield break;
                }

                var setValueMethod = typeof(TMP_Dropdown).GetMethod("set_value");
                var disableDropdownIfMulticlassMethod = typeof(FeatureDescriptionItemBind).GetMethod("DisableDropdownIfMulticlass");

                foreach (var instruction in instructions)
                {
                    yield return instruction;

                    if (instruction.Calls(setValueMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Call, disableDropdownIfMulticlassMethod);
                    }
                }
            }
        }
    }
}
