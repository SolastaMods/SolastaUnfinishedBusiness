using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Bugfix
{
    [HarmonyPatch(typeof(CharacterActionCastSpell), "StartConcentrationAsNeeded")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal class CharacterActionCastSpell_StartConcentrationAsNeeded
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
                !__instance.ActiveSpell.SpellDefinition.IsSubSpellOf(SolastaModApi.DatabaseHelper.SpellDefinitions.BestowCurse)
                || __instance.ActiveSpell.SlotLevel < 5;
        }
    }
}
