using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Api;

namespace SolastaCommunityExpansion.Patches.Bugfix;

// ensure conjured units teleport with the party
[HarmonyPatch(typeof(Functor), "SelectCharacters")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class Functor_SelectCharacters
{
    internal static void Postfix(
        FunctorParametersDescription functorParameters,
        List<GameLocationCharacter> selectedCharacters)
    {
        //
        // BUGFIX: conjured units teleport with party
        //

        if (functorParameters.CharacterLookUpMethod != FunctorDefinitions.CharacterLookUpMethod.AllPartyMembers)
        {
            return;
        }

        var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var len = functorParameters.PlayerPlacementMarkers.Length;
        var idx = 0;

        // only conjured units should teleport with the party
        foreach (var guestCharacter in gameLocationCharacterService.GuestCharacters
                     .Where(x => x.RulesetCharacter.Tags.Contains(AttributeDefinitions.TagConjure)))
        {
            var rulesetCharacter = guestCharacter.RulesetCharacter;

            if (rulesetCharacter == null)
            {
                continue;
            }

            if (!rulesetCharacter.ConditionsByCategory.Values.Select(rulesetConditions => rulesetConditions
                    .Where(x => x.ConditionDefinition ==
                                DatabaseHelper.ConditionDefinitions.ConditionConjuredCreature)
                    .Any(x => gameLocationCharacterService.PartyCharacters.Any(y =>
                        y.RulesetCharacter.Guid == x.SourceGuid))).Any(found => found))
            {
                continue;
            }

            var playerPlacementMarkers = functorParameters.PlayerPlacementMarkers;
            var newPlayerPlacementMarkers =
                playerPlacementMarkers.AddToArray(playerPlacementMarkers[idx++ % len]);

            functorParameters.playerPlacementMarkers = newPlayerPlacementMarkers;
            selectedCharacters.Add(guestCharacter);
        }
    }
}
