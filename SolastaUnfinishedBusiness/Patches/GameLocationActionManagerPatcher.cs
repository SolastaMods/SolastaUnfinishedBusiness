using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class GameLocationActionManagerPatcher
{
    [HarmonyPatch(typeof(GameLocationActionManager), "ReactToSpendSpellSlot")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ReactToSpendSpellSlot_Patch
    {
        internal static bool Prefix(GameLocationActionManager __instance, CharacterActionParams reactionParams)
        {
            //PATCH: replace `SpendSpellSlot` reaction with custom one
            __instance.AddInterruptRequest(new ReactionRequestSpendSpellSlotExtended(reactionParams));

            return false;
        }
    }

    [HarmonyPatch(typeof(GameLocationActionManager), "ReactForOpportunityAttack")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ReactForOpportunityAttack_Patch
    {
        internal static bool Prefix(GameLocationActionManager __instance, CharacterActionParams reactionParams)
        {
            //PATCH: replace `OpportunityAttack` reaction with warcaster one
            __instance.AddInterruptRequest(new ReactionRequestWarcaster(reactionParams));

            return false;
        }
    }

    [HarmonyPatch(typeof(GameLocationActionManager), "ReactToSpendPower")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ReactToSpendPower_Patch
    {
        internal static bool Prefix(GameLocationActionManager __instance, CharacterActionParams reactionParams)
        {
            //PATCH: replace `SpendPower` reaction for bundled powers
            if (reactionParams.RulesetEffect is not RulesetEffectPower powerEffect ||
                !powerEffect.PowerDefinition.IsBundlePower())
            {
                return true;
            }

            __instance.AddInterruptRequest(new ReactionRequestSpendBundlePower(reactionParams));
            return false;
        }
    }

    [HarmonyPatch(typeof(GameLocationActionManager), "ReactForReadiedAction")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ReactForReadiedAction_Patch
    {
        internal static bool Prefix([NotNull] CharacterActionParams reactionParams)
        {
            //PATCH: Make cantrips that have more than 1 target hit same target more than once when used as readied action. 
            // Only Eldritch Blast and its variants should be affected

            // For some reason TA do not set reactionParams.ReadyActionType to ReadyActionType.Cantrip
            // So we manually detect it as casting spell level 0
            if (reactionParams.RulesetEffect is not RulesetEffectSpell { SlotLevel: 0 } spell)
            {
                return true;
            }

            var spellTargets = spell.ComputeTargetParameter();

            if (!reactionParams.RulesetEffect.EffectDescription.IsSingleTarget || spellTargets <= 1)
            {
                return true;
            }

            var target = reactionParams.TargetCharacters.FirstOrDefault();
            var mod = reactionParams.ActionModifiers.FirstOrDefault();

            while (target != null && mod != null && reactionParams.TargetCharacters.Count < spellTargets)
            {
                reactionParams.TargetCharacters.Add(target);
                // Technically casts after first might need to have different mods, but not by much since we attacking same target.
                reactionParams.ActionModifiers.Add(mod.Clone());
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(GameLocationActionManager), "ExecuteActionAsync")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ExecuteActionAsync_Patch
    {
        internal static IEnumerator Postfix(
            [NotNull] IEnumerator values,
            [NotNull] CharacterAction action)
        {
            Main.Logger.Log(action.ActionDefinition.Name);

            //PATCH: calls handlers for `ICustomOnActionFeature`
            var features = action.ActingCharacter.RulesetCharacter.GetSubFeaturesByType<ICustomOnActionFeature>();

            foreach (var feature in features)
            {
                feature.OnBeforeAction(action);
            }

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            foreach (var feature in features)
            {
                feature.OnAfterAction(action);
            }
        }
    }
}
