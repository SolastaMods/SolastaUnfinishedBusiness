using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.RitualCasting
{
    // ensures ritual spells work correctly when MC
    [HarmonyPatch(typeof(RulesetCharacter), "CanCastAnyRitualSpell")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_CanCastAnyRitualSpell
    {
        internal static bool Prefix(RulesetCharacter __instance, ref bool __result,
            List<SpellDefinition> ___usableSpells)
        {
            if (__instance is not RulesetCharacterHero)
            {
                return true;
            }

            RitualSelectionPanel_Bind.EnumerateUsableRitualSpells(__instance, RuleDefinitions.RitualCasting.None,
                ___usableSpells);
            __result = ___usableSpells.Count > 0;

            return false;
        }
    }
}
