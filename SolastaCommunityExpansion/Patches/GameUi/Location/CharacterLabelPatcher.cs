using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.GameUi.Location
{
    // allows certain conditions to report on hero label
    [HarmonyPatch(typeof(CharacterLabel), "ConditionAdded")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameConsole_ConditionAdded
    {
        internal static void Prefix(CharacterLabel __instance, RulesetActor character, RulesetCondition condition)
        {
            if (Global.CharacterLabelEnabledConditions.Contains(condition.ConditionDefinition))
            {
                var displayConditionLabelMethod = typeof(CharacterLabel).GetMethod("DisplayConditionLabel", BindingFlags.NonPublic | BindingFlags.Instance);

                displayConditionLabelMethod.Invoke(__instance, new object[] { character, condition, false });
            }
        }
    }

    // allows certain conditions to report on hero label
    [HarmonyPatch(typeof(CharacterLabel), "ConditionRemoved")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameConsole_ConditionRemoved
    {
        internal static void Prefix(CharacterLabel __instance, RulesetActor character, RulesetCondition condition)
        {
            if (Global.CharacterLabelEnabledConditions.Contains(condition.ConditionDefinition))
            {
                var displayConditionLabelMethod = typeof(CharacterLabel).GetMethod("DisplayConditionLabel", BindingFlags.NonPublic | BindingFlags.Instance);

                displayConditionLabelMethod.Invoke(__instance, new object[] { character, condition, true });
            }
        }
    }
}
