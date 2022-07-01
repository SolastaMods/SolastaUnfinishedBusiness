using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.GameUi.LevelUp;

// avoids a restart when enabling / disabling feats on the Mod UI panel
[HarmonyPatch(typeof(FeatSubPanel), "Bind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class FeatSubPanel_Bind
{
    internal static void Prefix(FeatSubPanel __instance)
    {
        var dbFeatDefinition = DatabaseRepository.GetDatabase<FeatDefinition>();

        __instance.relevantFeats.SetRange(dbFeatDefinition.Where(x => !x.GuiPresentation.Hidden));

        if (Main.Settings.EnableSortingFeats)
        {
            __instance.relevantFeats.Sort((a, b) => a.FormatTitle().CompareTo(b.FormatTitle()));
        }

        while (__instance.table.childCount < __instance.relevantFeats.Count)
        {
            Gui.GetPrefabFromPool(__instance.itemPrefab, __instance.table);
        }

        while (__instance.table.childCount > __instance.relevantFeats.Count)
        {
            Gui.ReleaseInstanceToPool(__instance.table.GetChild(__instance.table.childCount - 1).gameObject);
        }
    }
}

// enforce feat selection panel to always display 4 columns for easy selection
[HarmonyPatch(typeof(FeatSubPanel), "SetState")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class FeatSubPanel_SetState
{
    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var forceRebuildLayoutImmediateMethod = typeof(LayoutRebuilder).GetMethod("ForceRebuildLayoutImmediate",
            BindingFlags.Static | BindingFlags.Public);
        var forceSameWidthMethod = typeof(FeatSubPanel_SetState).GetMethod("ForceSameWidth");

        var code = instructions.ToList();
        var index = code.FindIndex(x => x.Calls(forceRebuildLayoutImmediateMethod));

        code[index] = new CodeInstruction(OpCodes.Ldarg_1);
        code.Insert(index + 1, new CodeInstruction(OpCodes.Call, forceSameWidthMethod));

        return code;
    }

    public static void ForceSameWidth(RectTransform table, bool active)
    {
        const int COLUMNS = 4;
        const int WIDTH = 224;
        const int HEIGHT = 34;
        const int SPACING = 6;

        if (active && Main.Settings.EnableSameWidthFeatSelection)
        {
            var hero = Global.ActiveLevelUpHero;
            var buildingData = hero.GetHeroBuildingData();

            if (buildingData == null)
            {
                return;
            }

            var trainedFeats = buildingData.LevelupTrainedFeats.SelectMany(x => x.Value).ToList();

            trainedFeats.AddRange(hero.TrainedFeats);

            var j = 0;
            var rect = table.GetComponent<RectTransform>();

            rect.sizeDelta = new Vector2(rect.sizeDelta.x, ((table.childCount / COLUMNS) + 1) * (HEIGHT + SPACING));

            for (var i = 0; i < table.childCount; i++)
            {
                var child = table.GetChild(i);

                var featItem = child.GetComponent<FeatItem>();

                if (trainedFeats.Contains(featItem.GuiFeatDefinition.FeatDefinition))
                {
                    continue;
                }

                var x = j % COLUMNS;
                var y = j / COLUMNS;
                var posX = x * (WIDTH + (SPACING * 2));
                var posY = -y * (HEIGHT + SPACING);

                rect = child.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(posX, posY);
                rect.sizeDelta = new Vector2(WIDTH, HEIGHT);

                j++;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(table);
    }
}
