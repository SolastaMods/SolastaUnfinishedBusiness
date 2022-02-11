
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Multiclass.Patches.LevelUp
{
    internal static class CharacterStageClassSelectionPanelPatcher
    {
        // flags displaying the class panel
        [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "OnBeginShow")]
        internal static class CharacterStageClassSelectionPanelOnBeginShow
        {
            internal static void Prefix(CharacterStageClassSelectionPanel __instance, ref int ___selectedClass)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (Models.LevelUpContext.LevelingUp)
                {
                    var compatibleClasses = __instance.GetField<CharacterStageClassSelectionPanel, List<CharacterClassDefinition>>("compatibleClasses");

                    Models.LevelUpContext.DisplayingClassPanel = true;
                    Models.InOutRulesContext.EnumerateHeroAllowedClassDefinitions(Models.LevelUpContext.SelectedHero, compatibleClasses, ref ___selectedClass);

                    var commonData = __instance.CommonData;

                    // NOTE: don't use AttackModesPanel?. which bypasses Unity object lifetime check
                    if (commonData.AttackModesPanel)
                    {
                        commonData.AttackModesPanel.RefreshNow();
                    }

                    // NOTE: don't use PersonalityMapPanel?. which bypasses Unity object lifetime check
                    if (commonData.PersonalityMapPanel)
                    {
                        commonData.PersonalityMapPanel.RefreshNow();
                    }
                }
            }
        }

        // patches the method to get my own classLevel
        [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "FillClassFeatures")]
        internal static class CharacterStageClassSelectionPanelFillClassFeatures
        {
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

                var getHeroCharacterMethod = typeof(ICharacterBuildingService).GetMethod("get_HeroCharacter");
                var getClassLevelMethod = typeof(Models.LevelUpContext).GetMethod("GetClassLevel");
                var instructionsToBypass = 0;

                foreach (var instruction in instructions)
                {
                    if (instructionsToBypass > 0)
                    {
                        instructionsToBypass--;
                    }
                    else if (instruction.Calls(getHeroCharacterMethod))
                    {
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Call, getClassLevelMethod);
                        instructionsToBypass = 3;
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }

        // patches the method to get my own classLevel
        [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "RefreshCharacter")]
        internal static class CharacterStageClassSelectionPanelRefreshCharacter
        {
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

                var getHeroCharacterMethod = typeof(ICharacterBuildingService).GetMethod("get_HeroCharacter");
                var getClassLevelMethod = typeof(Models.LevelUpContext).GetMethod("GetClassLevel");
                var instructionsToBypass = 0;

                foreach (var instruction in instructions)
                {
                    if (instructionsToBypass > 0)
                    {
                        instructionsToBypass--;
                    }
                    else if (instruction.Calls(getHeroCharacterMethod))
                    {
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Call, getClassLevelMethod);
                        instructionsToBypass = 3;
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }

        // hides the equipment panel group on level up
        [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "Refresh")]
        internal static class CharacterStageClassSelectionPanelRefresh
        {
            public static bool SetActive()
            {
                return !(Models.LevelUpContext.LevelingUp && Models.LevelUpContext.DisplayingClassPanel);
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

                var setActiveFound = 0;
                var setActiveMethod = typeof(GameObject).GetMethod("SetActive");
                var mySetActiveMethod = typeof(CharacterStageClassSelectionPanelRefresh).GetMethod("SetActive");

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(setActiveMethod) && ++setActiveFound == 4)
                    {
                        yield return new CodeInstruction(OpCodes.Pop);
                        yield return new CodeInstruction(OpCodes.Call, mySetActiveMethod);
                    }

                    yield return instruction;
                }
            }
        }
    }
}
