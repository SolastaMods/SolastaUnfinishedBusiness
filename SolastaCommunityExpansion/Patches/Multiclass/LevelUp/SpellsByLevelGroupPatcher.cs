using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Multiclass.LevelUp
{
    // filters how spells are displayed during level up
    [HarmonyPatch(typeof(SpellsByLevelGroup), "CommonBind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellsByLevelGroup_CommonBind
    {
        internal static void Prefix(SpellBox.BindMode bindMode, List<SpellDefinition> allSpells)
        {
            if (!Main.Settings.EnableMulticlass)
            {
                return;
            }

            if (Main.Settings.DisplayAllKnownSpellsDuringLevelUp ||
                bindMode == SpellBox.BindMode.Inspection || bindMode == SpellBox.BindMode.Preparation)
            {
                return;
            }

            var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();

            var hero = characterBuildingService.CurrentLocalHeroCharacter;

            if (hero == null)
            {
                return;
            }

            var allowedSpells = LevelUpContext.GetAllowedSpells(hero);

            allSpells.RemoveAll(x => !allowedSpells.Contains(x));
        }
    }
}
