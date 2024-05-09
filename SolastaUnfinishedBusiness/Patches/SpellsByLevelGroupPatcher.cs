using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class SpellsByLevelGroupPatcher
{
    [HarmonyPatch(typeof(SpellsByLevelGroup), nameof(SpellsByLevelGroup.CommonBind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CommonBind_Patch
    {
        [UsedImplicitly]
        public static void Prefix(RulesetCharacter caster, Dictionary<SpellDefinition, string> extraSpellsMap)
        {
            //PATCH: add all auto prepared spells to extra spells map, so that different sources of auto spells won't bleed their tag
            //Don't use GetOriginalHero() here
            LevelUpContext.EnumerateExtraSpells(extraSpellsMap, caster as RulesetCharacterHero);
        }
    }
}
