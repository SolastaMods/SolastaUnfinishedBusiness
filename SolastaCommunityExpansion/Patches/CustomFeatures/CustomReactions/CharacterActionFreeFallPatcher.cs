using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomReactions
{
    // allow character fall handler to account for custom fall prevention powers (and not only feather fall and boots)
    [HarmonyPatch(typeof(CharacterActionFreeFall), "HandleCharacterFall")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterActionFreeFall_HandleCharacterFall
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            CharacterActionFreeFall __instance)
        {
            while (values.MoveNext())
            {
                yield return values.Current;
            }

            var extraEvents = Process(__instance);

            while (extraEvents.MoveNext())
            {
                yield return extraEvents.Current;
            }
        }

        private static RulesetUsablePower GetReactionPowerToPreventFall(RulesetCharacter character, RulesetCharacter target)
        {
            var powers = character.UsablePowers.Where(power =>
            {
                //TODO: check if power has validators
                var effect = power.PowerDefinition.EffectDescription;

                var activationTime = power.PowerDefinition.ActivationTime;
                if (activationTime != RuleDefinitions.ActivationTime.Reaction)
                {
                    return false;
                }

                foreach (var form in effect.EffectForms)
                {
                    if (form.FormType != EffectForm.EffectFormType.Condition)
                    {
                        continue;
                    }

                    var conditionForm = form.ConditionForm;

                    if (conditionForm.Operation != ConditionForm.ConditionOperation.Add)
                    {
                        continue;
                    }

                    if (!conditionForm.ConditionDefinition.IsSubtypeOf("ConditionFeatherFalling"))
                    {
                        continue;
                    }

                    if (effect.TargetType == RuleDefinitions.TargetType.Self && character == target)
                    {
                        return true;
                    }

                    if (effect.TargetType == RuleDefinitions.TargetType.Individuals)
                    {
                        //TODO: check targeting side and range
                        return true;
                    }
                }

                return false;
            });

            return powers.FirstOrDefault();
        }

        private static IEnumerator Process(CharacterActionFreeFall __instance)
        {
            var fallingCharacter = __instance.ActingCharacter;
            var rulesetFallingCharacter = fallingCharacter.RulesetCharacter;

            if (rulesetFallingCharacter.HasConditionOfType("ConditionFeatherFalling"))
            {
                yield break;
            }

            var characterService = ServiceRepository.GetService<IGameLocationCharacterService>();

            foreach (var partyCharacter in characterService.PartyCharacters)
            {
                var rulesetPartyMember = partyCharacter.RulesetCharacter;

                if (rulesetPartyMember.IsDeadOrDyingOrUnconscious)
                {
                    continue;
                }

                if (partyCharacter.GetActionTypeStatus(ActionDefinitions.ActionType.Reaction) !=
                    ActionDefinitions.ActionStatus.Available)
                {
                    continue;
                }

                var usablePower = GetReactionPowerToPreventFall(rulesetPartyMember, rulesetFallingCharacter);

                if (usablePower == null)
                {
                    continue;
                }

                var reactionParams = new CharacterActionParams(partyCharacter, ActionDefinitions.Id.PowerReaction);
                reactionParams.TargetCharacters.Add(fallingCharacter);
                reactionParams.ActionModifiers.Add(new ActionModifier());
                var rulesService = ServiceRepository.GetService<IRulesetImplementationService>();
                reactionParams.RulesetEffect =
                    rulesService.InstantiateEffectPower(rulesetPartyMember, usablePower, false);
                reactionParams.StringParameter = usablePower.PowerDefinition.Name;
                reactionParams.IsReactionEffect = true;
                var actionService = ServiceRepository.GetService<IGameLocationActionService>();
                var previousReactionCount = actionService.PendingReactionRequestGroups.Count;

                actionService.ReactToUsePower(reactionParams, "" /*usable_power.PowerDefinition.Name*/);

                while (previousReactionCount < actionService.PendingReactionRequestGroups.Count)
                {
                    yield return null;
                }
            }
        }
    }
}
