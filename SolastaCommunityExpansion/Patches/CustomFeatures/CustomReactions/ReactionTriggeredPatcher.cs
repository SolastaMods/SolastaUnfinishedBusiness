using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomReactions;

[HarmonyPatch(typeof(ReactionModal), "ReactionTriggered")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class ReactionModal_ReactionTriggered
{
    public static bool Prefix(ReactionRequest request)
    {
        var isCtrlPressed = Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl);

        if (Main.Settings.EnableCtrlClickBypassAttackReactionPanel
            && isCtrlPressed
            && (!Main.Settings.EnableIgnoreCtrlClickOnCriticalHit || !Global.CriticalHit))
        {
            ServiceRepository.GetService<ICommandService>().ProcessReactionRequest(request, false);

            return false;
        }

        //
        // wildshape heroes should not be able to cast spells
        //
        var rulesetCharacter = request.Character.RulesetCharacter;

        if (rulesetCharacter.IsSubstitute
            && request is ReactionRequestCastSpell or ReactionRequestCastFallPreventionSpell
                or ReactionRequestCastImmunityToSpell)
        {
            ServiceRepository.GetService<ICommandService>().ProcessReactionRequest(request, false);
            return false;
        }

        //
        // TODO: Create a FeatureBuilder with Validators to create a generic check here
        //

        //
        // Tacticians heroes should only CounterStrike with melee weapons
        //
        if (request is not ReactionRequestCounterAttackWithPower || request.SuboptionTag != "CounterStrike" ||
            request.Character.RulesetCharacter is not RulesetCharacterHero hero || !hero.IsWieldingRangedWeapon())
        {
            return true;
        }

        ServiceRepository.GetService<ICommandService>().ProcessReactionRequest(request, false);
        return false;
    }
}
