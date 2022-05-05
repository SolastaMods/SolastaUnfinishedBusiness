using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaMulticlass.Patches.LevelUp
{
    internal static class SpellsByLevelGroupPatcher
    {
        // filters how spells are displayed during level up
        [HarmonyPatch(typeof(SpellsByLevelGroup), "CommonBind")]
        internal static class SpellsByLevelGroupCommonBind
        {
            internal static void Prefix(SpellBox.BindMode bindMode, List<SpellDefinition> allSpells)
            {
                if (bindMode == SpellBox.BindMode.Inspection || bindMode == SpellBox.BindMode.Preparation)
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
}
