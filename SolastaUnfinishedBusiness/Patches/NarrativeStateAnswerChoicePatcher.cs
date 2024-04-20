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
        public static void Postfix(NarrativeStateAnswerChoice __instance)
        {
            if (!Global.IsMultiplayer ||
                !Main.Settings.EnableAlternateVotingSystem ||
                !Main.Settings.EnableSumD20OnAlternateVotingSystem)
            {
                return;
            }

            var characterService = ServiceRepository.GetService<IGameLocationCharacterService>();

            DiceRolls.Clear();

            var actorLine = string.Empty;

            if (__instance.narrativeSequence.AdventureLogInfos.Count > 0)
            {
                actorLine = __instance.narrativeSequence.AdventureLogInfos.Last().ActorLine;
            }

            var console = Gui.Game.GameConsole;
            var entry = new GameConsoleEntry(actorLine, console.consoleTableDefinition) { Indent = false };

            console.AddEntry(entry);

            foreach (var gameLocationCharacter in characterService.PartyCharacters)
            {
                var dieRoll = RuleDefinitions.RollDie(
                    RuleDefinitions.DieType.D20, RuleDefinitions.AdvantageType.None, out _, out _);

                DiceRolls.Add(gameLocationCharacter.Guid, dieRoll);

                entry = new GameConsoleEntry("Feedback/&NarrativeChoiceRoll",
                    console.consoleTableDefinition) { Indent = true };

                console.AddCharacterEntry(gameLocationCharacter.RulesetCharacter, entry);
                entry.AddParameter(
                    ConsoleStyleDuplet.ParameterType.Positive, Gui.FormatDieTitle(RuleDefinitions.DieType.D20));
                entry.AddParameter(
                    ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString());
                console.AddEntry(entry);
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
            if (!Global.IsMultiplayer ||
                !Main.Settings.EnableAlternateVotingSystem)
            {
                return true;
            }

            // compute weights using charisma modifier
            var computedVotes = 0;
            var networkingService = ServiceRepository.GetService<INetworkingService>();
            var characterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var playersInRoom = networkingService.GetPlayersInRoom();
            var votes = new Dictionary<int, int>();

            foreach (var current in __instance.playerVotesPerChoice)
            {
                var heroIndex = current.Key;
                var intList = current.Value;

                if (intList == null)
                {
                    continue;
                }

                var hero = characterService.PartyCharacters[heroIndex];
                var charismaModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                    hero.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Charisma));

                votes.TryAdd(heroIndex, 0);

                foreach (var _ in intList)
                {
                    ++computedVotes;

                    votes[heroIndex] += charismaModifier;
                }
            }

            // add D20 rolls
            if (Main.Settings.EnableSumD20OnAlternateVotingSystem)
            {
                foreach (var heroIndex in votes.Keys.ToList())
                {
                    var hero = characterService.PartyCharacters[heroIndex];

                    votes[heroIndex] += DiceRolls[hero.Guid];
                }
            }

            // determine highest selection
            var maxWeight = 0;

            selectedIndex = 0;

            foreach (var vote in votes)
            {
                var heroIndex = vote.Key;
                var weight = vote.Value;

                if (weight < maxWeight)
                {
                    continue;
                }

                maxWeight = weight;
                selectedIndex = heroIndex;
            }

            everyoneVoted = playersInRoom.Count <= computedVotes;

            return false;
        }
    }
}
