using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi.Battle
{
    [HarmonyPatch(typeof(ReactionModal), "ReactionTriggered")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ReactionModal_ReactionTriggered
    {
        public static bool Prefix(ReactionRequest request)
        {
            var isCtrlPressed = Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl);

            if (request is ReactionRequestSpendSpellSlot
                && Main.Settings.EnableCtrlClickBypassSmiteReactionPanel
                && isCtrlPressed)
            {
                ServiceRepository.GetService<ICommandService>().ProcessReactionRequest(request, false);
                return false;
            }

            //
            // wildshape heroes should not be able to cast spells
            //
            var rulesetCharacter = request.Character.RulesetCharacter;

            if (rulesetCharacter.IsSubstitute
                && (request is ReactionRequestCastSpell
                || request is ReactionRequestCastFallPreventionSpell
                || request is ReactionRequestCastImmunityToSpell))
            {
                ServiceRepository.GetService<ICommandService>().ProcessReactionRequest(request, false);
                return false;
            }

            return true;
        }
    }
}
