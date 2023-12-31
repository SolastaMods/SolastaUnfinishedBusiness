using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ReactionModalPatcher
{
    //TODO: Create a FeatureBuilder with Validators to create a generic check here
    [HarmonyPatch(typeof(ReactionModal), nameof(ReactionModal.ReactionTriggered))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ReactionTriggered_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(ReactionRequest request)
        {
            // wildshape heroes should not be able to cast spells
            var rulesetCharacter = request.Character.RulesetCharacter;

            if (!rulesetCharacter.IsSubstitute
                || request is not (ReactionRequestCastSpell or ReactionRequestCastFallPreventionSpell
                    or ReactionRequestCastImmunityToSpell))
            {
                return true;
            }

            ServiceRepository.GetService<ICommandService>().ProcessReactionRequest(request, false);
            return false;
        }
    }

    //PATCH: ensure whoever reacts first will get the reaction handled first by game
    [HarmonyPatch(typeof(ReactionModal), nameof(ReactionModal.OnReact))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnReact_Patch
    {
        [UsedImplicitly]
        public static void Prefix(CharacterReactionItem item)
        {
            var character = item.ReactionRequest.Character;
            var timestamp = (int)DateTime.Now.ToFileTimeUtc();

            if (!character.UsedSpecialFeatures.TryAdd("ReactionTimestamp", timestamp))
            {
                character.UsedSpecialFeatures["ReactionTimestamp"] = timestamp;
            }
        }
    }
}
