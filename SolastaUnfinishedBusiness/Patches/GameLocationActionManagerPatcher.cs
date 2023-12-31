using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameLocationActionManagerPatcher
{
    //PATCH: supports `AddFighterLevelToIndomitableSavingReroll`
    [HarmonyPatch(typeof(GameLocationActionManager),
        nameof(GameLocationActionManager.ReactToIndomitableResistSavingThrow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ReactToIndomitableResistSavingThrow_Patch
    {
        [UsedImplicitly]
        public static void Prefix(CharacterActionParams reactionParams)
        {
            if (!Main.Settings.AddFighterLevelToIndomitableSavingReroll)
            {
                return;
            }

            var rulesetCharacter = reactionParams.ActingCharacter.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                CharacterContext.ConditionIndomitableSaving.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.Guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                CharacterContext.ConditionIndomitableSaving.Name,
                0,
                0,
                0);
        }
    }

    [HarmonyPatch(typeof(GameLocationActionManager), nameof(GameLocationActionManager.ReactToSpendSpellSlot))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ReactToSpendSpellSlot_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(GameLocationActionManager __instance, CharacterActionParams reactionParams)
        {
            //PATCH: replace `SpendSpellSlot` reaction with custom one
            __instance.AddInterruptRequest(new ReactionRequestSpendSpellSlotExtended(reactionParams));

            return false;
        }
    }

    [HarmonyPatch(typeof(GameLocationActionManager), nameof(GameLocationActionManager.ReactForOpportunityAttack))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ReactForOpportunityAttack_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(GameLocationActionManager __instance, CharacterActionParams reactionParams)
        {
            //PATCH: replace `OpportunityAttack` reaction with warcaster one

            //replace only for player characters
            if (reactionParams.ActingCharacter.Side != Side.Ally)
            {
                return true;
            }

            __instance.AddInterruptRequest(new ReactionRequestWarcaster(reactionParams));

            return false;
        }
    }

    [HarmonyPatch(typeof(GameLocationActionManager), nameof(GameLocationActionManager.ReactToSpendPower))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ReactToSpendPower_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(GameLocationActionManager __instance, CharacterActionParams reactionParams)
        {
            //PATCH: replace `SpendPower` reaction for bundled powers or customized one for other powers
            if (reactionParams.RulesetEffect is not RulesetEffectPower powerEffect)
            {
                return true;
            }

            if (powerEffect.PowerDefinition.IsBundlePower())
            {
                __instance.AddInterruptRequest(new ReactionRequestSpendBundlePower(reactionParams)
                {
                    Resource = powerEffect.PowerDefinition.GetFirstSubFeatureOfType<ICustomReactionResource>()
                });
            }
            else
            {
                __instance.AddInterruptRequest(new ReactionRequestSpendPowerCustom(reactionParams));
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(GameLocationActionManager), nameof(GameLocationActionManager.CharacterDamageReceivedAsync))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsAnyMetamagicOptionAvailable_Patch
    {
        [UsedImplicitly]
        public static IEnumerator Postfix(
            [NotNull] IEnumerator values,
            RulesetCharacter rulesetTarget,
            bool wasConscious,
            bool stillConscious,
            bool massiveDamage)
        {
            //PATCH: support for `DoNotTerminateWhileUnconscious`
            yield return DoNotTerminateWhileUnconscious.TerminateAllSpellsAndEffects(
                values, rulesetTarget, wasConscious, stillConscious, massiveDamage);
        }
    }

    //PATCH: ensure whoever reacts first will get the reaction handled first by game
    [HarmonyPatch(typeof(GameLocationActionManager), nameof(GameLocationActionManager.ProcessReactionRequest))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ProcessReactionRequest_Patch
    {
        // this is vanilla code except for the BEGIN END patch block
        [UsedImplicitly]
        public static bool Prefix(GameLocationActionManager __instance, ReactionRequest reactionRequest, bool validated)
        {
            __instance.ReactionRequestProcessed?.Invoke(reactionRequest, validated);

            if (!__instance.pendingReactionRequestGroups.Peek().Requests.Contains(reactionRequest))
            {
                if (!reactionRequest.Validated && reactionRequest.Processed)
                {
                    return false;
                }

                Trace.LogAssertion("Request hasn't been invalidated but not found in the top pending group");
            }

            var flag1 = false;

            if (validated && !reactionRequest.Processed)
            {
                reactionRequest.Validated = true;
                reactionRequest.OnSetValid();
                reactionRequest.Processed = true;

                if (reactionRequest.ValidationDismissesSimilarReactions)
                {
                    var reactionRequestGroup = __instance.pendingReactionRequestGroups.Peek();
                    var reactionRequestList = new List<ReactionRequest>();
                    reactionRequestList.AddRange(reactionRequestGroup.Requests);

                    foreach (var reactionRequest1 in reactionRequestList
                                 .Where(reactionRequest1 => reactionRequest1 != reactionRequest))
                    {
                        reactionRequest1.Validated = false;
                        reactionRequest1.OnSetInvalid();
                        reactionRequest1.Processed = true;
                        reactionRequest1.ReactionDismissForced?.Invoke(reactionRequest1);

                        flag1 = __instance.RemoveReactionRequest(reactionRequest1);

                        if (reactionRequest1.ReactionParams.RulesetEffect != null &&
                            reactionRequest1.ReactionParams.IsReactionEffect)
                        {
                            reactionRequest1.ReactionParams.RulesetEffect.Terminate(false);
                        }
                    }
                }
            }
            else
            {
                flag1 = __instance.RemoveReactionRequest(reactionRequest);

                if (reactionRequest.ReactionParams.RulesetEffect != null &&
                    reactionRequest.ReactionParams.IsReactionEffect)
                {
                    reactionRequest.ReactionParams.RulesetEffect.Terminate(false);
                }

                reactionRequest.OnSetInvalid();
            }

            reactionRequest.ReactionParams.ReactionValidated = reactionRequest.Validated;

            if (__instance.pendingReactionRequestGroups.Empty() || flag1)
            {
                return false;
            }

            var flag2 = true;
            var reactionRequestGroup1 = __instance.pendingReactionRequestGroups.Peek();

            foreach (var request in reactionRequestGroup1.Requests
                         .Where(request => !request.Processed))
            {
                flag2 = false;
            }

            if (!flag2)
            {
                return false;
            }

            //BEGIN PATCH
            reactionRequestGroup1.Requests.Sort((a, b) =>
            {
                a.Character.UsedSpecialFeatures.TryGetValue("ReactionTimestamp", out var aTimestamp);
                b.Character.UsedSpecialFeatures.TryGetValue("ReactionTimestamp", out var bTimestamp);

                return aTimestamp <= bTimestamp ? -1 : 1;
            });
            //END PATCH

            __instance.unstoppableCoroutines.Add(__instance.ExecuteReactionRequestGroupAsync(reactionRequestGroup1));

            return false;
        }
    }
}
