using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class NarrativeStateAnswerChoicePatcher
{
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

                foreach (var vote in intList)
                {
                    ++computedVotes;

                    var charGuids = networkingService.GetCharactersGuidsByPlayer(playersInRoom[vote].actorNumber);
                    var controlledHeroes = gameLocationCharacterService.AllValidEntities
                        .Where(x => charGuids.Contains(x.Guid));
                    var charismaModifiers = controlledHeroes
                        .Select(y =>
                            AttributeDefinitions.ComputeAbilityScoreModifier(
                                y.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Charisma)));

                    weightedVote += charismaModifiers.Average() *
                                    (100f + RandomExtensions.RangeInclusive(0, Main.Settings.VotingSystemRandomRange)) / 100f;
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
