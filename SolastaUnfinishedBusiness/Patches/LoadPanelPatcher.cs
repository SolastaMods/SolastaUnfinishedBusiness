using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using UnityEngine;
using UnityEngine.EventSystems;
using static SolastaUnfinishedBusiness.Models.SaveByLocationContext;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class LoadPanelPatcher
{
    [HarmonyPatch(typeof(LoadPanel), nameof(LoadPanel.OnBeginShow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnBeginShow_Patch
    {

        private static bool NeedsCustomLogic(LoadPanel panel)
        {
            return Main.Settings.EnableSaveByLocation && !panel.ImportSaveMode;
        }

        [UsedImplicitly]
        public static void Prefix([NotNull] LoadPanel __instance)
        {
            //PATCH: EnableSaveByLocation
            if (NeedsCustomLogic(__instance))
            {
                LoadPanelOnBeginShowSaveByLocationBehavior(__instance);
            }
            else
            {
                Dropdown?.SetActive(false);
            }

            //PATCH: Allow import any campaign if override min max level is on

            // this is causing issues loading games so had to disable until finding out why

            // if (Main.Settings.OverrideMinMaxLevel)
            // {
            //     __instance.CampaignForImportSaveMode.maxLevelImport = Level20Context.ModMaxLevel;
            // }
        }
        
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: EnableSaveByLocation - do not enumerate saves if custom logic applies
            var oldMethod = typeof(LoadPanel).GetMethod(nameof(LoadPanel.EnumerateSaveLines));
            var newMethod = new Func<LoadPanel, IEnumerator>(CustomEnumerate).Method;

            return instructions.ReplaceCalls(oldMethod, "LoadPanel.OnBeginShow",
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, newMethod));
        }

        private static IEnumerator CustomEnumerate(LoadPanel panel)
        {
            if (NeedsCustomLogic(panel))
            {
                yield break;
            }

            yield return panel.EnumerateSaveLines();
        }
    }

    [HarmonyPatch(typeof(LoadPanel), nameof(LoadPanel.OnEndHide))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnBeginHide_Patch
    {
        [UsedImplicitly]
        public static void Postfix()
        {
            Dropdown?.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(LoadPanel), nameof(LoadPanel.HandleInputControlSchemeChangedForShow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class HandleInputControlSchemeChangedForShow_Patch
    {
        [UsedImplicitly]
        public static void Postfix()
        {
            Dropdown?.UpdateControls();
        }
    }
}
