using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion.Multiclass.Models;

namespace SolastaCommunityExpansion.Multiclass.Patches.LevelUp
{
    internal static class SpellsByLevelGroupPatcher
    {
        // filters how spells are displayed during level up
        [HarmonyPatch(typeof(SpellsByLevelGroup), "CommonBind")]
        internal static class SpellsByLevelGroupCommonBind
        {
            internal static void Prefix(SpellBox.BindMode bindMode, List<SpellDefinition> allSpells)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                // it looks like it's ok to use CurrentLocalHeroCharacter on this context as this is an UI only patch
                var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();
                var hero = characterBuildingService.CurrentLocalHeroCharacter;

                // it should only apply when leveling up
                if (hero == null)
                {
                    return;
                }

                var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
                var isMulticlass = LevelUpContext.IsMulticlass(hero);

                if (!(isLevelingUp && isMulticlass))
                {
                    return;
                }

                if (bindMode == SpellBox.BindMode.Learning && !Main.Settings.EnableDisplayAllKnownSpellsOnLevelUp)
                {
                    allSpells.RemoveAll(s => !CacheSpellsContext.IsSpellOfferedBySelectedClassSubclass(hero, s));
                }
                else if (bindMode == SpellBox.BindMode.Unlearn)
                {
                    allSpells.RemoveAll(s => !CacheSpellsContext.IsSpellOfferedBySelectedClassSubclass(hero, s) || !CacheSpellsContext.IsSpellKnownBySelectedClassSubclass(hero, s));
                }
            }
        }
    }
}
