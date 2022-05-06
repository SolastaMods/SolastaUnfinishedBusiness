using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaMulticlass.Models;
using UnityEngine;

namespace SolastaMulticlass.Patches.LevelUp
{
    internal static class CharacterStageClassSelectionPanelPatcher
    {
        // flag displaying the class panel / apply in/out logic
        [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "OnBeginShow")]
        internal static class CharacterStageClassSelectionPanelOnBeginShow
        {
            internal static void Prefix(
                CharacterStageClassSelectionPanel __instance,
                RulesetCharacterHero ___currentHero,
                List<CharacterClassDefinition> ___compatibleClasses,
                ref int ___selectedClass)
            {
                if (!LevelUpContext.IsLevelingUp(___currentHero))
                {
                    return;
                }

                LevelUpContext.SetIsClassSelectionStage(___currentHero, true);
                InOutRulesContext.EnumerateHeroAllowedClassDefinitions(___currentHero, ___compatibleClasses, ref ___selectedClass);

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

        // hide the equipment panel group
        [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "Refresh")]
        internal static class CharacterStageClassSelectionPanelRefresh
        {
            public static bool SetActive(RulesetCharacterHero currentHero) => !LevelUpContext.IsLevelingUp(currentHero);

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var setActiveFound = 0;
                var setActiveMethod = typeof(GameObject).GetMethod("SetActive");
                var mySetActiveMethod = typeof(CharacterStageClassSelectionPanelRefresh).GetMethod("SetActive");
                var currentHeroField = typeof(CharacterStageClassSelectionPanel).GetField("currentHero", BindingFlags.Instance | BindingFlags.NonPublic);

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(setActiveMethod) && ++setActiveFound == 4)
                    {
                        yield return new CodeInstruction(OpCodes.Pop);
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, currentHeroField);
                        yield return new CodeInstruction(OpCodes.Call, mySetActiveMethod);
                    }

                    yield return instruction;
                }
            }
        }
    }
}
