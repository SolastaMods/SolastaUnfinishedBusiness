using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaModApi;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.UpcastConjureElementalAndFey
{
    [HarmonyPatch(typeof(SubspellSelectionModal), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [HarmonyPatch(new Type[] {
        typeof(SpellDefinition), typeof(RulesetCharacter), typeof(RulesetSpellRepertoire),
        typeof(SpellsByLevelBox.SpellCastEngagedHandler), typeof(int), typeof(RectTransform)})]
    internal static class SubspellSelectionModal_Bind
    {
        public static void Prefix(SpellDefinition masterSpell, int slotLevel)
        {
            if (!Main.Settings.EnableUpcastConjureElementalAndFey)
            {
                return;
            }

            SpellDefinition_SubspellsList.FilterBySlotLevel =
                masterSpell.Name == DatabaseHelper.SpellDefinitions.ConjureElemental.Name
                || masterSpell.Name == DatabaseHelper.SpellDefinitions.ConjureFey.Name
                ? slotLevel
                : null;
        }

        public static void Postfix()
        {
            SpellDefinition_SubspellsList.FilterBySlotLevel = null;
        }
    }

    [HarmonyPatch(typeof(SubspellSelectionModal), "OnActivate")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SubspellSelectionModal_OnActivate
    {
        public static bool Prefix(SubspellSelectionModal __instance, int index, int ___slotLevel,
            RulesetSpellRepertoire ___spellRepertoire, SpellsByLevelBox.SpellCastEngagedHandler ___spellCastEngaged)
        {
            if (!Main.Settings.EnableUpcastConjureElementalAndFey ||
                SpellDefinition_SubspellsList.FilteredSubspells == null ||
                SpellDefinition_SubspellsList.FilteredSubspells.Count == 0)
            {
                return true;
            }

            var subspells = SpellDefinition_SubspellsList.FilteredSubspells;

            if (subspells.Count > index)
            {
                ___spellCastEngaged?.Invoke(___spellRepertoire, SpellDefinition_SubspellsList.FilteredSubspells[index], ___slotLevel);

                // If a device had the summon function, implement here

                //else if (this.deviceFunctionEngaged != null)
                //    this.deviceFunctionEngaged(this.guiCharacter, this.rulesetItemDevice, this.rulesetDeviceFunction, 0, index);

                __instance.Hide();

                subspells.Clear();

                return false;
            }

            return true;
        }
    }
}
