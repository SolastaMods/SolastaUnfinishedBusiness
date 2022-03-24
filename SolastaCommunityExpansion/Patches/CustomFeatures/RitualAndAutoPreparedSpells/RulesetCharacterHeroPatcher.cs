using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.RitualAndAutoPreparedSpells
{
    //
    // Extra Ritual Casting
    //
    [HarmonyPatch(typeof(RulesetCharacterHero), "EnumerateUsableRitualSpells")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_EnumerateUsableRitualSpells
    {
        internal static void Postfix(RulesetCharacterHero __instance, RuleDefinitions.RitualCasting ritualType, List<SpellDefinition> ritualSpells)
        {
            if ((ExtraRitualCasting)ritualType != ExtraRitualCasting.Known)
            {
                return;
            }

            var spellRepertoire = __instance.SpellRepertoires
                .Where(r => r.SpellCastingFeature.SpellReadyness == RuleDefinitions.SpellReadyness.AllKnown)
                .FirstOrDefault(r => r.SpellCastingFeature.SpellKnowledge == RuleDefinitions.SpellKnowledge.Selection);

            if (spellRepertoire == null)
            {
                return;
            }

            ritualSpells.AddRange(spellRepertoire.KnownSpells
                .Where(s => s.Ritual)
                .Where(s => spellRepertoire.MaxSpellLevelOfSpellCastingLevel >= s.SpellLevel));

            if (spellRepertoire.AutoPreparedSpells == null)
            {
                return;
            }

            ritualSpells.AddRange(spellRepertoire.AutoPreparedSpells
                .Where(s => s.Ritual)
                .Where(s => spellRepertoire.MaxSpellLevelOfSpellCastingLevel >= s.SpellLevel));
        }
    }
}
