using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaModApi.Extensions;

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
            RuleDefinitions.RitualCasting ritualType,
            List<SpellDefinition> ritualSpells)
        {
            if (rulesetCharacter is not RulesetCharacterHero)
            {
                rulesetCharacter.EnumerateUsableRitualSpells(ritualType, ritualSpells);

                return;
            }

            ritualSpells.Clear();

            rulesetCharacter.EnumerateFeaturesToBrowse<FeatureDefinitionMagicAffinity>(rulesetCharacter.FeaturesToBrowse);

            foreach (FeatureDefinitionMagicAffinity featureDefinitionMagicAffinity in rulesetCharacter.FeaturesToBrowse)
            {
                var spellCastingClass = DatabaseRepository.GetDatabase<CharacterClassDefinition>()
                    .FirstOrDefault(x =>
                        x.FeatureUnlocks.Any(x =>
                            x.FeatureDefinition == featureDefinitionMagicAffinity
                            || (x.FeatureDefinition is FeatureDefinitionFeatureSet featureSet
                            && featureSet.FeatureSet.Contains(featureDefinitionMagicAffinity))));

                var spellCastingSubclass = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>()
                    .FirstOrDefault(x =>
                        x.FeatureUnlocks.Any(x =>
                            x.FeatureDefinition == featureDefinitionMagicAffinity
                            || (x.FeatureDefinition is FeatureDefinitionFeatureSet featureSet
                            && featureSet.FeatureSet.Contains(featureDefinitionMagicAffinity))));

                var spellRepertoire = rulesetCharacter.SpellRepertoires
                    .FirstOrDefault(x =>
                        x.SpellCastingClass == spellCastingClass
                        && x.SpellCastingSubclass == spellCastingSubclass);

                var maxSpellLevel = Models.SharedSpellsContext.GetClassSpellLevel(spellRepertoire);

                if (featureDefinitionMagicAffinity.RitualCasting == RuleDefinitions.RitualCasting.Prepared
                    && spellRepertoire.SpellCastingFeature.SpellReadyness == RuleDefinitions.SpellReadyness.Prepared
                    && spellRepertoire.SpellCastingFeature.SpellKnowledge == RuleDefinitions.SpellKnowledge.WholeList)
                {
                    var spells = spellRepertoire.PreparedSpells
                        .Where(s => s.Ritual)
                        .Where(s => maxSpellLevel >= s.SpellLevel);

                    ritualSpells.AddRange(spells);
                }
                else if (featureDefinitionMagicAffinity.RitualCasting == RuleDefinitions.RitualCasting.Spellbook
                        && spellRepertoire.SpellCastingFeature.SpellKnowledge == RuleDefinitions.SpellKnowledge.Spellbook)
                {
                    rulesetCharacter.CharacterInventory.EnumerateAllItems(rulesetCharacter.Items);

                    var spells = rulesetCharacter.Items
                        .OfType<RulesetItemSpellbook>()
                        .SelectMany(x => x.ScribedSpells)
                        .Where(s => s.Ritual)
                        .Where(s => maxSpellLevel >= s.SpellLevel);

                    rulesetCharacter.Items.Clear();

                    ritualSpells.AddRange(spells);
                }
                //
                // Special case for Witch
                //
                else if (featureDefinitionMagicAffinity.RitualCasting == (RuleDefinitions.RitualCasting)ExtraRitualCasting.Known)
                {
                    var spells = spellRepertoire.KnownSpells
                        .Where(s => s.Ritual)
                        .Where(s => maxSpellLevel >= s.SpellLevel);

                    ritualSpells.AddRange(spells);

                    if (spellRepertoire.AutoPreparedSpells == null)
                    {
                        return;
                    }

                    spells = spellRepertoire.AutoPreparedSpells
                        .Where(s => s.Ritual)
                        .Where(s => maxSpellLevel >= s.SpellLevel);

                    ritualSpells.AddRange(spells);
                }
            }

            rulesetCharacter.FeaturesToBrowse.Clear();
        }
    }
}
