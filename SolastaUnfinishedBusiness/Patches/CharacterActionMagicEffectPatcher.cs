using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionMagicEffectPatcher
{
    [HarmonyPatch(typeof(CharacterActionMagicEffect), "ForceApplyConditionOnSelf")]
    [UsedImplicitly]
    public static class ForceApplyConditionOnSelf_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] CharacterActionMagicEffect __instance)
        {
            //PATCH: compute effect level of forced on self conditions
            //used for Spirit Shroud to grant more damage with extra spell slots
            var actionParams = __instance.ActionParams;
            var effectDescription = actionParams.RulesetEffect.EffectDescription;

            if (!effectDescription.HasForceSelfCondition)
            {
                return true;
            }

            var service = ServiceRepository.GetService<IRulesetImplementationService>();
            var formsParams = new RulesetImplementationDefinitions.ApplyFormsParams();

            var effectLevel = 0;
            var effectSourceType = RuleDefinitions.EffectSourceType.Power;

            if (__instance is CharacterActionCastSpell spell)
            {
                effectSourceType = RuleDefinitions.EffectSourceType.Spell;
                effectLevel = spell.ActiveSpell.SlotLevel;
            }
            else if (__instance is CharacterActionUsePower power)
            {
                effectLevel = power.activePower.EffectLevel;
            }

            var character = __instance.ActingCharacter.RulesetCharacter;

            formsParams.FillSourceAndTarget(character, character);
            formsParams.FillFromActiveEffect(actionParams.RulesetEffect);
            formsParams.FillSpecialParameters(false, 0, 0, 0, effectLevel, null,
                RuleDefinitions.RollOutcome.Success, 0, false, 0, 1, null);
            formsParams.effectSourceType = effectSourceType;

            if (effectDescription.RangeType is RuleDefinitions.RangeType.MeleeHit or RuleDefinitions.RangeType.RangeHit)
            {
                formsParams.attackOutcome = RuleDefinitions.RollOutcome.Success;
            }

            service.ApplyEffectForms(effectDescription.EffectForms,
                formsParams,
                null,
                out _,
                forceSelfConditionOnly: true,
                effectApplication: effectDescription.EffectApplication,
                filters: effectDescription.EffectFormFilters,
                terminateEffectOnTarget: out _);

            return false;
        }
    }

    [HarmonyPatch(typeof(CharacterActionMagicEffect), "ExecuteImpl")]
    [UsedImplicitly]
    public static class ExecuteImpl_Patch
    {
        [UsedImplicitly]
        public static void Prefix([NotNull] CharacterActionMagicEffect __instance)
        {
            var definition = __instance.GetBaseDefinition();

            //PATCH: skip spell animation if this is "attack after cast" spell
            if (definition.HasSubFeatureOfType<IPerformAttackAfterMagicEffectUse>())
            {
                __instance.ActionParams.SkipAnimationsAndVFX = true;
            }
        }

        [UsedImplicitly]
        public static IEnumerator Postfix(
            [NotNull] IEnumerator values,
            CharacterActionMagicEffect __instance)
        {
            //PATCH: support for `IPerformAttackAfterMagicEffectUse` and `IChainMagicEffect` feature
            // enables to perform automatic attacks after spell cast (like for sunlight blade cantrip) and chain effects
            while (values.MoveNext())
            {
                yield return values.Current;
            }

            var definition = __instance.GetBaseDefinition();

            //TODO: add possibility to get attack via feature
            //TODO: add possibility to process multiple attack features
            var customFeature = definition.GetFirstSubFeatureOfType<IPerformAttackAfterMagicEffectUse>();

            CharacterActionAttack attackAction = null;

            var getAttackAfterUse = customFeature?.PerformAttackAfterUse;
            var attackOutcome = RuleDefinitions.RollOutcome.Neutral;
            var attacks = getAttackAfterUse?.Invoke(__instance);

            if (attacks is { Count: > 0 })
            {
                void AttackImpactStartHandler(
                    GameLocationCharacter attacker,
                    GameLocationCharacter defender,
                    RuleDefinitions.RollOutcome outcome,
                    CharacterActionParams actionParams,
                    RulesetAttackMode attackMode,
                    ActionModifier attackModifier)
                {
                    attackOutcome = outcome;
                }

                __instance.ActingCharacter.AttackImpactStart += AttackImpactStartHandler;

                foreach (var attackParams in attacks)
                {
                    attackAction = new CharacterActionAttack(attackParams);

                    var enums = attackAction.Execute();

                    while (enums.MoveNext())
                    {
                        yield return enums.Current;
                    }
                }

                __instance.ActingCharacter.AttackImpactStart -= AttackImpactStartHandler;
            }

            var saveRangeType = __instance.actionParams.activeEffect.EffectDescription.rangeType;

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            //chained effects would be useful for EOrb
            var chainAction = definition.GetFirstSubFeatureOfType<IChainMagicEffect>()
                ?.GetNextMagicEffect(__instance, attackAction, attackOutcome);

            if (chainAction != null)
            {
                var enums = chainAction.Execute();

                while (enums.MoveNext())
                {
                    yield return enums.Current;
                }
            }

            __instance.actionParams.activeEffect.EffectDescription.rangeType = saveRangeType;
        }
    }

    [HarmonyPatch(typeof(CharacterActionMagicEffect), "ApplyForms")]
    [HarmonyPatch(new[]
    {
        typeof(GameLocationCharacter), // caster
        typeof(RulesetEffect), // activeEffect
        typeof(int), // addDice,
        typeof(int), // addHP,
        typeof(int), // addTempHP,
        typeof(int), // effectLevel,
        typeof(GameLocationCharacter), // target,
        typeof(ActionModifier), // actionModifier,
        typeof(RuleDefinitions.RollOutcome), // outcome,
        typeof(bool), // criticalSuccess,
        typeof(bool), // rolledSaveThrow,
        typeof(RuleDefinitions.RollOutcome), // saveOutcome,
        typeof(int), // saveOutcomeDelta,
        typeof(int), // targetIndex,
        typeof(int), // totalTargetsNumber,
        typeof(RulesetItem), // targetITem,
        typeof(RuleDefinitions.EffectSourceType), // sourceType,
        typeof(int), // ref damageReceive
        typeof(bool), //out damageAbsorbedByTemporaryHitPoints
        typeof(bool) //out terminateEffectOnTarget
    }, new[]
    {
        ArgumentType.Normal, // caster
        ArgumentType.Normal, // activeEffect
        ArgumentType.Normal, // addDice,
        ArgumentType.Normal, // addHP,
        ArgumentType.Normal, // addTempHP,
        ArgumentType.Normal, // effectLevel,
        ArgumentType.Normal, // target,
        ArgumentType.Normal, // actionModifier,
        ArgumentType.Normal, // outcome,
        ArgumentType.Normal, // criticalSuccess,
        ArgumentType.Normal, // rolledSaveThrow,
        ArgumentType.Normal, // saveOutcome,
        ArgumentType.Normal, // saveOutcomeDelta,
        ArgumentType.Normal, // targetIndex,
        ArgumentType.Normal, // totalTargetsNumber,
        ArgumentType.Normal, // targetITem,
        ArgumentType.Normal, // sourceType,
        ArgumentType.Ref, //ref damageReceive
        ArgumentType.Out, //out damageAbsorbedByTemporaryHitPoints
        ArgumentType.Out //out terminateEffectOnTarget
    })]
    [UsedImplicitly]
    public static class ApplyForms_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: support for `PushesFromEffectPoint`
            // allows push/grab motion effects to work relative to casting point, instead of caster's position
            // used for Grenadier's force grenades
            // sets position of the formsParams to the first position from ActionParams, when applicable
            var method =
                typeof(PushesFromEffectPoint).GetMethod(nameof(PushesFromEffectPoint.SetPositionAndApplyForms),
                    BindingFlags.Static | BindingFlags.NonPublic);

            return instructions.ReplaceCall(
                "ApplyEffectForms",
                -1, "CharacterActionMagicEffect.ApplyForms",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, method));
        }
    }
}
