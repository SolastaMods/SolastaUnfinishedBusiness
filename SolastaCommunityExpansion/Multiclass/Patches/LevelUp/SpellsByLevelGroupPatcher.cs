using System.Collections.Generic;
using HarmonyLib;

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

                if (Models.LevelUpContext.LevelingUp && Models.LevelUpContext.IsMulticlass)
                {
                    if (bindMode == SpellBox.BindMode.Learning && !Main.Settings.EnableDisplayAllKnownSpellsOnLevelUp)
                    {
                        allSpells.RemoveAll(s => !Models.CacheSpellsContext.IsSpellOfferedBySelectedClassSubclass(s));
                    }
                    else if (bindMode == SpellBox.BindMode.Unlearn)
                    {
                        allSpells.RemoveAll(s => !Models.CacheSpellsContext.IsSpellOfferedBySelectedClassSubclass(s) || !Models.CacheSpellsContext.IsSpellKnownBySelectedClassSubclass(s));
                    }
                }
            }
        }
    }
}
