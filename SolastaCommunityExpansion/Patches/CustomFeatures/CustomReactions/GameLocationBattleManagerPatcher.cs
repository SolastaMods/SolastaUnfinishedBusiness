using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomReactions;

[HarmonyPatch(typeof(GameLocationBattleManager), "HandleFailedSavingThrow")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GameLocationBattleManager_HandleFailedSavingThrow
{
    internal static IEnumerator Postfix(IEnumerator values,
        GameLocationBattleManager __instance,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier saveModifier,
        bool hasHitVisual,
        bool hasBorrowedLuck
    )
    {
        while (values.MoveNext())
        {
            yield return values.Current;
        }

        var saveOutcome = action.SaveOutcome;

        if (!IsFailed(saveOutcome))
        {
            yield break;
        }

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var rulesService = ServiceRepository.GetService<IRulesetImplementationService>();
        var rulesetDefender = defender.RulesetCharacter;
        var features = rulesetDefender.GetSubFeaturesByType<IUsePowerToRerollFailedSave>();
        var actionParams = action.ActionParams;

        foreach (var feature in features)
        {
            var power = feature.GetPowerToRerollFailedSave(rulesetDefender, saveOutcome);

            if (!rulesetDefender.CanUsePower(power))
            {
                continue;
            }

            var usablePower = UsablePowersProvider.Get(power, rulesetDefender);

            var reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.SpendPower)
            {
                StringParameter = feature.ReactionName,
                RulesetEffect = rulesService.InstantiateEffectPower(rulesetDefender, usablePower, false)
            };

            var count = actionService.PendingReactionRequestGroups.Count;
            actionService.ReactToSpendPower(reactionParams);

            yield return __instance.WaitForReactions(defender, actionService, count);

            if (reactionParams.ReactionValidated)
            {
                GameConsoleHelper.LogCharacterUsedPower(rulesetDefender, power, indent: true);
                action.RolledSaveThrow = actionParams.RulesetEffect == null
                    ? actionParams.AttackMode.TryRollSavingThrow(attacker.RulesetCharacter,
                        defender.RulesetActor, saveModifier,
                        actionParams.AttackMode.EffectDescription.EffectForms, out saveOutcome,
                        out var saveOutcomeDelta)
                    : actionParams.RulesetEffect.TryRollSavingThrow(attacker.RulesetCharacter, attacker.Side,
                        defender.RulesetActor, saveModifier, hasHitVisual, out saveOutcome, out saveOutcomeDelta);
                action.SaveOutcome = saveOutcome;
                action.SaveOutcomeDelta = saveOutcomeDelta;
            }

            if (!IsFailed(saveOutcome))
            {
                yield break;
            }
        }
    }

    private static bool IsFailed(RuleDefinitions.RollOutcome outcome)
    {
        return outcome != RuleDefinitions.RollOutcome.Failure && outcome != RuleDefinitions.RollOutcome.CriticalFailure;
    }
}
