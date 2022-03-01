using HarmonyLib;
using SolastaModApi;
using SolastaModApi.Infrastructure;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SolastaCommunityExpansion.Patches.Bugfix
{
    // ensure conjured units teleport with the party
    [HarmonyPatch(typeof(Functor), "SelectCharacters")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Functor_SelectCharacters
    {
        internal static void Postfix(FunctorParametersDescription functorParameters, List<GameLocationCharacter> selectedCharacters)
        {
            if (!Main.Settings.BugFixConjuredUnitsTeleportWithParty)
            {
                return;
            }

            if (functorParameters.CharacterLookUpMethod != FunctorDefinitions.CharacterLookUpMethod.AllPartyMembers)
            {
                return;
            }

            var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var len = functorParameters.PlayerPlacementMarkers.Length;
            var idx = 0;

            foreach (var guestCharacter in gameLocationCharacterService.GuestCharacters)
            {
                var rulesetCharacter = guestCharacter.RulesetCharacter;

                if (rulesetCharacter == null)
                {
                    continue;
                }

                foreach (var kvp in rulesetCharacter.ConditionsByCategory)
                {
                    var found = false;

                    foreach (var rulesetCondition in kvp.Value)
                    {
                        if (rulesetCondition.ConditionDefinition == DatabaseHelper.ConditionDefinitions.ConditionConjuredCreature
                            && gameLocationCharacterService.PartyCharacters.Any(x => x.RulesetCharacter.Guid == rulesetCondition.SourceGuid))
                        {
                            found = true;

                            break;
                        }
                    }

                    if (found)
                    {
                        var playerPlacementMarkers = functorParameters.PlayerPlacementMarkers;
                        var newPlayerPlacementMarkers = playerPlacementMarkers.AddToArray(playerPlacementMarkers[idx++ % len]);

                        functorParameters.SetField("playerPlacementMarkers", newPlayerPlacementMarkers);
                        selectedCharacters.Add(guestCharacter);

                        break;
                    }
                }
            }
        }
    }
}
