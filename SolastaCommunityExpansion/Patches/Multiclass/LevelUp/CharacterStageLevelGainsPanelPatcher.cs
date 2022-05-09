using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Multiclass.LevelUp
{
    // patches the method to get my own class and level for level up
    [HarmonyPatch(typeof(CharacterStageLevelGainsPanel), "EnterStage")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageLevelGainsPanel_EnterStage
    {
        public static void GetLastAssignedClassAndLevel(ICharacterBuildingService _, RulesetCharacterHero hero, out CharacterClassDefinition lastClassDefinition, out int level)
        {
            if (LevelUpContext.IsLevelingUp(hero))
            {
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
            var code = instructions.ToList();

            if (Main.Settings.EnableMulticlass)
            {
                var getLastAssignedClassAndLevelMethod = typeof(ICharacterBuildingService).GetMethod("GetLastAssignedClassAndLevel");
                var customGetLastAssignedClassAndLevelMethod = typeof(CharacterStageLevelGainsPanel_EnterStage).GetMethod("GetLastAssignedClassAndLevel");
                var index = code.FindIndex(x => x.Calls(getLastAssignedClassAndLevelMethod));

                code[index] = new CodeInstruction(OpCodes.Call, customGetLastAssignedClassAndLevelMethod);
            }

            return code;
        }
    }

    // only displays spell casting features from the current class
    [HarmonyPatch(typeof(CharacterStageLevelGainsPanel), "RefreshSpellcastingFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageLevelGainsPanel_RefreshSpellcastingFeatures
    {
        public static List<RulesetSpellRepertoire> SpellRepertoires(RulesetCharacterHero rulesetCharacterHero)
        {
            if (LevelUpContext.IsLevelingUp(rulesetCharacterHero) && SharedSpellsContext.IsMulticaster(rulesetCharacterHero))
            {
                return rulesetCharacterHero.SpellRepertoires
                    .Where(x => LevelUpContext.IsRepertoireFromSelectedClassSubclass(rulesetCharacterHero, x))
                    .ToList();
            }

            return rulesetCharacterHero.SpellRepertoires;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = instructions.ToList();

            if (Main.Settings.EnableMulticlass)
            {
                var spellRepertoiresMethod = typeof(RulesetCharacter).GetMethod("get_SpellRepertoires");
                var filteredSpellRepertoiresMethod = typeof(CharacterStageLevelGainsPanel_RefreshSpellcastingFeatures).GetMethod("SpellRepertoires");
                var index = code.FindIndex(x => x.Calls(spellRepertoiresMethod));

                code[index] = new CodeInstruction(OpCodes.Call, filteredSpellRepertoiresMethod);
            }

            return code;
        }
    }
}
