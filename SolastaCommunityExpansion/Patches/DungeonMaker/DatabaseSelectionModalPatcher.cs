using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches
{
    internal static class DatabaseSelectionModalPatcher
    {
        [HarmonyPatch(typeof(DatabaseSelectionModal), "SelectMonsterDefinition")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class DatabaseSelectionModal_BuildMonsters
        {
            internal static void Prefix(DatabaseSelectionModal __instance, List<MonsterDefinition> ___allMonsters)
            {
                bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                if (Main.Settings.UnleashAllMonsters)
                {
                    ___allMonsters.Clear();

                    if (isShiftPressed)
                    {
                        foreach (var allElement in DatabaseRepository.GetDatabase<MonsterDefinition>().Where(x => !x.GuiPresentation.Hidden))
                        {
                            ___allMonsters.Add(allElement);
                        }

                        ___allMonsters.Sort(__instance);
                    }
                }
            }
        }

        // this patch unleashes all monster definitions to be used as NPCs
        [HarmonyPatch(typeof(DatabaseSelectionModal), "SelectNpcDefinition")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class DatabaseSelectionModal_BuildNpcs
        {
            internal static void Prefix(DatabaseSelectionModal __instance, List<MonsterDefinition> ___allNpcs)
            {
                bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                if (Main.Settings.UnleashAllNPCs)
                {
                    ___allNpcs.Clear();

                    if (isShiftPressed)
                    {
                        foreach (var allElement in DatabaseRepository.GetDatabase<MonsterDefinition>().Where(x => !x.GuiPresentation.Hidden))
                        {
                            ___allNpcs.Add(allElement);
                        }

                        ___allNpcs.Sort(__instance);
                    }
                }
            }
        }
    }
}
