using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.RitualCasting;

// ensures ritual spells work correctly when MC
[HarmonyPatch(typeof(RulesetCharacter), "CanCastAnyRitualSpell")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacter_CanCastAnyRitualSpell
{
    internal static bool Prefix(RulesetCharacter __instance, ref bool __result)
    {
        if (__instance is not RulesetCharacterHero)
        {
            return true;
        }

        RitualSelectionPanel_Bind.EnumerateUsableRitualSpells(__instance, RuleDefinitions.RitualCasting.None,
            __instance.usableSpells);
        __result = __instance.usableSpells.Count > 0;

        return false;
    }
}
