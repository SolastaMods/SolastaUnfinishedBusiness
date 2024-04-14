using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Interfaces;

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

            var characterService = ServiceRepository.GetService<IGameLocationCharacterService>();

            if (!characterService.PartyCharacters.Contains(__instance.ActiveContender)
                && !characterService.GuestCharacters.Contains(__instance.ActiveContender))
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

            var characterService = ServiceRepository.GetService<IGameLocationCharacterService>();

            if (!characterService.PartyCharacters.Contains(__instance.ActiveContender)
                && !characterService.GuestCharacters.Contains(__instance.ActiveContender))
            {
                __result = __instance.PlayerContenders;
            }
        }
    }

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

            foreach (var (character, features) in __instance.InitiativeSortedContenders
                         .ToList()
                         .Select(character =>
                             (character, character.RulesetCharacter.GetSubFeaturesByType<IInitiativeEndListener>())))
            {
                //PATCH: supports `SenseNormalVisionRangeMultiplier`
                var multiplier = Main.Settings.SenseNormalVisionRangeMultiplier;

                if (multiplier > 0)
                {
                    var rulesetCharacter = character.RulesetCharacter;
                    var conditionName = $"ConditionSenseNormalVision{(multiplier == 1 ? 24 : 48)}";

                    rulesetCharacter.InflictCondition(
                        conditionName,
                        RuleDefinitions.DurationType.Irrelevant,
                        1,
                        RuleDefinitions.TurnOccurenceType.StartOfTurn,
                        AttributeDefinitions.TagEffect,
                        rulesetCharacter.guid,
                        rulesetCharacter.CurrentFaction.Name,
                        1,
                        conditionName,
                        0,
                        0,
                        0);
                }

                //PATCH: mainly supports Thief level 17th through ICharacterInitiativeEndListener interface
                foreach (var feature in features)
                {
                    yield return feature.OnInitiativeEnded(character);
                }
            }
        }
    }

    //PATCH: supports `SenseNormalVisionRangeMultiplier`
    [HarmonyPatch(typeof(GameLocationBattle), nameof(GameLocationBattle.IntroduceNewContender))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IntroduceNewContender_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GameLocationCharacter character)
        {
            var multiplier = Main.Settings.SenseNormalVisionRangeMultiplier;

            if (multiplier == 0)
            {
                return;
            }

            // don't use InflictCondition here to avoid a too soon character refresh
            var rulesetCharacter = character.RulesetCharacter;
            var conditionName = $"ConditionSenseNormalVision{(multiplier == 1 ? 24 : 48)}";
            var condition = DatabaseRepository.GetDatabase<ConditionDefinition>().GetElement(conditionName);
            var activeCondition = RulesetCondition.CreateActiveCondition(
                character.Guid,
                condition,
                RuleDefinitions.DurationType.Irrelevant,
                1,
                RuleDefinitions.TurnOccurenceType.StartOfTurn,
                character.Guid,
                rulesetCharacter.CurrentFaction.Name,
                effectDefinitionName: conditionName);

            rulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagEffect, activeCondition, false);
        }
    }
}
