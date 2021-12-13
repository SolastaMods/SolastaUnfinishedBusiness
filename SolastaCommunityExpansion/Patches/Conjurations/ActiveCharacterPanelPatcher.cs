using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Conjurations
{
    [HarmonyPatch(typeof(ActiveCharacterPanel), "OnStopConcentratingCb")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ActiveCharacterPanel_OnStopConcentratingCb
    {
        internal static void Prefix(ActiveCharacterPanel __instance)
        {
            if(!Main.Settings.FullyControlAlliedConjurations || !Main.Settings.DismissControlledConjurationsWhenDeliberatelyDropConcentration)
            {
                return;
            }

            Main.Log($"ActiveCharacterPanel_OnStopConcentratingCb: {__instance.GuiCharacter.RulesetCharacter.ConcentratedSpell?.SpellDefinition.Name ?? "(none)"}");

            // Check we have a concentration spell
            var spell = __instance.GuiCharacter.RulesetCharacterHero?.ConcentratedSpell;
            if (spell == null)
            {
                Main.Log($"ActiveCharacterPanel_OnStopConcentratingCb: No spell.");
                return;
            }

            var spellDefinition = spell.SpellDefinition;

            // Only interested in specific summoning spells
            if(spellDefinition.Name.StartsWith("ConjureElemental") // TODO: ConjureFey
                && spellDefinition.SpellLevel >= 5
                && spellDefinition.EffectDescription.EffectForms
                    .SingleOrDefault(ef => ef.FormType == EffectForm.EffectFormType.Summon)
                    ?.SummonForm.PersistOnConcentrationLoss == true)
            {
                // locate and remove all beneficial conditions of type 'conjuration'
                foreach (var guid in __instance.GuiCharacter.RulesetCharacterHero.ConcentratedSpell.TrackedConditionGuids)
                {
                    if (RulesetEntity.TryGetEntity<RulesetCondition>(guid, out var condition))
                    {
                        Main.Log($"Condition={condition.Name}, target={condition.TargetGuid}");

                        if (RulesetEntity.TryGetEntity<RulesetCharacter>(condition.TargetGuid, out var monster))
                        {
                            Main.Log($"Monster={monster.Name}");

                            // clear all conditions.  Tried targetting just the conjuration condition but didn't work.
                            foreach (var rulesetCondition in monster.ConditionsByCategory.SelectMany(c => c.Value))
                            {
                                rulesetCondition.Clear();
                            }
                        }
                    }
                }
            }
        }
    }
}
