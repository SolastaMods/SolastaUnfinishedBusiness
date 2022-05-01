using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.LevelUp
{
    [HarmonyPatch(typeof(FeatSubPanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FeatSubPanel_Bind
    {
        internal static void Prefix(List<FeatDefinition> ___relevantFeats, RectTransform ___table, GameObject ___itemPrefab)
        {
            var dbFeatDefinition = DatabaseRepository.GetDatabase<FeatDefinition>();

            ___relevantFeats.SetRange(dbFeatDefinition.Where(x => !x.GuiPresentation.Hidden));

            if (Main.Settings.EnableSortingFeats)
            {
                ___relevantFeats.Sort((a, b) => a.FormatTitle().CompareTo(b.FormatTitle()));
            }

            while (___table.childCount < ___relevantFeats.Count)
            {
                Gui.GetPrefabFromPool(___itemPrefab, ___table);
            }

            while (___table.childCount > ___relevantFeats.Count)
            {
                Gui.ReleaseInstanceToPool(___table.GetChild(___table.childCount - 1).gameObject);
            }

            for (var i = 0; i < ___table.childCount; i++)
            {
                var child = ___table.GetChild(i);
                var rectTransform = child.GetComponent<RectTransform>();

                rectTransform.sizeDelta = new Vector2(200, 34);
            }
        }
    }
}
