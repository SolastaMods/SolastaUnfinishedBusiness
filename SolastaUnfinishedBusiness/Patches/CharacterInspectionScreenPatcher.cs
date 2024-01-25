using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterInspectionScreenPatcher
{
    [HarmonyPatch(typeof(CharacterInspectionScreen), nameof(CharacterInspectionScreen.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Prefix(CharacterInspectionScreen __instance, RulesetCharacterHero heroCharacter)
        {
            //PATCH: sets the inspection context for MC heroes
            Global.InspectedHero = heroCharacter;

            //PATCH: gets more real state for the toggles on top (MULTICLASS)
            var transform = __instance.toggleGroup.transform;

            transform.position =
                new Vector3(__instance.characterPlate.transform.position.x / 2f, transform.position.y, 0);
        }

        [UsedImplicitly]
        public static void Postfix(CharacterInspectionScreen __instance, RulesetCharacterHero heroCharacter)
        {
            //PATCH: hide repertoires that have hidden spell casting feature
            for (var index = 3; index < __instance.toggleGroup.transform.childCount; ++index)
            {
                var child = __instance.toggleGroup.transform.GetChild(index);

                if (index <= 3 && Gui.Game != null)
                {
                    continue;
                }

                var repertoire = heroCharacter.SpellRepertoires[index - __instance.staticTogglesNumber];

                if (repertoire.SpellCastingFeature.GuiPresentation.Hidden)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    [HarmonyPatch(typeof(CharacterInspectionScreen), nameof(CharacterInspectionScreen.Unbind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Unbind_Patch
    {
        [UsedImplicitly]
        public static void Prefix()
        {
            //PATCH: resets the inspection context for MC heroes
            Global.InspectedHero = null;

            //PATCH: enables Inventory Filtering and Sorting
            if (Main.Settings.EnableInventoryFilteringAndSorting && !Global.IsMultiplayer)
            {
                InventoryManagementContext.ResetControls();
            }
        }
    }

    //PATCH: resets the inspection context for MC heroes otherwise we get class name bleeding on char pool
    [HarmonyPatch(typeof(CharacterInspectionScreen), nameof(CharacterInspectionScreen.DoClose))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class DoClose_Patch
    {
        [UsedImplicitly]
        public static void Prefix()
        {
            Global.InspectedHero = null;
        }
    }
}
