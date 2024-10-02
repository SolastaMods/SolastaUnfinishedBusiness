using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;

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

            ServiceRepository.GetService<IGameLocationActionService>().ProcessReactionRequest(request, false);
            return false;
        }
    }

    [HarmonyPatch(typeof(ReactionModal), nameof(ReactionModal.OnReact))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnReact_Patch
    {
        private const string ReactionTimestamp =
            GameLocationActionManagerPatcher.ExecuteReactionRequestGroupAsync_Patch.ReactionTimestamp;

        [UsedImplicitly]
        public static void Prefix(CharacterReactionItem item)
        {
            var caster = item.ReactionRequest.Character;

            //PATCH: register on acting character if SHIFT is pressed on reaction confirmations
            caster.RegisterShiftState();

            //PATCH: ensure whoever reacts first will get the reaction handled first by game
            var timestamp = (int)DateTime.Now.ToFileTimeUtc();

            if (!caster.UsedSpecialFeatures.TryAdd(ReactionTimestamp, timestamp))
            {
                caster.UsedSpecialFeatures[ReactionTimestamp] = timestamp;
            }
        }
    }
}
