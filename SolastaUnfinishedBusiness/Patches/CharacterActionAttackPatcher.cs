using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionAttackPatcher
{
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
            //PATCH: adds support for `IReactToAttackFinished` by calling `HandleReactToAttackFinished` on features
            var actingCharacter = __instance.ActingCharacter;
            var character = actingCharacter.RulesetCharacter;
            var found = false;

            GameLocationCharacter defender = null;

            var outcome = RuleDefinitions.RollOutcome.Neutral;

            CharacterActionParams actionParams = null;
            RulesetAttackMode mode = null;
            ActionModifier modifier = null;

            // ReSharper disable InconsistentNaming
            void AttackImpactStartHandler(
                GameLocationCharacter _,
                GameLocationCharacter _defender,
                RuleDefinitions.RollOutcome _outcome,
                CharacterActionParams _params,
                RulesetAttackMode _mode,
                ActionModifier _modifier)
            {
                found = true;
                defender = _defender;
                outcome = _outcome;
                actionParams = _params;
                mode = _mode;
                modifier = _modifier;
            }
            // ReSharper enable InconsistentNaming

            actingCharacter.AttackImpactStart += AttackImpactStartHandler;

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            actingCharacter.AttackImpactStart -= AttackImpactStartHandler;

            if (!found)
            {
                yield break;
            }

            var attackerFeatures = character?.GetSubFeaturesByType<IReactToMyAttackFinished>();

            if (attackerFeatures != null)
            {
                foreach (var feature in attackerFeatures)
                {
                    yield return feature.HandleReactToMyAttackFinished(
                        actingCharacter, defender, outcome, actionParams, mode, modifier);
                }
            }

            var defenderFeatures = defender.RulesetCharacter?.GetSubFeaturesByType<IReactToAttackOnMeFinished>();

            if (defenderFeatures != null)
            {
                foreach (var feature in defenderFeatures)
                {
                    yield return feature.HandleReactToAttackOnMeFinished(
                        actingCharacter, defender, outcome, actionParams, mode, modifier);
                }
            }

            // this happens on battle end
            if (Gui.Battle == null)
            {
                yield break;
            }

            foreach (var gameLocationAlly in Gui.Battle.GetOpposingContenders(actingCharacter.Side)
                         .Where(x => x != defender))
            {
                var allyFeatures =
                    gameLocationAlly.RulesetCharacter?.GetSubFeaturesByType<IReactToAttackOnAllyFinished>();

                if (allyFeatures == null)
                {
                    yield break;
                }

                foreach (var feature in allyFeatures)
                {
                    yield return feature.HandleReactToAttackOnAllyFinished(
                        actingCharacter, gameLocationAlly, defender, outcome, actionParams, mode, modifier);
                }
            }
        }
    }
}
