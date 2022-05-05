using System.Collections.Generic;
using HarmonyLib;
using SolastaMulticlass.Models;

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
                var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();

                var hero = characterBuildingService.CurrentLocalHeroCharacter;

                if (hero == null)
                {
                    return;
                }

                if (!LevelUpContext.IsLevelingUp(hero))
                {
                    return;
                }

                if (bindMode == SpellBox.BindMode.Learning)
                {
                    allSpells.RemoveAll(s => !LevelUpContext.IsSpellOfferedBySelectedClassSubclass(hero, s));
                }
                else if (bindMode == SpellBox.BindMode.Unlearn)
                {
                    allSpells.RemoveAll(s => !LevelUpContext.IsSpellOfferedBySelectedClassSubclass(hero, s));
                }
            }
        }
    }
}
