using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.FullyControlConjurations
{
    [HarmonyPatch(typeof(ActiveCharacterPanel), "OnStopConcentratingCb")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ActiveCharacterPanel_OnStopConcentratingCb
    {
        /// <summary>
        /// Set both summoned Elemental and Feys to be dismissed or turn hostile
        /// when deliberately dropping concentration (as per my understanding of SRD).
        /// TA implementation is Elemental=hostile, Fey=dismissed.
        /// Elemental and Fey turn hostile when concentration broken by enemy or when casting another concentration spell
        /// without first dropping concentration.
        /// </summary>
        internal static void Prefix(ActiveCharacterPanel __instance)
        {
            if (!Main.Settings.FullyControlConjurations)
            {
                return;
            }

            // Check we have a concentration spell
            var spell = __instance.GuiCharacter.RulesetCharacterHero?.ConcentratedSpell;
            if (spell == null)
            {
                Main.Log("ActiveCharacterPanel_OnStopConcentratingCb: No spell.");
                return;
            }

            Main.Log($"ActiveCharacterPanel_OnStopConcentratingCb: {spell.SpellDefinition.Name}");

            var spellDefinition = spell.SpellDefinition;

            if (spellDefinition.Name.StartsWith("ConjureElemental") || spellDefinition.Name.StartsWith("ConjureFey"))
            {
                foreach (var guid in __instance.GuiCharacter.RulesetCharacterHero.ConcentratedSpell.TrackedConditionGuids)
                {
                    if (RulesetEntity.TryGetEntity<RulesetCondition>(guid, out var condition))
                    {
                        Main.Log($"Condition={condition.Name}, target={condition.TargetGuid}");

                        if (RulesetEntity.TryGetEntity<RulesetCharacter>(condition.TargetGuid, out var monster)
                            && monster.TryGetConditionOfCategoryAndType(
                                AttributeDefinitions.TagConjure, RuleDefinitions.ConditionConjuredCreature, out var conjuredCreatureCondition))
                        {
                            conjuredCreatureCondition.TerminationKillsConjured = Main.Settings.DismissControlledConjurationsWhenDeliberatelyDropConcentration;
                        }
                    }
                }
            }
        }
    }
}
