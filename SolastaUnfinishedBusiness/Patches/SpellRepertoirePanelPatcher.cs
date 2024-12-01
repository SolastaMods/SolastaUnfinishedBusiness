using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using TMPro;
using UnityEngine;
using static SolastaUnfinishedBusiness.Models.Level20Context;

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
            MulticlassGameUi.RebuildSlotsTable(__instance);

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
    //UI allows other spells to be selected so easier to prevent it here
    [HarmonyPatch(typeof(SpellRepertoirePanel), nameof(SpellRepertoirePanel.OnSpellSelectedForPreparation))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnSpellSelectedForPreparation_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(SpellRepertoirePanel __instance, SpellBox spellBox)
        {
            var rulesetCharacter = __instance.GuiCharacter.RulesetCharacter;
            var spellDefinition = spellBox.SpellDefinition;

            return !Tabletop2024Context.IsInvalidMemorizeSelectedSpell(__instance, rulesetCharacter, spellDefinition) &&
                   !WizardSpellMastery.IsInvalidSelectedSpell(rulesetCharacter, spellDefinition) &&
                   !WizardSignatureSpells.IsInvalidSelectedSpell(rulesetCharacter, spellDefinition);
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

        private static void RepaintPanel(
            SpellRepertoirePanel __instance,
            string title,
            bool showDesc, bool showAutoButton, bool showClearRevertButtons, string byPassInstruction = null)
        {
            var titleTransform = __instance.PreparationPanel.transform.FindChildRecursive("Title");
            var descriptionTransform = __instance.PreparationPanel.transform.FindChildRecursive("Description");
            var automateButtonTransform = __instance.PreparationPanel.transform.FindChildRecursive("AutomateButton");
            var clearButtonTransform = __instance.PreparationPanel.transform.FindChildRecursive("ClearButton");
            var revertButtonTransform = __instance.PreparationPanel.transform.FindChildRecursive("RevertButton");
            var instructionTransform = __instance.PreparationPanel.transform.FindChildRecursive("Instruction");

            titleTransform!.GetComponentInChildren<TextMeshProUGUI>().text = title;

            descriptionTransform!.gameObject.SetActive(showDesc);

            // not the best solution but this object is getting re-activated somewhere else so moving off-screen
            automateButtonTransform!.localPosition = showAutoButton
                ? new Vector3(-12.5f, -61)
                : new Vector3(-1000, -1000);

            clearButtonTransform!.gameObject.SetActive(showClearRevertButtons);
            revertButtonTransform!.gameObject.SetActive(showClearRevertButtons);

            if (byPassInstruction != null)
            {
                instructionTransform!.GetComponentInChildren<TextMeshProUGUI>().text = byPassInstruction;
            }
        }

        private static void RefreshInteractivePreparation(
            SpellsByLevelGroup spellsByLevelGroup,
            bool canSelectSpells,
            bool maxReached,
            List<SpellDefinition> preparedSpells,
            SpellRepertoirePanel spellRepertoirePanel)
        {
            var rulesetCharacter = spellRepertoirePanel.GuiCharacter.RulesetCharacter;

            if (Tabletop2024Context.IsMemorizeSpellPreparation(rulesetCharacter))
            {
                RepaintPanel(
                    spellRepertoirePanel, Tabletop2024Context.FeatureMemorizeSpell.FormatTitle(), false, false, false,
                    Gui.Format("Screen/&PreparePanelInstruction", 1.ToString()));
            }
            else if (WizardSpellMastery.IsPreparation(rulesetCharacter, out _))
            {
                RepaintPanel(
                    spellRepertoirePanel, WizardSpellMastery.FeatureSpellMastery.FormatTitle(), true, false, true);

                canSelectSpells = spellsByLevelGroup.SpellLevel is 1 or 2;
            }
            else if (WizardSignatureSpells.IsPreparation(rulesetCharacter, out _))
            {
                RepaintPanel(
                    spellRepertoirePanel, WizardSignatureSpells.PowerSignatureSpells.FormatTitle(),
                    Main.Settings.EnableSignatureSpellsRelearn, false, true);

                canSelectSpells = spellsByLevelGroup.SpellLevel is 3;
            }
            else
            {
                RepaintPanel(
                    spellRepertoirePanel,
                    Gui.Localize("Screen/&PreparePanelTitle"),
                    true, true, true);
            }

            spellsByLevelGroup.RefreshInteractivePreparation(canSelectSpells, maxReached, preparedSpells);
        }
    }
}
