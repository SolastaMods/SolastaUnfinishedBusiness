using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.DungeonMaker
{
    [HarmonyPatch(typeof(DatabaseSelectionModal), "SelectMonsterDefinition")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class DatabaseSelectionModal_BuildMonsters
    {
        internal static void Prefix(List<MonsterDefinition> ___allMonsters)
        {
            if (Main.Settings.UnleashNpcAsEnemy)
            {
                var isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                ___allMonsters.Clear();

                if (isShiftPressed)
                {
                    ___allMonsters.AddRange(DatabaseRepository.GetDatabase<MonsterDefinition>()
                        .Where(x => !x.GuiPresentation.Hidden)
                        .OrderBy(d => Gui.Localize(d.GuiPresentation.Title)));
                }
            }
        }
    }

    // this patch unleashes all monster definitions to be used as NPCs
    [HarmonyPatch(typeof(DatabaseSelectionModal), "SelectNpcDefinition")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class DatabaseSelectionModal_BuildNpcs
    {
        internal static void Prefix(List<MonsterDefinition> ___allNpcs)
        {
            if (Main.Settings.UnleashEnemyAsNpc)
            {
                var isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                ___allNpcs.Clear();

                if (isShiftPressed)
                {
                    ___allNpcs.AddRange(DatabaseRepository.GetDatabase<MonsterDefinition>()
                        .Where(x => !x.GuiPresentation.Hidden)
                        .OrderBy(d => Gui.Localize(d.GuiPresentation.Title)));
                }
            }
        }
    }
}
