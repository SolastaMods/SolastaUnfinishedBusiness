using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
        internal static ReactionRequest Request { get; private set; }

        [UsedImplicitly]
        public static bool Prefix(ReactionRequest request)
        {
            Request = request;

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

    //PATCH: ensure we correctly spend powers after they are used (BUGFIX)
    [HarmonyPatch(typeof(ReactionModal), nameof(ReactionModal.OnReact))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnReact_Patch
    {
        [UsedImplicitly]
        public static void Postfix()
        {
            if (ReactionTriggered_Patch.Request.ReactionParams.RulesetEffect is not RulesetEffectPower rulesetEffect ||
                rulesetEffect.PowerDefinition.ActivationTime != RuleDefinitions.ActivationTime.OnAttackHitMelee ||
                rulesetEffect.PowerDefinition.RechargeRate != RuleDefinitions.RechargeRate.TurnStart)
            {
                return;
            }

            var rulesetCharacter = ReactionTriggered_Patch.Request.Character.RulesetCharacter;

            var rulesetUsablePower =
                rulesetCharacter.UsablePowers.FirstOrDefault(
                    x => x.PowerDefinition == rulesetEffect.PowerDefinition);

            if (rulesetUsablePower != null)
            {
                ReactionTriggered_Patch.Request.Character.RulesetCharacter.UsePower(rulesetUsablePower);
            }
        }
    }
}
