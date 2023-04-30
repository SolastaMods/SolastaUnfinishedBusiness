using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameLocationBattlePatcher
{
    //PATCH: EnableEnemiesControlledByPlayer
    [HarmonyPatch(typeof(GameLocationBattle), nameof(GameLocationBattle.GetMyContenders))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GetMyContenders_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GameLocationBattle __instance, ref List<GameLocationCharacter> __result)
        {
            if (!Main.Settings.EnableEnemiesControlledByPlayer || __instance == null)
            {
                return;
            }

            var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

            if (!gameLocationCharacterService.PartyCharacters.Contains(__instance.ActiveContender)
                && !gameLocationCharacterService.GuestCharacters.Contains(__instance.ActiveContender))
            {
                __result = __instance.EnemyContenders;
            }
        }
    }

    //PATCH: EnableEnemiesControlledByPlayer
    [HarmonyPatch(typeof(GameLocationBattle), nameof(GameLocationBattle.GetOpposingContenders))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GetOpposingContenders_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GameLocationBattle __instance, ref List<GameLocationCharacter> __result)
        {
            if (!Main.Settings.EnableEnemiesControlledByPlayer || __instance == null)
            {
                return;
            }

            var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

            if (!gameLocationCharacterService.PartyCharacters.Contains(__instance.ActiveContender)
                && !gameLocationCharacterService.GuestCharacters.Contains(__instance.ActiveContender))
            {
                __result = __instance.PlayerContenders;
            }
        }
    }

#if false
    // testing if possible for a hero to have more than one turn in a round for Thief 17th

    [HarmonyPatch(typeof(GameLocationBattle), nameof(GameLocationBattle.RollInitiative))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RollInitiative_Patch
    {
        [UsedImplicitly]
        public static IEnumerator Postfix(IEnumerator values, GameLocationBattle __instance)
        {
            while (values.MoveNext())
            {
                yield return values.Current;
            }
            
            __instance.initiativeSortedContenders.Add(__instance.playerContenders[0]);
        }
    }
#endif
}
