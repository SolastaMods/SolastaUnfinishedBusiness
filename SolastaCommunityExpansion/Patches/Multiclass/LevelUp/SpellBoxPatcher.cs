using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.Multiclass.LevelUp
{
    [HarmonyPatch(typeof(SpellBox), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellActivationBox_Refresh
    {
        public static void Postfix(SpellBox __instance, SpellBox.BindMode ___bindMode, RectTransform ___autoPreparedGroup, GuiLabel ___autoPreparedTitle, GuiTooltip ___autoPreparedTooltip)
        {
            if (!Main.Settings.EnableMulticlass
                || ___bindMode == SpellBox.BindMode.Preparation
                || ___bindMode == SpellBox.BindMode.Inspection)
            {
                return;
            }

            var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();
            var hero = characterBuildingService.CurrentLocalHeroCharacter;

            if (hero == null)
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
                ___autoPreparedTitle.Text = string.Format("Screen/&MulticlassSpellTitle");
                ___autoPreparedTooltip.Content = string.Format("Screen/&MulticlassSpellDescription");
                ___autoPreparedGroup.gameObject.SetActive(true);
            }
        }
    }
}
