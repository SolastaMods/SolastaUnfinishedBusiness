using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaModApi;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.BestowCurseNoConcentrationRequired
{
    [HarmonyPatch(typeof(CharacterActionCastSpell), "StartConcentrationAsNeeded")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterActionCastSpell_StartConcentrationAsNeeded
    {
        public static bool Prefix(CharacterActionCastSpell __instance)
        {
            if (!Main.Settings.BestowCurseNoConcentrationRequiredForSlotLevel5OrAbove)
            {
                return true;
            }

            // Per SRD - Bestow Curse does not need concentration when cast with slot level 5 or above.
            // If the active spell is a sub-spell of Bestow Curse and the slot level is >= 5 don't run StartConcentrationAsNeeded.
            return
                !__instance.ActiveSpell.SpellDefinition.IsSubSpellOf(DatabaseHelper.SpellDefinitions.BestowCurse)
                || __instance.ActiveSpell.SlotLevel < 5;
        }
    }
}
