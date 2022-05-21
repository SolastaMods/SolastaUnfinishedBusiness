using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    [HarmonyPatch(typeof(GameLocationCharacter), "StartBattleTurn")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationCharacter_StartBattleTurn
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
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationCharacter_EndBattleTurn
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
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
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
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
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
