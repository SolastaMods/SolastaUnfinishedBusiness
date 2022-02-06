using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
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
                var hero = characterBuildingService.HeroCharacter;
                var choiceDropdown = featureDescriptionItem.GetField<FeatureDescriptionItem, GuiDropdown>("choiceDropdown");

                if (Models.LevelUpContext.LevelingUp && Models.LevelUpContext.DisplayingClassPanel && hero.ClassesAndLevels.ContainsKey(Models.LevelUpContext.SelectedClass))
                {
                    var featureDefinitionFeatureSet = featureDescriptionItem.Feature as FeatureDefinitionFeatureSet;
                    var featureDefinitions = new List<FeatureDefinition>();

                    foreach (var activeFeature in characterBuildingService.HeroCharacter.ActiveFeatures.Where(x => x.Key.StartsWith(AttributeDefinitions.TagClass)))
                    {
                        featureDefinitions.AddRange(activeFeature.Value);
                    }

                    for (var index = 0; index < featureDefinitionFeatureSet.FeatureSet.Count; ++index)
                    {
                        if (featureDefinitions.Contains(featureDefinitionFeatureSet.FeatureSet[index]))
                        {
                            choiceDropdown.value = index;
                        }
                    }

                    choiceDropdown.interactable = false;
                }
                else
                {
                    choiceDropdown.interactable = true;
                }
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
                var getLastAssignedClassAndLevelCustomMethod = typeof(FeatureDescriptionItemBind).GetMethod("DisableDropdownIfMulticlass");

                foreach (var instruction in instructions)
                {
                    yield return instruction;

                    if (instruction.Calls(setValueMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Call, getLastAssignedClassAndLevelCustomMethod);
                    }
                }
            }
        }
    }
}
