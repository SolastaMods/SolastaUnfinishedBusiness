// keep this handy in case we ever need to reapply an effect that advances on CasterLevelTable

#if false
using System.Collections;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionReapplyEffectPatcher
{
    [HarmonyPatch(typeof(CharacterActionReapplyEffect), nameof(CharacterActionReapplyEffect.ExecuteImpl))]
    [UsedImplicitly]
    public static class ExecuteImpl_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            out IEnumerator __result,
            CharacterActionReapplyEffect __instance)
        {
            __result = ExecuteImpl(__instance);

            return false;
        }

        // mainly vanilla code except for BEGIN/END patch block
        private static IEnumerator ExecuteImpl(CharacterActionReapplyEffect actionReapplyEffect)
        {
            RulesetEffectSpell activeEffect = null;
            var rulesetCharacter = actionReapplyEffect.ActingCharacter.RulesetCharacter;

            foreach (var allCondition in actionReapplyEffect.ActingCharacter.RulesetCharacter.AllConditions
                         .Where(x =>
                             x.ConditionDefinition.Name == actionReapplyEffect.ActionDefinition.MatchingCondition))
            {
                foreach (var rulesetEffectSpell in actionReapplyEffect.ActingCharacter.RulesetCharacter.SpellsCastByMe
                             .Where(x => x.TrackedConditionGuids.Contains(allCondition.Guid)))
                {
                    activeEffect = rulesetEffectSpell;

                    foreach (var current2 in rulesetEffectSpell.TrackedConditionGuids)
                    {
                        if (RulesetEntity.TryGetEntity(current2, out RulesetCondition rulesetCondition) &&
                            (long)rulesetCondition.TargetGuid != (long)actionReapplyEffect
                                .ActingCharacter.RulesetCharacter.Guid)
                        {
                            RulesetEntity.TryGetEntity(rulesetCondition.TargetGuid, out rulesetCharacter);
                        }

                        break;
                    }
                }

                break;
            }

            if (activeEffect == null)
            {
                yield break;
            }

            var effectDescription = activeEffect.EffectDescription;
            var additionalDiceBySlotDelta = 0;

            //BEGIN PATCH

            //PATCH: supports CasterLevelTable with recurrent effects
            if (effectDescription.EffectAdvancement.EffectIncrementMethod == EffectIncrementMethod.CasterLevelTable)
            {
                var characterLevel = character.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
                
                additionalDiceBySlotDelta =
                    effectDescription.EffectAdvancement.ComputeAdditionalDiceByCasterLevel(characterLevel - 1)
            }
            else

            //END PATCH
            {
                var deltaLevel = activeEffect.SlotLevel - activeEffect.SpellDefinition.SpellLevel;

                additionalDiceBySlotDelta =
                    effectDescription.EffectAdvancement.ComputeAdditionalDiceBySlotDelta(deltaLevel);
            }

            var fromActor = GameLocationCharacter.GetFromActor(rulesetCharacter);
            var impactCenter = new Vector3();
            var identity = Quaternion.identity;
            var service = ServiceRepository.GetService<IGameLocationPositioningService>();

            service.ComputeImpactCenterPositionAndRotation(fromActor, ref impactCenter, ref identity);

            var impactPlanePosition = service.GetImpactPlanePosition(impactCenter);
            var data = new ActionDefinitions.MagicEffectCastData
            {
                Source = activeEffect.Name,
                EffectDescription = effectDescription,
                Caster = actionReapplyEffect.ActingCharacter,
                Targets = [],
                TargetIndex = 0,
                ImpactPoint = impactCenter,
                ImpactRotation = identity,
                ImpactPlanePoint = impactPlanePosition,
                ActionType = actionReapplyEffect.ActionType,
                ActionId = actionReapplyEffect.ActionId,
                IsDivertHit = false,
                ComputedTargetParameter = activeEffect.ComputeTargetParameter()
            };
            var magicEffectHitTarget =
                ServiceRepository.GetService<IGameLocationActionService>().MagicEffectHitTarget;

            magicEffectHitTarget?.Invoke(ref data);

            var formsParams = new RulesetImplementationDefinitions.ApplyFormsParams();

            formsParams.FillSourceAndTarget(actionReapplyEffect.ActingCharacter.RulesetCharacter, rulesetCharacter);
            formsParams.FillFromActiveEffect(activeEffect);
            formsParams.FillSpecialParameters(false, additionalDiceBySlotDelta, 0, 0, activeEffect.EffectLevel,
                new ActionModifier(), RollOutcome.Neutral, 0, false, 0, 1, null);
            formsParams.effectSourceType = activeEffect.EffectSourceType;

            ServiceRepository.GetService<IRulesetImplementationService>().ApplyEffectForms(
                effectDescription.EffectForms,
                formsParams,
                null,
                out _,
                out _,
                effectApplication: effectDescription.EffectApplication,
                filters: effectDescription.EffectFormFilters);
        }
    }
}
#endif
