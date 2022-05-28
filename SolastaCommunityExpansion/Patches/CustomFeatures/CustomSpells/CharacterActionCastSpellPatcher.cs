using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomSpells
{
    [HarmonyPatch(typeof(CharacterActionCastSpell), "ApplyMagicEffect")]
    [HarmonyPatch(
        new[]
        {
            typeof(GameLocationCharacter), typeof(ActionModifier), typeof(int), typeof(int),
            typeof(RuleDefinitions.RollOutcome), typeof(bool), typeof(RuleDefinitions.RollOutcome), typeof(int),
            typeof(int)
        },
        new[]
        {
            ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal,
            ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref
        })]
    internal static class CharacterActionCastSpell_ApplyMagicEffect
    {
        internal static bool Prefix(CharacterActionCastSpell __instance,
            GameLocationCharacter target,
            ActionModifier actionModifier,
            int targetIndex,
            int targetCount,
            RuleDefinitions.RollOutcome outcome,
            bool rolledSaveThrow,
            RuleDefinitions.RollOutcome saveOutcome,
            int saveOutcomeDelta,
            ref int damageReceived
        )
        {
            var activeSpell = __instance.ActiveSpell;
            var effectLevelProvider = activeSpell.SpellDefinition.GetFirstSubFeatureOfType<ICustomSpellEffectLevel>();

            if (effectLevelProvider == null) { return true; }

            var actingCharacter = __instance.ActingCharacter;
            var effectLevel = effectLevelProvider.GetEffectLevel(actingCharacter.RulesetActor);

            //Re-implementing CharacterActionMagicEffect.ApplyForms
            var formsParams = new RulesetImplementationDefinitions.ApplyFormsParams();
            formsParams.FillSourceAndTarget(actingCharacter.RulesetCharacter, target.RulesetActor);
            formsParams.FillFromActiveEffect(activeSpell);
            formsParams.FillSpecialParameters(
                rolledSaveThrow,
                __instance.GetProperty<int>("AddDice"),
                __instance.GetProperty<int>("AddHP"),
                __instance.GetProperty<int>("AddTempHP"),
                effectLevel,
                actionModifier,
                saveOutcome,
                saveOutcomeDelta,
                outcome == RuleDefinitions.RollOutcome.CriticalSuccess,
                targetIndex,
                targetCount,
                __instance.GetProperty<RulesetItem>("TargetItem")
            );
            formsParams.effectSourceType = RuleDefinitions.EffectSourceType.Spell;
            formsParams.targetSubstitute = __instance.ActionParams.TargetSubstitute;
            var rangeType = activeSpell.EffectDescription.RangeType;
            if (rangeType == RuleDefinitions.RangeType.MeleeHit || rangeType == RuleDefinitions.RangeType.RangeHit)
            {
                formsParams.attackOutcome = outcome;
            }

            var actualEffectForms = __instance.actualEffectForms;

            damageReceived = ServiceRepository.GetService<IRulesetImplementationService>()
                .ApplyEffectForms(actualEffectForms[targetIndex], formsParams);

            return false;
        }
    }
}
