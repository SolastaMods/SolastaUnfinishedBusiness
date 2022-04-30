using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaMulticlass.Patches.RitualCasting
{
    internal static class RitualSelectionPanelPatcher
    {
        // ensures ritual spells from all spell repertoires are made available
        [HarmonyPatch(typeof(RitualSelectionPanel), "Bind")]
        internal static class RitualSelectionPanelBind
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var enumerateUsableRitualSpellsMethod = typeof(RulesetCharacter).GetMethod("EnumerateUsableRitualSpells");
                var myEnumerateUsableRitualSpellsMethod = typeof(RitualSelectionPanelBind).GetMethod("EnumerateUsableRitualSpells");

                foreach (CodeInstruction instruction in instructions)
                {
                    if (instruction.Calls(enumerateUsableRitualSpellsMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Call, myEnumerateUsableRitualSpellsMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
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
}
