using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class FunctorPatcher
{
    [HarmonyPatch(typeof(Functor), nameof(Functor.SelectCharacters))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SelectCharacters_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            [NotNull] FunctorParametersDescription functorParameters,
            List<GameLocationCharacter> selectedCharacters)
        {
            //PATCH: ensure conjured units teleport with the party
            if (functorParameters.CharacterLookUpMethod != FunctorDefinitions.CharacterLookUpMethod.AllPartyMembers)
            {
                return;
            }

            var characterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var len = functorParameters.PlayerPlacementMarkers.Length;
            var idx = 0;

            // only conjured units should teleport with the party
            foreach (var guestCharacter in characterService.GuestCharacters
                         .ToList()
                         .Where(x => x.RulesetCharacter.Tags.Contains(AttributeDefinitions.TagConjure)))
            {
                var rulesetCharacter = guestCharacter.RulesetCharacter;

                if (rulesetCharacter == null)
                {
                    continue;
                }

                if (!rulesetCharacter.ConditionsByCategory
                        .SelectMany(x => x.Value)
                        .Where(x =>
                            x.ConditionDefinition == DatabaseHelper.ConditionDefinitions.ConditionConjuredCreature)
                        .Any(x => characterService.PartyCharacters
                            .Any(y => y.RulesetCharacter.Guid == x.SourceGuid)))
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
}
