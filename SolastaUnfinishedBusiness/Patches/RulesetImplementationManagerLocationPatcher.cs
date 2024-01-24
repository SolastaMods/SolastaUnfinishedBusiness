using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomValidators;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetImplementationManagerLocationPatcher
{
    [HarmonyPatch(typeof(RulesetImplementationManagerLocation),
        nameof(RulesetImplementationManagerLocation.InstantiateEffectInvocation))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class InstantiateEffectInvocation_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            RulesetEffectSpell __result,
            RulesetInvocation invocation)
        {
            //PATCH: setup repertoire for spells cast through invocation 
            __result.spellRepertoire ??= invocation.invocationRepertoire;
        }
    }

    [HarmonyPatch(typeof(RulesetImplementationManagerLocation),
        nameof(RulesetImplementationManagerLocation.IsMetamagicOptionAvailable))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsMetamagicOptionAvailable_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            ref bool __result,
            RulesetEffectSpell rulesetEffectSpell,
            RulesetCharacter caster,
            MetamagicOptionDefinition metamagicOption,
            ref string failure)
        {
            //PATCH: support for custom metamagic
            var validator = metamagicOption.GetFirstSubFeatureOfType<ValidateMetamagicApplication>();
            validator?.Invoke(caster, rulesetEffectSpell, metamagicOption, ref __result, ref failure);
        }
    }

    [HarmonyPatch(typeof(RulesetImplementationManagerLocation),
        nameof(RulesetImplementationManagerLocation.IsSituationalContextValid))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsSituationalContextValid_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            ref bool __result,
            RulesetImplementationDefinitions.SituationalContextParams contextParams)
        {
            //PATCH: supports custom situational context
            //used to tweak `Reckless` feat to properly work with reach weapons
            //and for Blade Dancer subclass features
            __result = CustomSituationalContext.IsContextValid(contextParams, __result);
        }
    }

    [HarmonyPatch(typeof(RulesetImplementationManagerLocation),
        nameof(RulesetImplementationManagerLocation.InstantiateActiveDeviceFunction))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class InstantiateActiveDeviceFunction_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            RulesetImplementationManagerLocation __instance,
            ref RulesetEffect __result,
            RulesetCharacter user,
            RulesetItemDevice usableDevice,
            RulesetDeviceFunction usableDeviceFunction,
            int addedCharges,
            bool delayRegistration)
        {
            //PATCH: support `RulesetEffectPowerWithAdvancement` by creating custom instance when needed
            return RulesetEffectPowerWithAdvancement.InstantiateActiveDeviceFunction(__instance, ref __result, user,
                usableDevice, usableDeviceFunction, addedCharges, delayRegistration);
        }
    }


    [HarmonyPatch(typeof(RulesetImplementationManagerLocation),
        nameof(RulesetImplementationManagerLocation.ApplyMotionForm))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ApplyMotionForm_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(EffectForm effectForm, RulesetImplementationDefinitions.ApplyFormsParams formsParams)
        {
            //PATCH: support for `PushesFromEffectPoint`
            // allows push/grab motion effects to work relative to casting point, instead of caster's position
            // used for Grenadier's force grenades
            // if effect source definition has marker, and forms params have position, will try to push target from that point

            var useDefaultLogic = ForcePushOrDragFromEffectPoint.TryPushFromEffectTargetPoint(effectForm, formsParams);

            if (useDefaultLogic)
            {
                useDefaultLogic = CustomSwap(effectForm, formsParams);
            }

            return useDefaultLogic;
        }

        private static bool CustomSwap(EffectForm effectForm,
            RulesetImplementationDefinitions.ApplyFormsParams formsParams)
        {
            // Main.Log2($"CustomSwap", true);
            var motionForm = effectForm.MotionForm;

            if (motionForm.Type != (MotionForm.MotionType)ExtraMotionType.CustomSwap)
            {
                return true;
            }

            var action = ServiceRepository.GetService<IGameLocationActionService>();
            var attacker = GameLocationCharacter.GetFromActor(formsParams.sourceCharacter);
            var defender = GameLocationCharacter.GetFromActor(formsParams.targetCharacter);

            if (attacker == null || defender == null)
            {
                return true;
            }

            const ActionDefinitions.Id ACTION_ID = (ActionDefinitions.Id)ExtraActionId.PushedCustom;

            action.ExecuteAction(
                new CharacterActionParams(attacker, ACTION_ID, defender.LocationPosition)
                {
                    BoolParameter = false, BoolParameter4 = false, CanBeCancelled = false, CanBeAborted = false
                }, null, true);
            action.ExecuteAction(
                new CharacterActionParams(defender, ActionDefinitions.Id.Pushed, attacker.LocationPosition)
                {
                    BoolParameter = false, BoolParameter4 = false, CanBeCancelled = false, CanBeAborted = false
                }, null, false);

            return false;
        }
    }

    [HarmonyPatch(typeof(RulesetImplementationManagerLocation),
        nameof(RulesetImplementationManagerLocation.IsAnyMetamagicOptionAvailable))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsAnyMetamagicOptionAvailable_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: support for `ReplaceMetamagicOption`
            return ReplaceMetamagicOption.PatchMetamagicGetter(instructions,
                "RulesetImplementationManagerLocation.IsAnyMetamagicOptionAvailable");
        }
    }

    [HarmonyPatch(typeof(RulesetImplementationManagerLocation),
        nameof(RulesetImplementationManagerLocation.ApplyShapeChangeForm))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ApplyShapeChangeForm_Patch
    {
        private static readonly List<RulesetEffectPower> PowersUsedByMe = [];
        private static readonly List<RulesetEffectSpell> SpellsCastByMe = [];

        [UsedImplicitly]
        public static void Prefix(RulesetImplementationDefinitions.ApplyFormsParams formsParams)
        {
            var source = formsParams.sourceCharacter;

            //BUGFIX: allow Druids to keep concentration on spells / powers with proxy summon forms
            SpellsCastByMe.SetRange(source.SpellsCastByMe);
            source.SpellsCastByMe.Clear();

            PowersUsedByMe.SetRange(source.PowersUsedByMe);
            source.PowersUsedByMe.Clear();
        }

        [UsedImplicitly]
        public static void Postfix(
            RulesetImplementationManagerLocation __instance,
            RulesetImplementationDefinitions.ApplyFormsParams formsParams)
        {
            var source = formsParams.sourceCharacter;

            __instance.TryFindSubstituteOfCharacter(source, out var characterMonster);

            //BUGFIX: allow Druids to keep concentration on spells / powers with proxy summon forms

            //TODO: do I need to add them back to source?
            source.SpellsCastByMe.SetRange(SpellsCastByMe);
            source.PowersUsedByMe.SetRange(PowersUsedByMe);

            characterMonster.SpellsCastByMe.SetRange(SpellsCastByMe);
            characterMonster.PowersUsedByMe.SetRange(PowersUsedByMe);

            //PATCH: allows shape changers to get bonuses effects defined in features / feats / etc.
            var sourceAbilityBonus = formsParams.activeEffect.ComputeSourceAbilityBonus(source);
            var proficiencyBonus = formsParams.activeEffect.ComputeSourceProficiencyBonus(source);
            var creatureTags = formsParams.targetSubstitute.CreatureTags;

            foreach (var summoningAffinity in source
                         .GetFeaturesByType<FeatureDefinitionSummoningAffinity>()
                         .Where(x => creatureTags.Contains(x.RequiredMonsterTag)))
            {
                foreach (var addedCondition in summoningAffinity.AddedConditions)
                {
                    var sourceAmount = 0;

                    // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                    switch (addedCondition.AmountOrigin)
                    {
                        case ConditionDefinition.OriginOfAmount.SourceHalfHitPoints:
                            sourceAmount = addedCondition.BaseAmount +
                                           (source.TryGetAttributeValue(AttributeDefinitions.HitPoints) / 2);
                            break;
                        case ConditionDefinition.OriginOfAmount.SourceSpellCastingAbility:
                            var num1 = source.SpellRepertoires
                                .Select(spellRepertoire => AttributeDefinitions.ComputeAbilityScoreModifier(
                                    source.TryGetAttributeValue(spellRepertoire.SpellCastingAbility)))
                                .Prepend(0)
                                .Max();

                            sourceAmount = num1;
                            break;
                        case ConditionDefinition.OriginOfAmount.SourceSpellAttack:
                            var num2 = source.SpellRepertoires
                                .Select(spellRepertoire => spellRepertoire.SpellAttackBonus)
                                .Prepend(0)
                                .Max();

                            sourceAmount = num2;
                            break;
                    }

                    characterMonster.InflictCondition(
                        addedCondition.Name,
                        formsParams.durationType,
                        formsParams.durationParameter,
                        formsParams.endOfEffect,
                        AttributeDefinitions.TagEffect,
                        source.Guid,
                        source.CurrentFaction.Name,
                        formsParams.effectLevel,
                        string.Empty, sourceAmount,
                        sourceAbilityBonus,
                        proficiencyBonus);

                    // we need to re-assign max hit points as we're on a postfix
                    characterMonster.currentHitPoints =
                        characterMonster.GetAttribute(AttributeDefinitions.HitPoints).MaxValue;

                    characterMonster.RefreshAll();
                }
            }
        }
    }
}
