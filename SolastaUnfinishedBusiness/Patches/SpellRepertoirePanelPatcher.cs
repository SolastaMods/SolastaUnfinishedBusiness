using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Models;
using TMPro;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class SpellRepertoirePanelPatcher
{
    [HarmonyPatch(typeof(SpellRepertoirePanel), nameof(SpellRepertoirePanel.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(SpellRepertoirePanel __instance)
        {
            //PATCH: filters how spells and slots are displayed on inspection (MULTICLASS)
            MulticlassGameUiContext.RebuildSlotsTable(__instance);

            //PATCH: displays sorcery point box for sorcerers only
            if (!Main.Settings.EnableDisplaySorceryPointBoxSorcererOnly)
            {
                return;
            }

            if (__instance.SpellRepertoire.SpellCastingClass != DatabaseHelper.CharacterClassDefinitions.Sorcerer)
            {
                __instance.sorceryPointsBox.gameObject.SetActive(false);
            }
        }
    }

    //PATCH: Supports Wizard Mastery and Signature spell features
    [HarmonyPatch(typeof(SpellRepertoirePanel), nameof(SpellRepertoirePanel.RefreshPreparation))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshPreparation_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var refreshInteractivePreparationMethod =
                typeof(SpellsByLevelGroup).GetMethod("RefreshInteractivePreparation");
            var myRefreshInteractivePreparationMethod =
                new Action<SpellsByLevelGroup, bool, bool, List<SpellDefinition>, SpellRepertoirePanel>(
                    RefreshInteractivePreparation).Method;

            return instructions.ReplaceCalls(refreshInteractivePreparationMethod,
                "SpellRepertoirePanel.RefreshPreparation",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, myRefreshInteractivePreparationMethod));
        }

        private static void RepaintPanel(SpellRepertoirePanel __instance, string title, bool showDesc, bool showButton)
        {
            var titleTransform = __instance.PreparationPanel.transform.FindChildRecursive("Title");
            var descriptionTransform = __instance.PreparationPanel.transform.FindChildRecursive("Description");
            var automateButtonTransform = __instance.PreparationPanel.transform.FindChildRecursive("AutomateButton");

            descriptionTransform!.gameObject.SetActive(showDesc);
            // not the best solution but this object is getting re-activated somewhere else so moving off-screen
            automateButtonTransform!.localPosition = showButton ? new Vector3(-12.5f, -61) : new Vector3(-1000, -1000);
            titleTransform!.GetComponentInChildren<TextMeshProUGUI>().text = title;
        }

        private static void RefreshInteractivePreparation(
            SpellsByLevelGroup spellsByLevelGroup,
            bool canSelectSpells,
            bool maxReached,
            List<SpellDefinition> preparedSpells,
            SpellRepertoirePanel spellRepertoirePanel)
        {
            var rulesetCharacter = spellRepertoirePanel.GuiCharacter.RulesetCharacter;

            if (Level20Context.WizardSpellMastery.IsPreparation(rulesetCharacter, out _))
            {
                RepaintPanel(
                    spellRepertoirePanel,
                    Gui.Localize(Level20Context.WizardSpellMastery.FeatureSpellMastery.GuiPresentation.Title),
                    true, false);

                canSelectSpells = spellsByLevelGroup.SpellLevel is 1 or 2;
            }
            else if (Level20Context.WizardSignatureSpells.IsPreparation(rulesetCharacter, out _))
            {
                RepaintPanel(
                    spellRepertoirePanel,
                    Gui.Localize(Level20Context.WizardSignatureSpells.PowerSignatureSpells.GuiPresentation.Title),
                    false, false);

                canSelectSpells = spellsByLevelGroup.SpellLevel is 3;
            }
            else
            {
                RepaintPanel(
                    spellRepertoirePanel,
                    Gui.Localize("Screen/&PreparePanelTitle"),
                    true, true);
            }

            spellsByLevelGroup.RefreshInteractivePreparation(canSelectSpells, maxReached, preparedSpells);
        }
    }
}
