using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class NarrativeStateAnswerChoicePatcher
{
    private static readonly Dictionary<ulong, int> DiceRolls = new();

    [HarmonyPatch(typeof(NarrativeStateAnswerChoice), nameof(NarrativeStateAnswerChoice.Begin))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Begin_Patch
    {
        [UsedImplicitly]
        public static void Postfix()
        {
            var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

            DiceRolls.Clear();

            foreach (var gameLocationCharacter in gameLocationCharacterService.PartyCharacters)
            {
                var dieRoll = RuleDefinitions.RollDie(
                    RuleDefinitions.DieType.D20, RuleDefinitions.AdvantageType.None, out _, out _);

                DiceRolls.Add(gameLocationCharacter.Guid, dieRoll);

                Main.Info($"{gameLocationCharacter.Name} rolled a {dieRoll} for narrative choices");
            }
        }
    }

    [HarmonyPatch(typeof(NarrativeStateAnswerChoice), nameof(NarrativeStateAnswerChoice.GetVoteWinner))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GetVoteWinner_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(NarrativeStateAnswerChoice __instance, ref int selectedIndex, ref bool everyoneVoted)
        {
            if (!Main.Settings.EnableAlternateVotingSystem)
            {
                return true;
            }

            selectedIndex = 0;

            var maxWeightedVote = 0d;
            var computedVotes = 0;
            var networkingService = ServiceRepository.GetService<INetworkingService>();
            var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

            var playersInRoom = networkingService.GetPlayersInRoom();

            foreach (var current in __instance.playerVotesPerChoice)
            {
                var intList = current.Value;

                if (intList == null)
                {
                    continue;
                }

                var weightedVote = 0d;

                foreach (var _ in intList)
                {
                    ++computedVotes;

                    var hero = gameLocationCharacterService.PartyCharacters[current.Key];
                    var charismaModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                        hero.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Charisma));

                    weightedVote += charismaModifier * DiceRolls[hero.Guid];
                }

                if (weightedVote < maxWeightedVote)
                {
                    continue;
                }

                maxWeightedVote = weightedVote;

                var key = current.Key;

                selectedIndex = key;
            }

            everyoneVoted = playersInRoom.Count <= computedVotes;

            return false;
        }
    }
}
