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
            internal static void Prefix(SpellsByLevelGroup __instance, SpellBox.BindMode bindMode, List<SpellDefinition> allSpells)
            {
                if (__instance.SpellRepertoire == null)
                {
                    return;
                }

                var hero = LevelUpContext.GetHero(__instance.SpellRepertoire.CharacterName);

                if (hero == null)
                {
                    return;
                }

                var isMulticaster = SharedSpellsContext.IsMulticaster(hero);

                if (!isMulticaster)
                {
                    return;
                }

                if (bindMode == SpellBox.BindMode.Learning)
                {
                    allSpells.RemoveAll(s => !LevelUpContext.IsSpellOfferedBySelectedClassSubclass(hero, s));
                }
                else if (bindMode == SpellBox.BindMode.Unlearn)
                {
                    allSpells.RemoveAll(s => !LevelUpContext.IsSpellOfferedBySelectedClassSubclass(hero, s) || !LevelUpContext.IsSpellKnownBySelectedClassSubclass(hero, s));
                }
            }
        }
    }
}
