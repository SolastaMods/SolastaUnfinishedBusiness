using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.LevelUp;

// tag spells learned from other caster classes as Multiclass
[HarmonyPatch(typeof(SpellBox), "Refresh")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class SpellBox_Refresh
{
    public static void Postfix(SpellBox __instance)
    {
        if (__instance.GuiSpellDefinition == null
            || __instance.bindMode is SpellBox.BindMode.Preparation or SpellBox.BindMode.Inspection)
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
            characterLevelUpScreen.currentStagePanel;

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

        if (!otherClassesKnownSpells.Contains(__instance.SpellDefinition))
        {
            return;
        }

        __instance.autoPreparedTitle.Text = "Screen/&MulticlassSpellTitle";
        __instance.autoPreparedTooltip.Content = "Screen/&MulticlassSpellDescription";
        __instance.autoPreparedGroup.gameObject.SetActive(true);
    }
}
