using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionRecklessAttackPatcher
{
    [HarmonyPatch(typeof(CharacterActionRecklessAttack), nameof(CharacterActionRecklessAttack.ExecuteImpl))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ExecuteImpl_Patch
    {
        [UsedImplicitly]
        public static IEnumerator Postfix(
            [NotNull] IEnumerator values,
            [NotNull] CharacterActionRecklessAttack __instance)
        {
            //PATCH: support for action switching
            yield return Process(__instance, values);
        }

        private static IEnumerator Process(
            // ReSharper disable once SuggestBaseTypeForParameter
            CharacterActionRecklessAttack action,
            IEnumerator values)
        {
            if (!Main.Settings.EnableBarbarianReckless2024)
            {
                yield return values;
                yield break;
            }

            if (action.ActingCharacter.RulesetCharacter.HasConditionOfType(
                    DatabaseHelper.ConditionDefinitions.ConditionReckless))
            {
                yield break;
            }

            var activeCondition1 = RulesetCondition.CreateActiveCondition(
                action.ActingCharacter.RulesetCharacter.Guid,
                DatabaseHelper.ConditionDefinitions.ConditionReckless,
                RuleDefinitions.DurationType.Round,
                0, RuleDefinitions.TurnOccurenceType.StartOfTurn,
                action.ActingCharacter.Guid,
                action.ActingCharacter.RulesetCharacter.CurrentFaction.Name);

            action.ActingCharacter.RulesetCharacter.AddConditionOfCategory(
                AttributeDefinitions.TagCombat, activeCondition1);

            var activeCondition2 = RulesetCondition.CreateActiveCondition(
                action.ActingCharacter.RulesetCharacter.Guid,
                DatabaseHelper.ConditionDefinitions.ConditionRecklessVulnerability,
                RuleDefinitions.DurationType.Round,
                0,
                RuleDefinitions.TurnOccurenceType.StartOfTurn,
                action.ActingCharacter.Guid,
                action.ActingCharacter.RulesetCharacter.CurrentFaction.Name);

            action.ActingCharacter.RulesetCharacter.AddConditionOfCategory(
                AttributeDefinitions.TagCombat, activeCondition2);
        }
    }
}
