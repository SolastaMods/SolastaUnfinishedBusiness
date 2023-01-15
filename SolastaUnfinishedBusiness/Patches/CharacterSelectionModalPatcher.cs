using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterSelectionModalPatcher
{
    //PATCH: this patch changes the min/max requirements on locations or campaigns
    [HarmonyPatch(typeof(CharacterSelectionModal), "SelectFromPool")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SelectFromPool_Patch
    {
        [UsedImplicitly]
        public static void Prefix(CharacterSelectionModal __instance)
        {
            if (!Main.Settings.OverrideMinMaxLevel)
            {
                return;
            }

            __instance.MinLevel = ToolsContext.DungeonMinLevel;
            __instance.MaxLevel = ToolsContext.DungeonMaxLevel;
        }
    }

    [HarmonyPatch(typeof(CharacterSelectionModal), "EnumeratePlates")]
    [UsedImplicitly]
    public static class EnumeratePlates_Patch
    {
        //PATCH: correctly offers on adventures with min/max caps on character level (MULTICLASS)
        private static int MyLevels(IEnumerable<int> levels)
        {
            return levels.Sum();
        }

        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var myLevelMethod = new Func<IEnumerable<int>, int>(MyLevels).Method;
            var levelsField = typeof(RulesetCharacterHero.Snapshot).GetField("Levels");

            return instructions.ReplaceLoadField(levelsField,
                -1,
                2, "CharacterSelectionModal.EnumeratePlates",
                new CodeInstruction(OpCodes.Ldfld, levelsField),
                new CodeInstruction(OpCodes.Call, myLevelMethod));
        }

        //PATCH: don't display checkboxes when selection heroes on a modal (DEFAULT_PARTY)
        [UsedImplicitly]
        public static void Postfix([NotNull] CharacterSelectionModal __instance)
        {
            ToolsContext.Disable(__instance.charactersTable);
        }
    }
}
