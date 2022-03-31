using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion;
using SolastaMulticlass.Models;

namespace SolastaMulticlass.Patches.LevelUp
{
    internal static class SpellsByLevelGroupPatcher
    {
        // filters how spells are displayed during level up
        [HarmonyPatch(typeof(SpellsByLevelGroup), "CommonBind")]
        internal static class SpellsByLevelGroupCommonBind
        {
            internal static void Prefix(SpellsByLevelGroup __instance, SpellBox.BindMode bindMode, List<SpellDefinition> allSpells)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                var hero = LevelUpContext.GetHero(__instance.SpellRepertoire.CharacterName);

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
