using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public class SpellsByLevelGroupPatcher
{
    [HarmonyPatch(typeof(SpellsByLevelGroup), "CommonBind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class CommonBind_Patch
    {
        public static void Prefix(RulesetCharacter caster, Dictionary<SpellDefinition, string> extraSpellsMap)
        {
            //PATCH: add all auto prepared spells to extra spells map, so that different sources of auto spells won't bleed their tag
            LevelUpContext.EnumerateExtraSpells(extraSpellsMap, caster as RulesetCharacterHero);
        }
    }
}
