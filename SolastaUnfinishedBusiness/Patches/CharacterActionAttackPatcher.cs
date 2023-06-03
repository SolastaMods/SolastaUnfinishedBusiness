using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionAttackPatcher
{
    //PATCH: Adds support to IReactToAttackOnEnemyFinished, IReactToMyAttackFinished, IReactToAttackOnMeFinished, IReactToAttackOnMeOrAllyFinished
    [HarmonyPatch(typeof(CharacterActionAttack), nameof(CharacterActionAttack.ExecuteImpl))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ExecuteImpl_Patch
    {
        [UsedImplicitly]
        public static IEnumerator Postfix(
            [NotNull] IEnumerator values,
            [NotNull] CharacterActionAttack __instance)
        {
            var found = false;
            var gameLocationAttacker = __instance.ActingCharacter;

            GameLocationCharacter gameLocationDefender = null;
            CharacterActionParams actionParams = null;
            RulesetAttackMode attackMode = null;
            ActionModifier actionModifier = null;
            var rollOutcome = RollOutcome.Neutral;

            void AttackImpactStartHandler(
                GameLocationCharacter attacker,
                GameLocationCharacter defender,
                RollOutcome outcome,
                CharacterActionParams parameters,
                RulesetAttackMode mode,
                ActionModifier modifier)
            {
                found = true;
                gameLocationDefender = defender;
                rollOutcome = outcome;
                actionParams = parameters;
                attackMode = mode;
                actionModifier = modifier;
            }

            gameLocationAttacker.AttackImpactStart += AttackImpactStartHandler;

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            gameLocationAttacker.AttackImpactStart -= AttackImpactStartHandler;

            if (!found)
            {
                yield break;
            }

            //
            // IReactToMyAttackFinished
            //

            if (Gui.Battle != null &&
                gameLocationAttacker.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                gameLocationDefender.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
            {
                var attackerFeatures = gameLocationAttacker.RulesetCharacter
                    .GetSubFeaturesByType<IReactToMyAttackFinished>();

                foreach (var feature in attackerFeatures)
                {
                    yield return feature.HandleReactToMyAttackFinished(
                        gameLocationAttacker, gameLocationDefender, rollOutcome, actionParams, attackMode,
                        actionModifier);
                }
            }

            //
            // IReactToAttackOnEnemyFinished
            //

            if (Gui.Battle != null &&
                gameLocationDefender.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
            {
                foreach (var gameLocationAlly in Gui.Battle.AllContenders
                             .Where(x =>
                                 (x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                                  x.RulesetCharacter.HasAnyConditionOfType(ConditionMindControlledByCaster)) ||
                                 x.Side == gameLocationAttacker.Side)
                             .ToList())
                {
                    var allyFeatures =
                        gameLocationAlly.RulesetCharacter.GetSubFeaturesByType<IReactToAttackOnEnemyFinished>();

                    foreach (var feature in allyFeatures)
                    {
                        yield return feature.HandleReactToAttackOnEnemyFinished(
                            gameLocationAttacker, gameLocationAlly, gameLocationDefender, rollOutcome, actionParams,
                            attackMode,
                            actionModifier);

                        if (Gui.Battle == null)
                        {
                            yield break;
                        }
                    }
                }
            }

            //
            // IReactToAttackOnMeFinished
            //

            if (Gui.Battle != null &&
                gameLocationAttacker.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                gameLocationDefender.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
            {
                var defenderFeatures = gameLocationDefender.RulesetCharacter
                    .GetSubFeaturesByType<IReactToAttackOnMeFinished>();

                foreach (var feature in defenderFeatures)
                {
                    yield return feature.HandleReactToAttackOnMeFinished(
                        gameLocationAttacker, gameLocationDefender, rollOutcome, actionParams, attackMode,
                        actionModifier);
                }
            }

            //
            // IReactToAttackOnMeOrAllyFinished
            //

            // ReSharper disable once InvertIf
            if (Gui.Battle != null &&
                gameLocationDefender.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
            {
                foreach (var gameLocationAlly in Gui.Battle.GetOpposingContenders(gameLocationAttacker.Side)
                             .Where(x => x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                             .ToList())
                {
                    var allyFeatures = gameLocationAlly.RulesetCharacter
                        .GetSubFeaturesByType<IReactToAttackOnMeOrAllyFinished>();

                    foreach (var feature in allyFeatures)
                    {
                        yield return feature.HandleReactToAttackOnAllyFinished(
                            gameLocationAttacker, gameLocationAlly, gameLocationDefender, rollOutcome, actionParams,
                            attackMode,
                            actionModifier);
                    }
                }
            }
        }
    }
}
