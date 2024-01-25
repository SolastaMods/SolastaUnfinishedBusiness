using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.BehaviorsGeneric;
using SolastaUnfinishedBusiness.BehaviorsSpecific;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
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
            yield return RestrictEffectToNotTerminateWhileUnconscious.TerminateAllSpellsAndEffects(
                values, rulesetTarget, wasConscious, stillConscious, massiveDamage);
        }
    }

    [HarmonyPatch(typeof(GameLocationActionManager),
        nameof(GameLocationActionManager.ExecuteReactionRequestGroupAsync))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ExecuteReactionRequestGroupAsync_Patch
    {
        public const string ReactionTimestamp = "ReactionTimestamp";

        [UsedImplicitly]
        public static IEnumerator Postfix(
            [NotNull] IEnumerator values,
            ReactionRequestGroup reactionRequestGroup)
        {
            //PATCH: ensure whoever reacts first will get the reaction handled first by game
            if (!Global.IsMultiplayer)
            {
                reactionRequestGroup.Requests.Sort((a, b) =>
                {
                    a.Character.UsedSpecialFeatures.TryGetValue(ReactionTimestamp, out var aTimestamp);
                    b.Character.UsedSpecialFeatures.TryGetValue(ReactionTimestamp, out var bTimestamp);

                    return aTimestamp <= bTimestamp ? -1 : 1;
                });
            }

            while (values.MoveNext())
            {
                yield return values.Current;
            }
        }
    }
}
