using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaMulticlass.Models;

namespace SolastaMulticlass.Patches.LevelUp
{
    internal static class CharacterStageLevelGainsPanelPatcher
    {
        // patches the method to get my own class and level for level up
        [HarmonyPatch(typeof(CharacterStageLevelGainsPanel), "EnterStage")]
        internal static class CharacterStageLevelGainsPanelEnterStage
        {
            public static void GetLastAssignedClassAndLevel(ICharacterBuildingService _, RulesetCharacterHero hero, out CharacterClassDefinition lastClassDefinition, out int level)
            {
                if (LevelUpContext.IsLevelingUp(hero))
                {
                    LevelUpContext.SetIsClassSelectionStage(hero, false);

                    lastClassDefinition = LevelUpContext.GetSelectedClass(hero);
                    level = hero.ClassesHistory.Count;
                }
                else if (hero.ClassesHistory.Count > 0)
                {
                    lastClassDefinition = hero.ClassesHistory[hero.ClassesHistory.Count - 1];
                    level = hero.ClassesAndLevels[lastClassDefinition];
                }
                else
                {
                    lastClassDefinition = null;
                    level = 0;
                }
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var getLastAssignedClassAndLevelMethod = typeof(ICharacterBuildingService).GetMethod("GetLastAssignedClassAndLevel");
                var customGetLastAssignedClassAndLevelMethod = typeof(CharacterStageLevelGainsPanelEnterStage).GetMethod("GetLastAssignedClassAndLevel");

                var code = instructions.ToList();
                var index = code.FindIndex(x => x.Calls(getLastAssignedClassAndLevelMethod));

                code[index] = new CodeInstruction(OpCodes.Call, customGetLastAssignedClassAndLevelMethod);

                return code;
            }
        }

        // only displays spell casting features from the current class
        [HarmonyPatch(typeof(CharacterStageLevelGainsPanel), "RefreshSpellcastingFeatures")]
        internal static class CharacterStageLevelGainsPanelRefreshSpellcastingFeatures
        {
            public static List<RulesetSpellRepertoire> SpellRepertoires(RulesetCharacterHero rulesetCharacterHero)
            {
                if (LevelUpContext.IsLevelingUp(rulesetCharacterHero) && SharedSpellsContext.IsMulticaster(rulesetCharacterHero))
                {
                    var result = new List<RulesetSpellRepertoire>();

                    result.AddRange(rulesetCharacterHero.SpellRepertoires.Where(x => LevelUpContext.IsRepertoireFromSelectedClassSubclass(rulesetCharacterHero, x)));

                    return result;
                }

                return rulesetCharacterHero.SpellRepertoires;
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var spellRepertoiresMethod = typeof(RulesetCharacter).GetMethod("get_SpellRepertoires");
                var filteredSpellRepertoiresMethod = typeof(CharacterStageLevelGainsPanelRefreshSpellcastingFeatures).GetMethod("SpellRepertoires");

                var code = instructions.ToList();
                var index = code.FindIndex(x => x.Calls(spellRepertoiresMethod));

                code[index] = new CodeInstruction(OpCodes.Call, filteredSpellRepertoiresMethod);

                return code;
            }
        }
    }
}
