using HarmonyLib;
using SolastaCommunityExpansion.Features;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures;

internal static class GameLocationCharacterPatcher
{
    [HarmonyPatch(typeof(GameLocationCharacter), "StartBattleTurn")]
    internal static class GameLocationCharacter_StartBattleTurn_Patch
    {
        internal static void Postfix(GameLocationCharacter __instance)
        {
            if (!__instance.Valid)
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
    internal static class GameLocationCharacter_EndBattleTurn_Patch
    {
        internal static void Postfix(GameLocationCharacter __instance)
        {
            if (!__instance.Valid)
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
    internal static class GameLocationCharacter_StartBattle
    {
        internal static void Postfix(GameLocationCharacter __instance, bool surprise)
        {
            if (!__instance.Valid)
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
    internal static class GameLocationCharacter_EndBattle
    {
        internal static void Postfix(GameLocationCharacter __instance)
        {
            if (!__instance.Valid)
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
}