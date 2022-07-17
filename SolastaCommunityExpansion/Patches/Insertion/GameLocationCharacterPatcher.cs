using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Classes.Magus;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Insertion;

internal static class GameLocationCharacterPatcher
{
    [HarmonyPatch(typeof(GameLocationCharacter), "AttackOn")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class AttackOn
    {
        internal static void Prefix(
            [NotNull] GameLocationCharacter __instance,
            GameLocationCharacter target,
            RuleDefinitions.RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            var character = __instance.RulesetCharacter;

            if (character == null)
            {
                return;
            }

            var features = character.GetSubFeaturesByType<IOnAttackHitEffect>();

            foreach (var effect in features)
            {
                effect.BeforeOnAttackHit(__instance, target, outcome, actionParams, attackMode, attackModifier);
            }
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), "AttackImpactOn")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class AttackImpactOn
    {
        internal static void Prefix(
            [NotNull] GameLocationCharacter __instance,
            GameLocationCharacter target,
            RuleDefinitions.RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            var character = __instance.RulesetCharacter;
            if (character == null)
            {
                return;
            }

            var features = character.GetSubFeaturesByType<IOnAttackHitEffect>();
            foreach (var effect in features)
            {
                effect.AfterOnAttackHit(__instance, target, outcome, actionParams, attackMode, attackModifier);
            }
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), "HandleActionExecution")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationCharacter_HandleActionExecution
    {
        internal static void Postfix(
            GameLocationCharacter __instance,
            CharacterActionParams actionParams,
            ActionDefinitions.ActionScope scope)
        {
            if (scope != ActionDefinitions.ActionScope.Battle)
            {
                return;
            }

            if (actionParams.actionDefinition.Id != ActionDefinitions.Id.CastMain)
            {
                return;
            }

            var rulesetCharacter = actionParams.actingCharacter.RulesetCharacter;

            if (!rulesetCharacter.HasAnyFeature(Magus.SpellStrike))
            {
                return;
            }

            if (!Global.IsSpellStrike)
            {
                return;
            }

            __instance.UsedMainAttacks++;

            if (rulesetCharacter != null)
            {
                rulesetCharacter.ExecutedAttacks++;
                rulesetCharacter.RefreshAttackModes();
            }

            __instance.currentActionRankByType[ActionDefinitions.ActionType.Main]--;
            __instance.RefreshActionPerformances();
        }
    }
}
