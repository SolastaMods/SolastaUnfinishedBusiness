using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomAttacks;

[HarmonyPatch(typeof(GameLocationCharacter), "StartBattleTurn")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GameLocationCharacter_StartBattleTurn
{
    internal static void Postfix(GameLocationCharacter __instance)
    {
        if (__instance.destroying || __instance.destroyedBody)
        {
            return;
        }

        var character = __instance.RulesetCharacter;
        var listeners = character?.GetSubFeaturesByType<ICharacterTurnStartListener>();

        if (listeners == null)
        {
            return;
        }

        foreach (var listener in listeners)
        {
            listener.OnChracterTurnStarted(__instance);
        }
    }
}

[HarmonyPatch(typeof(GameLocationCharacter), "EndBattleTurn")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GameLocationCharacter_EndBattleTurn
{
    internal static void Postfix(GameLocationCharacter __instance)
    {
        if (__instance.destroying || __instance.destroyedBody)
        {
            return;
        }

        var character = __instance.RulesetCharacter;
        var listeners = character?.GetSubFeaturesByType<ICharacterTurnEndListener>();

        if (listeners == null)
        {
            return;
        }

        foreach (var listener in listeners)
        {
            listener.OnChracterTurnEnded(__instance);
        }
    }
}

[HarmonyPatch(typeof(GameLocationCharacter), "StartBattle")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GameLocationCharacter_StartBattle
{
    internal static void Postfix(GameLocationCharacter __instance, bool surprise)
    {
        if (__instance.destroying || __instance.destroyedBody)
        {
            return;
        }

        var character = __instance.RulesetCharacter;
        var listeners = character?.GetSubFeaturesByType<ICharacterBattlStartedListener>();

        if (listeners == null)
        {
            return;
        }

        foreach (var listener in listeners)
        {
            listener.OnChracterBattleStarted(__instance, surprise);
        }
    }
}

[HarmonyPatch(typeof(GameLocationCharacter), "EndBattle")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GameLocationCharacter_EndBattle
{
    internal static void Postfix(GameLocationCharacter __instance)
    {
        if (__instance.destroying || __instance.destroyedBody)
        {
            return;
        }

        var character = __instance.RulesetCharacter;
        var listeners = character?.GetSubFeaturesByType<ICharacterBattlEndedListener>();

        if (listeners == null)
        {
            return;
        }

        foreach (var listener in listeners)
        {
            listener.OnChracterBattleEnded(__instance);
        }
    }
}

// This is basically re-implemented base method, but with a twist - it can skip some attack modes before returning
// Used for displaying more than 1 attack mode per action panel
[HarmonyPatch(typeof(GameLocationCharacter), "FindActionAttackMode")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GameLocationCharacter_FindActionAttackMode
{
    internal static void Postfix(GameLocationCharacter __instance, ref RulesetAttackMode __result,
        ActionDefinitions.Id actionId)
    {
        var skip = __instance.GetSkipAttackModes();

        if (skip == 0)
        {
            return;
        }

        var skipped = 0;

        if (actionId != ActionDefinitions.Id.AttackMain && actionId != ActionDefinitions.Id.AttackOff &&
            actionId != ActionDefinitions.Id.AttackOpportunity && actionId != ActionDefinitions.Id.AttackReadied &&
            actionId != ActionDefinitions.Id.ReactionShot && actionId != ActionDefinitions.Id.Volley &&
            actionId != ActionDefinitions.Id.WhirlwindAttack)
        {
            return;
        }

        foreach (var attackMode in __instance.RulesetCharacter.AttackModes
                     .Where(attackMode => !attackMode.AfterChargeOnly &&
                                          ((attackMode.ActionType == ActionDefinitions.ActionType.Main &&
                                            (actionId == ActionDefinitions.Id.AttackMain ||
                                             actionId == ActionDefinitions.Id.AttackReadied ||
                                             actionId == ActionDefinitions.Id.Volley ||
                                             actionId == ActionDefinitions.Id.WhirlwindAttack)) ||
                                           (attackMode.ActionType == ActionDefinitions.ActionType.Bonus &&
                                            actionId == ActionDefinitions.Id.AttackOff) ||
                                           (attackMode.ActionType == ActionDefinitions.ActionType.Reaction &&
                                            actionId == ActionDefinitions.Id.AttackOpportunity) ||
                                           (attackMode.ActionType == ActionDefinitions.ActionType.Reaction &&
                                            actionId == ActionDefinitions.Id.ReactionShot))))
        {
            // The only difference is this condition
            if (skipped == skip)
            {
                __result = attackMode;
                break;
            }

            skipped++;
        }
    }
}
