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
    //PATCH: Adds support to IAttackFinishedOnEnemy
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
            // IAttackFinishedOnEnemy
            //

            if (Gui.Battle != null &&
                gameLocationDefender.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
            {
                foreach (var gameLocationAlly in Gui.Battle.AllContenders
                             .Where(x =>
                                 (x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                                  x.RulesetCharacter.HasAnyConditionOfType(ConditionMindControlledByCaster)) ||
                                 x.Side == gameLocationAttacker.Side)
                             .ToList()) // avoid changing enumerator
                {
                    var allyFeatures =
                        gameLocationAlly.RulesetCharacter.GetSubFeaturesByType<IAttackFinishedOnEnemy>();

                    foreach (var feature in allyFeatures)
                    {
                        yield return feature.OnAttackFinishedOnEnemy(
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
        }
    }
}
