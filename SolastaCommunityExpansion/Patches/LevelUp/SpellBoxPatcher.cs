using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.LevelUp
{
    // tag spells learned from other caster classes as Multiclass
    [HarmonyPatch(typeof(SpellBox), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellBox_Refresh
    {
        public static void Postfix(SpellBox __instance, SpellBox.BindMode ___bindMode,
            RectTransform ___autoPreparedGroup, GuiLabel ___autoPreparedTitle, GuiTooltip ___autoPreparedTooltip)
        {
            if (__instance.GuiSpellDefinition == null
                || ___bindMode == SpellBox.BindMode.Preparation
                || ___bindMode == SpellBox.BindMode.Inspection)
            {
                return;
            }

            var characterLevelUpScreen = Gui.GuiService.GetScreen<CharacterLevelUpScreen>();

            if (characterLevelUpScreen == null
                || !characterLevelUpScreen.Visible)
            {
                return;
            }

            var currentStagePanel =
                characterLevelUpScreen.GetField<CharacterLevelUpScreen, CharacterStagePanel>("currentStagePanel");

            if (currentStagePanel is not CharacterStageSpellSelectionPanel)
            {
                return;
            }

            var hero = Global.ActiveLevelUpHero;

            if (hero == null)
            {
                return;
            }

            var isMulticlass = LevelUpContext.IsMulticlass(hero);

            if (!isMulticlass)
            {
                return;
            }

            var allowedAutoPreparedSpells = LevelUpContext.GetAllowedAutoPreparedSpells(hero);

            if (allowedAutoPreparedSpells.Contains(__instance.SpellDefinition))
            {
                return;
            }

            var otherClassesKnownSpells = LevelUpContext.GetOtherClassesKnownSpells(hero);

            if (otherClassesKnownSpells.Contains(__instance.SpellDefinition))
            {
                ___autoPreparedTitle.Text = "Screen/&MulticlassSpellTitle";
                ___autoPreparedTooltip.Content = "Screen/&MulticlassSpellDescription";
                ___autoPreparedGroup.gameObject.SetActive(true);
            }
        }
    }
}
