using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class SpellSelectionPanelPatcher
{
    [HarmonyPatch(typeof(SpellSelectionPanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Bind_Patch
    {
        internal static void Prefix(
            GuiCharacter caster,
            ref bool cantripOnly,
            ActionDefinitions.ActionType actionType)
        {
            //PATCH: supports `IReplaceAttackWithCantrip`
            var gameLocationCaster = caster.GameLocationCharacter;

            if (gameLocationCaster.RulesetCharacter.HasSubFeatureOfType<IReplaceAttackWithCantrip>()
                && gameLocationCaster.UsedMainAttacks > 0 && actionType == ActionDefinitions.ActionType.Main)
            {
                cantripOnly = true;
            }
        }

        internal static void Postfix(
            SpellSelectionPanel __instance,
            GuiCharacter caster,
            SpellsByLevelBox.SpellCastEngagedHandler spellCastEngaged,
            ActionDefinitions.ActionType actionType,
            bool cantripOnly)
        {
            //PATCH: shows spell selection on multiple rows
            if (!Main.Settings.EnableMultiLineSpellPanel)
            {
                return;
            }

            GameUiContext.SpellSelectionPanelMultilineBind(
                __instance, caster, spellCastEngaged, actionType, cantripOnly);
        }
    }

    [HarmonyPatch(typeof(SpellSelectionPanel), "Unbind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Unbind_Patch
    {
        internal static void Postfix()
        {
            //PATCH: shows spell selection on multiple rows
            if (!Main.Settings.EnableMultiLineSpellPanel)
            {
                return;
            }

            GameUiContext.SpellSelectionPanelMultilineUnbind();
        }
    }
}
