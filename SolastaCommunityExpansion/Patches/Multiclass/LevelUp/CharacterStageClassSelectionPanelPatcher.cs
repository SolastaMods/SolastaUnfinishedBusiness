using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.Multiclass.LevelUp
{
    // flag displaying the class panel / apply in/out logic
    [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageClassSelectionPanel_OnBeginShow
    {
        internal static void Prefix(
            CharacterStageClassSelectionPanel __instance,
            RulesetCharacterHero ___currentHero,
            List<CharacterClassDefinition> ___compatibleClasses,
            ref int ___selectedClass)
        {
            // avoids a restart when enabling / disabling classes on the Mod UI panel
            var visibleClasses = DatabaseRepository.GetDatabase<CharacterClassDefinition>().Where(x => !x.GuiPresentation.Hidden);

            ___compatibleClasses.SetRange(visibleClasses.OrderBy(x => x.FormatTitle()));
            
            if (!Main.Settings.EnableMulticlass)
            {
                return;
            }

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
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageClassSelectionPanel_Refresh
    {
        public static bool SetActive(RulesetCharacterHero currentHero) => !LevelUpContext.IsLevelingUp(currentHero);

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (!Main.Settings.EnableMulticlass)
            {
                foreach (var instruction in instructions)
                {
                    yield return instruction;
                };

                yield break;
            }

            var setActiveFound = 0;
            var setActiveMethod = typeof(GameObject).GetMethod("SetActive");
            var mySetActiveMethod = typeof(CharacterStageClassSelectionPanel_Refresh).GetMethod("SetActive");
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
