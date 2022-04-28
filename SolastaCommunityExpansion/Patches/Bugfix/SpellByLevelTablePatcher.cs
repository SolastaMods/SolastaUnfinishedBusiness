using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.BugFix
{
    /// <summary>
    /// Issue: WieldedConfigurationSelector.Bind passes character=null to mainHandSlotBox.Bind and offHandSlotBox.Bind
    /// Not fixed as of 1.3.40.
    /// </summary>
    [HarmonyPatch(typeof(SpellRepertoireLine), "FindAndSortRelevantSpells")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellsByLevelGroup_BindInspectionOrPreparation
    {
        public static void Prefix(List<SpellDefinition> spellDefinitions)
        {
            if (Main.Settings.BugFixHideReactionSpells)
            {
                spellDefinitions.RemoveAll(x => x.ActivationTime == RuleDefinitions.ActivationTime.Reaction);
            }
        }
    }
}
