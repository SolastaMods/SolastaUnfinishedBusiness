using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Multiclass.RitualCasting
{
    // ensures ritual spells from all spell repertoires are made available
    [HarmonyPatch(typeof(RitualSelectionPanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RitualSelectionPanel_Bind
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var enumerateUsableRitualSpellsMethod = typeof(RulesetCharacter).GetMethod("EnumerateUsableRitualSpells");
            var myEnumerateUsableRitualSpellsMethod = typeof(RitualSelectionPanel_Bind).GetMethod("EnumerateUsableRitualSpells");

            var code = instructions.ToList();
            var index = code.FindIndex(x => x.Calls(enumerateUsableRitualSpellsMethod));

            code[index] = new CodeInstruction(OpCodes.Call, myEnumerateUsableRitualSpellsMethod);

            return code;
        }

        public static void EnumerateUsableRitualSpells(
            RulesetCharacter rulesetCharacter,
            RuleDefinitions.RitualCasting _,
            List<SpellDefinition> ritualSpells)
        {
            rulesetCharacter.EnumerateFeaturesToBrowse<FeatureDefinitionMagicAffinity>(rulesetCharacter.FeaturesToBrowse);

            var ritualSpellsCache = new List<SpellDefinition>();

            ritualSpells.Clear();

            foreach (var featureDefinitionMagicAffinity in rulesetCharacter.FeaturesToBrowse
                .OfType<FeatureDefinitionMagicAffinity>()
                .Where(f => f.RitualCasting != RuleDefinitions.RitualCasting.None))
            {
                rulesetCharacter.EnumerateUsableRitualSpells(featureDefinitionMagicAffinity.RitualCasting, ritualSpellsCache);

                foreach (var ritualSpell in ritualSpellsCache)
                {
                    ritualSpells.TryAdd(ritualSpell);
                }
            }
        }
    }
}
