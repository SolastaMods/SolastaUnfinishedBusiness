using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionMagicEffectPatcher
{
    [HarmonyPatch(typeof(CharacterActionMagicEffect),
        nameof(CharacterActionMagicEffect.ForceApplyConditionOrLightOnSelf))]
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
            var effectSourceType = EffectSourceType.Power;

            switch (__instance)
            {
                case CharacterActionCastSpell spell:
                    effectSourceType = EffectSourceType.Spell;
                    effectLevel = spell.ActiveSpell.SlotLevel;
                    break;
                case CharacterActionUsePower power:
                    effectLevel = power.activePower.EffectLevel;
                    break;
            }

            var character = __instance.ActingCharacter.RulesetCharacter;

            formsParams.FillSourceAndTarget(character, character);
            formsParams.FillFromActiveEffect(actionParams.RulesetEffect);
            formsParams.FillSpecialParameters(
                false, 0, 0, 0, effectLevel, null, RollOutcome.Success, 0, false, 0, 1, null);
            formsParams.effectSourceType = effectSourceType;

            if (effectDescription.RangeType is RangeType.MeleeHit or RangeType.RangeHit)
            {
                formsParams.attackOutcome = RollOutcome.Success;
            }

            service.ApplyEffectForms(effectDescription.EffectForms,
                formsParams,
                null,
                out _,
                forceSelfConditionOrLightOnly: true,
                effectApplication: effectDescription.EffectApplication,
                filters: effectDescription.EffectFormFilters,
                terminateEffectOnTarget: out _);

            return false;
        }
    }

    [HarmonyPatch(typeof(CharacterActionMagicEffect), nameof(CharacterActionMagicEffect.ExecuteImpl))]
    [UsedImplicitly]
    public static class ExecuteImpl_Patch
    {
        [UsedImplicitly]
        public static void Prefix([NotNull] CharacterActionMagicEffect __instance)
        {
            var definition = __instance.GetBaseDefinition();
            var actingCharacter = __instance.ActingCharacter;
            var actionParams = __instance.ActionParams;

            //PATCH: skip spell animation if this is "attack after cast" spell
            if (definition.HasSubFeatureOfType<IAttackAfterMagicEffect>())
            {
                actionParams.SkipAnimationsAndVFX = true;
            }

            //PATCH: support for Altruistic metamagic - add caster as first target if necessary
            var effect = actionParams.RulesetEffect;
            var baseEffectDescription = (effect.SourceDefinition as IMagicEffect)?.EffectDescription;
            var effectDescription = effect.EffectDescription;
            var targets = actionParams.TargetCharacters;

            if (!effectDescription.InviteOptionalAlly
                || baseEffectDescription?.TargetType != TargetType.Self
                || targets.Count <= 0
                || targets[0] == actingCharacter)
            {
                return;
            }

            targets.Insert(0, actingCharacter);
            actionParams.ActionModifiers.Insert(0, actionParams.ActionModifiers[0]);
        }

        [UsedImplicitly]
        public static IEnumerator Postfix(
            [NotNull] IEnumerator values,
            CharacterActionMagicEffect __instance)
        {
            var baseDefinition = __instance.GetBaseDefinition();

            //PATCH: illusionary spells against creatures with True Sight should automatically save
            if (baseDefinition is SpellDefinition { SchoolOfMagic: SchoolIllusion } &&
                __instance.ActionParams.TargetCharacters.Count > 0 &&
                baseDefinition != DatabaseHelper.SpellDefinitions.Silence)
            {
                var target = __instance.ActionParams.TargetCharacters[0];
                var rulesetTarget = target.RulesetCharacter;
                var senseMode = rulesetTarget.SenseModes.FirstOrDefault(x => x.SenseType == SenseMode.Type.Truesight);

                if (senseMode != null && __instance.ActingCharacter.IsWithinRange(target, senseMode.SenseRange))
                {
                    rulesetTarget.InflictCondition(
                        SrdAndHouseRulesContext.ConditionAutomaticSavingThrow.Name,
                        DurationType.Round,
                        0,
                        TurnOccurenceType.StartOfTurn,
                        AttributeDefinitions.TagEffect,
                        rulesetTarget.Guid,
                        rulesetTarget.CurrentFaction.Name,
                        1,
                        SrdAndHouseRulesContext.ConditionAutomaticSavingThrow.Name,
                        0,
                        0,
                        0);
                }
            }

            //PATCH: supports `IMagicEffectInitiatedByMe`
            // no need to check for gui.battle != null
            var magicEffectInitiatedByMe = baseDefinition.GetFirstSubFeatureOfType<IMagicEffectInitiatedByMe>();

            if (magicEffectInitiatedByMe != null)
            {
                yield return magicEffectInitiatedByMe.OnMagicEffectInitiatedByMe(__instance, baseDefinition);
            }

            // VANILLA EVENTS
            while (values.MoveNext())
            {
                yield return values.Current;
            }

            //PATCH: supports `IPerformAttackAfterMagicEffectUse`
            // no need to check for gui.battle != null
            var attackAfterMagicEffect = baseDefinition.GetFirstSubFeatureOfType<IAttackAfterMagicEffect>();

            if (attackAfterMagicEffect != null)
            {
                var performAttackAfterUse = attackAfterMagicEffect.PerformAttackAfterUse;
                var characterActionAttacks = performAttackAfterUse?.Invoke(__instance);

                if (characterActionAttacks != null)
                {
                    __instance.ResultingActions.AddRange(
                        characterActionAttacks.Select(attackParams => new CharacterActionAttack(attackParams)));
                }
            }

            //PATCH: supports `IMagicEffectFinishedByMe`
            // no need to check for gui.battle != null
            var magicEffectFinishedByMe = baseDefinition.GetFirstSubFeatureOfType<IMagicEffectFinishedByMe>();

            if (magicEffectFinishedByMe != null)
            {
                yield return magicEffectFinishedByMe.OnMagicEffectFinishedByMe(__instance, baseDefinition);
            }
        }
    }

    [HarmonyPatch(typeof(CharacterActionMagicEffect), nameof(CharacterActionMagicEffect.ApplyForms))]
    [HarmonyPatch([
        typeof(GameLocationCharacter), // caster
        typeof(RulesetEffect), // activeEffect
        typeof(int), // addDice,
        typeof(int), // addHP,
        typeof(int), // addTempHP,
        typeof(int), // effectLevel,
        typeof(GameLocationCharacter), // target,
        typeof(ActionModifier), // actionModifier,
        typeof(RollOutcome), // outcome,
        typeof(bool), // criticalSuccess,
        typeof(bool), // rolledSaveThrow,
        typeof(RollOutcome), // saveOutcome,
        typeof(int), // saveOutcomeDelta,
        typeof(int), // targetIndex,
        typeof(int), // totalTargetsNumber,
        typeof(RulesetItem), // targetITem,
        typeof(EffectSourceType), // sourceType,
        typeof(int), // ref damageReceive
        typeof(bool), //out damageAbsorbedByTemporaryHitPoints
        typeof(bool) //out terminateEffectOnTarget
    ], [
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
    ])]
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
                typeof(PushesOrDragFromEffectPoint).GetMethod(
                    nameof(PushesOrDragFromEffectPoint.SetPositionAndApplyForms),
                    BindingFlags.Static | BindingFlags.NonPublic);

            return instructions.ReplaceCall(
                "ApplyEffectForms",
                -1, "CharacterActionMagicEffect.ApplyForms",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, method));
        }
    }

    [HarmonyPatch(typeof(CharacterActionMagicEffect),
        nameof(CharacterActionMagicEffect.HandlePostApplyMagicEffectOnZoneOrTargets))]
    [UsedImplicitly]
    public static class HandlePostApplyMagicEffectOnZoneOrTargets_Patch
    {
        [UsedImplicitly]
        public static IEnumerator Postfix(
            [NotNull] IEnumerator values,
            CharacterActionMagicEffect __instance,
            IGameLocationBattleService battleService,
            GameLocationCharacter target)
        {
            while (values.MoveNext())
            {
                yield return values.Current;
            }

            var attacker = __instance.ActingCharacter;

            //PATCH: support for `IMagicalAttackFinishedByMe`
            // no need to check for gui.battle != null
            foreach (var magicalAttackFinishedByMe in attacker.RulesetCharacter
                         .GetSubFeaturesByType<IMagicalAttackFinishedByMe>())
            {
                yield return magicalAttackFinishedByMe.OnMagicalAttackFinishedByMe(__instance, attacker, target);
            }

            //PATCH: support for `IMagicalAttackFinishedByMeOrAlly`
            // should also happen outside battles
            var contenders =
                battleService.Battle?.AllContenders ??
                ServiceRepository.GetService<IGameLocationCharacterService>().PartyCharacters;

            foreach (var ally in contenders
                         .Where(x => x.Side == attacker.Side
                                     && x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                         .ToList()) // avoid changing enumerator
            {
                foreach (var magicalAttackFinishedByMeOrAlly in ally.RulesetCharacter
                             .GetSubFeaturesByType<IMagicalAttackFinishedByMeOrAlly>())
                {
                    yield return magicalAttackFinishedByMeOrAlly
                        .OnMagicalAttackFinishedByMeOrAlly(__instance, attacker, target, ally);
                }
            }
        }
    }
}
