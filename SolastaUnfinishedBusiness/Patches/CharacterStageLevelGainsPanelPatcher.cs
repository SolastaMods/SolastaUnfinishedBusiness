using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class CharacterStageLevelGainPanelPatcher
{
    [HarmonyPatch(typeof(CharacterStageLevelGainsPanel), "EnterStage")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class EnterStage_Patch
    {
        //PATCH: gets my own class and level for level up (MULTICLASS)
        // ReSharper disable once UnusedMember.Global
        public static void GetLastAssignedClassAndLevel(
            ICharacterBuildingService _,
            [NotNull] RulesetCharacterHero hero,
            [CanBeNull] out CharacterClassDefinition lastClassDefinition,
            out int level)
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

        [NotNull]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var getLastAssignedClassAndLevelMethod =
                typeof(ICharacterBuildingService).GetMethod("GetLastAssignedClassAndLevel");
            var customGetLastAssignedClassAndLevelMethod =
                typeof(EnterStage_Patch).GetMethod("GetLastAssignedClassAndLevel");

            var code = instructions.ToList();
            var index = code.FindIndex(x => x.Calls(getLastAssignedClassAndLevelMethod));

            code[index] = new CodeInstruction(OpCodes.Call, customGetLastAssignedClassAndLevelMethod);

            return code;
        }
    }

    [HarmonyPatch(typeof(CharacterStageLevelGainsPanel), "RefreshSpellcastingFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class RefreshSpellcastingFeatures_Patch
    {
        //PATCH: only displays spell casting features from the current class (MULTICLASS)
        private static List<RulesetSpellRepertoire> SpellRepertoires(
            [NotNull] RulesetCharacterHero rulesetCharacterHero)
        {
            if (LevelUpContext.IsLevelingUp(rulesetCharacterHero) && LevelUpContext.IsMulticlass(rulesetCharacterHero))
            {
                return rulesetCharacterHero.SpellRepertoires
                    .Where(x => LevelUpContext.IsRepertoireFromSelectedClassSubclass(rulesetCharacterHero, x))
                    .ToList();
            }

            return rulesetCharacterHero.SpellRepertoires;
        }

        [NotNull]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var code = instructions.ToList();
            var spellRepertoiresMethod = typeof(RulesetCharacter).GetMethod("get_SpellRepertoires");
            var filteredSpellRepertoiresMethod =
                new Func<RulesetCharacterHero, List<RulesetSpellRepertoire>>(SpellRepertoires).Method;
            var index = code.FindIndex(x => x.Calls(spellRepertoiresMethod));

            code[index] = new CodeInstruction(OpCodes.Call, filteredSpellRepertoiresMethod);

            return code;
        }
    }
}
