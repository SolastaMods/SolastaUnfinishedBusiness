using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Subclasses;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetEffectPatcher
{
    //PATCH: supports Oath of Ancients / Oath of Dread level 20 powers
    [HarmonyPatch(typeof(RulesetEffect), nameof(RulesetEffect.ConditionSaveRerollRequested))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    // ReSharper disable once InconsistentNaming
    public static class ConditionSaveRerollRequested_Patch
    {
        private static bool TryGetCasterAndEffectForms(
            RulesetEffect __instance,
            out RulesetCharacter rulesetCharacter,
            out List<EffectForm> effectForms)
        {
            switch (__instance)
            {
                case RulesetEffectSpell rulesetEffectSpell:
                {
                    rulesetCharacter = rulesetEffectSpell.Caster;
                    effectForms = rulesetEffectSpell.EffectDescription.EffectForms;

                    return true;
                }
                case RulesetEffectPower rulesetEffectPower:
                {
                    rulesetCharacter = rulesetEffectPower.User;
                    effectForms = rulesetEffectPower.EffectDescription.EffectForms;

                    return true;
                }
                default:
                    rulesetCharacter = null;
                    effectForms = null;

                    return false;
            }
        }

        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var rollSavingThrowMethod = typeof(RulesetActor).GetMethod("RollSavingThrow");
            var myRollSavingThrowMethod =
                typeof(ConditionSaveRerollRequested_Patch).GetMethod("RollSavingThrow");
            //PATCH: make ISpellCastingAffinityProvider from dynamic item properties apply to repertoires
            return instructions
                .ReplaceCalls(rollSavingThrowMethod,
                    "RulesetEffect.ConditionSaveRerollRequested",
                    new CodeInstruction(OpCodes.Ldarg, 0),
                    new CodeInstruction(OpCodes.Call, myRollSavingThrowMethod));
        }

        [UsedImplicitly]
        public static void RollSavingThrow(
            RulesetCharacter __instance,
            int saveBonus,
            string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<RuleDefinitions.TrendInfo> modifierTrends,
            List<RuleDefinitions.TrendInfo> advantageTrends,
            int rollModifier,
            int saveDC,
            bool hasHitVisual,
            ref RuleDefinitions.RollOutcome outcome,
            ref int outcomeDelta,
            RulesetEffect rulesetEffect)
        {
            if (!TryGetCasterAndEffectForms(rulesetEffect, out var caster, out var effectForms))
            {
                __instance.RollSavingThrow(
                    saveBonus, abilityScoreName, sourceDefinition, modifierTrends, advantageTrends,
                    rollModifier, saveDC, hasHitVisual, out outcome, out outcomeDelta);

                return;
            }

            //PATCH: supports Oath of Ancients / Oath of Dread Path of The Savagery
            RulesetImplementationManagerPatcher.TryRollSavingThrow_Patch.OnRollSavingThrowOath(caster, __instance,
                sourceDefinition, OathOfAncients.ConditionElderChampionName,
                OathOfAncients.ConditionElderChampionEnemy);
            RulesetImplementationManagerPatcher.TryRollSavingThrow_Patch.OnRollSavingThrowOath(caster, __instance,
                sourceDefinition, OathOfDread.ConditionAspectOfDreadName,
                OathOfDread.ConditionAspectOfDreadEnemy);
            PathOfTheSavagery.OnRollSavingThrowFuriousDefense(__instance, ref abilityScoreName);

            //PATCH: supports `IRollSavingThrowInitiated` interface
            foreach (var rollSavingThrowInitiated in __instance.GetSubFeaturesByType<IRollSavingThrowInitiated>())
            {
                rollSavingThrowInitiated.OnSavingThrowInitiated(
                    caster,
                    __instance,
                    ref saveBonus,
                    ref abilityScoreName,
                    sourceDefinition,
                    modifierTrends,
                    advantageTrends,
                    ref rollModifier,
                    ref saveDC,
                    ref hasHitVisual,
                    outcome,
                    outcomeDelta,
                    effectForms);
            }

            __instance.RollSavingThrow(
                saveBonus, abilityScoreName, sourceDefinition, modifierTrends, advantageTrends,
                rollModifier, saveDC, hasHitVisual, out outcome, out outcomeDelta);

            //PATCH: supports `IRollSavingThrowFinished` interface
            foreach (var rollSavingThrowFinished in __instance.GetSubFeaturesByType<IRollSavingThrowFinished>())
            {
                rollSavingThrowFinished.OnSavingThrowFinished(
                    caster,
                    __instance,
                    saveBonus,
                    abilityScoreName,
                    sourceDefinition,
                    modifierTrends,
                    advantageTrends,
                    rollModifier,
                    saveDC,
                    hasHitVisual,
                    ref outcome,
                    ref outcomeDelta,
                    effectForms);
            }
        }
    }
}
