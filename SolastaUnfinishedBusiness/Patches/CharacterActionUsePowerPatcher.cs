using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomBehaviors;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionUsePowerPatcher
{
    [HarmonyPatch(typeof(CharacterActionUsePower), nameof(CharacterActionUsePower.CheckInterruptionBefore))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CheckInterruptionBefore_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] CharacterActionUsePower __instance)
        {
            //PATCH: ignores interruptions processing for certain powers so they won't interrupt invisibility
            return !Global.PowersThatIgnoreInterruptions.Contains(__instance.activePower.PowerDefinition);
        }
    }

    [HarmonyPatch(typeof(CharacterActionUsePower), nameof(CharacterActionUsePower.CheckInterruptionAfter))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CheckInterruptionAfter_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] CharacterActionUsePower __instance)
        {
            //PATCH: ignores interruptions processing for certain powers so they won't interrupt invisibility
            return !Global.PowersThatIgnoreInterruptions.Contains(__instance.activePower.PowerDefinition);
        }
    }

    [HarmonyPatch(typeof(CharacterActionUsePower), nameof(CharacterActionUsePower.GetAdvancementData))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GetAdvancementData_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] CharacterActionUsePower __instance)
        {
            //PATCH: Calculate advancement data for `RulesetEffectPowerWithAdvancement`
            return RulesetEffectPowerWithAdvancement.GetAdvancementData(__instance);
        }
    }

    [HarmonyPatch(typeof(CharacterActionUsePower), nameof(CharacterActionUsePower.HandleEffectUniqueness))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class HandleEffectUniqueness_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] CharacterActionUsePower __instance)
        {
            //PATCH: terminates all matching spells and powers of same group
            GlobalUniqueEffects.TerminateMatchingUniqueEffect(
                __instance.ActingCharacter.RulesetCharacter,
                __instance.activePower);

            //PATCH: Support for limited power effect instances
            //terminates earliest power effect instances of same limit, if limit reached
            //used to limit Inventor's infusions
            GlobalUniqueEffects.EnforceLimitedInstancePower(__instance);

            return false;
        }
    }

    [HarmonyPatch(typeof(CharacterActionUsePower), nameof(CharacterActionUsePower.SpendMagicEffectUses))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SpendMagicEffectUses_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] CharacterActionUsePower __instance)
        {
            //PATCH: we get an empty originItem under MP (GRENADIER) (MULTIPLAYER)
            if (__instance.activePower.OriginItem == null)
            {
                var provider = __instance.activePower.PowerDefinition.GetFirstSubFeatureOfType<PowerPoolDevice>();

                if (provider != null)
                {
                    __instance.activePower.originItem = provider.GetDevice(__instance.ActingCharacter.RulesetCharacter);
                }
            }

            //PATCH: Calculate extra charge usage for `RulesetEffectPowerWithAdvancement`
            if (__instance.activePower.OriginItem == null
                || __instance.activePower is not RulesetEffectPowerWithAdvancement power)
            {
                return true;
            }

            var usableDevice = power.OriginItem;

            foreach (var usableFunction in usableDevice.UsableFunctions
                         .Select(usableFunction => new
                         {
                             usableFunction, functionDescription = usableFunction.DeviceFunctionDescription
                         })
                         .Where(t =>
                             t.functionDescription.Type == DeviceFunctionDescription.FunctionType.Power &&
                             t.functionDescription.FeatureDefinitionPower == power.PowerDefinition)
                         .Select(t => t.usableFunction))
            {
                __instance.ActingCharacter.RulesetCharacter
                    .UseDeviceFunction(usableDevice, usableFunction, power.ExtraCharges);
                break;
            }

            ServiceRepository.GetService<IGameLocationActionService>()
                .ItemUsed?.Invoke(usableDevice.ItemDefinition.Name);

            return false;
        }
    }

    //PATCH: this is vanilla code with the exception it checks for sub classes repertoires as well to counter effects
    [HarmonyPatch(typeof(CharacterActionUsePower), nameof(CharacterActionUsePower.CounterEffectAction))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class HandleAttackerTriggeringPowerOnCharacterAttackHitConfirmed_Patch
    {
        [UsedImplicitly]
        public static IEnumerator Postfix(
            IEnumerator values,
            CharacterActionUsePower __instance,
            CharacterAction counterAction)
        {
            if (__instance.ActionParams.TargetAction == null)
            {
                yield break;
            }

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var effectForm in __instance.ActionParams.RulesetEffect.EffectDescription.EffectForms)
            {
                if (effectForm.FormType != EffectForm.EffectFormType.Counter)
                {
                    continue;
                }

                var counterForm = effectForm.CounterForm;

                if (__instance.ActionParams.TargetAction.ActionParams.RulesetEffect is not RulesetEffectSpell
                    counteredSpell)
                {
                    continue;
                }

                var counteredSpellDefinition = counteredSpell.SpellDefinition;
                var slotLevel = counteredSpell.SlotLevel;

                if (counterForm.AutomaticSpellLevel >= slotLevel)
                {
                    __instance.ActionParams.TargetAction.Countered = true;
                }
                else if (counterForm.CheckBaseDC != 0)
                {
                    var checkDC = counterForm.CheckBaseDC + slotLevel;

                    __instance.ActingCharacter.RulesetCharacter
                        .EnumerateFeaturesToBrowse<FeatureDefinitionMagicAffinity>(
                            __instance.ActingCharacter.RulesetCharacter.FeaturesToBrowse);

                    foreach (var featureDefinition in __instance
                                 .ActingCharacter.RulesetCharacter.FeaturesToBrowse)
                    {
                        var definitionMagicAffinity = (FeatureDefinitionMagicAffinity)featureDefinition;

                        if (definitionMagicAffinity.CounterspellAffinity == RuleDefinitions.AdvantageType.None)
                        {
                            continue;
                        }

                        var num =
                            definitionMagicAffinity.CounterspellAffinity == RuleDefinitions.AdvantageType.Advantage
                                ? 1
                                : -1;

                        __instance.ActionParams.ActionModifiers[0].AbilityCheckAdvantageTrends.Add(
                            new RuleDefinitions.TrendInfo(num,
                                RuleDefinitions.FeatureSourceType.CharacterFeature,
                                definitionMagicAffinity.Name, null));
                    }

                    if (counteredSpell.CounterAffinity != RuleDefinitions.AdvantageType.None)
                    {
                        var num = counteredSpell.CounterAffinity == RuleDefinitions.AdvantageType.Advantage
                            ? 1
                            : -1;

                        __instance.ActionParams.ActionModifiers[0].AbilityCheckAdvantageTrends.Add(
                            new RuleDefinitions.TrendInfo(num,
                                RuleDefinitions.FeatureSourceType.CharacterFeature,
                                counteredSpell.CounterAffinityOrigin, null));
                    }

                    var abilityScoreName = AttributeDefinitions.Charisma;

                    foreach (var spellRepertoire in
                             __instance.ActionParams.ActingCharacter.RulesetCharacter.SpellRepertoires
                                 .Where(spellRepertoire =>
                                     spellRepertoire.SpellCastingFeature.SpellCastingOrigin is
                                         FeatureDefinitionCastSpell.CastingOrigin.Class
                                         or FeatureDefinitionCastSpell.CastingOrigin.Subclass))
                    {
                        abilityScoreName = spellRepertoire.SpellCastingFeature.SpellcastingAbility;
                        break;
                    }

                    var proficiencyName = string.Empty;

                    if (counterForm.AddProficiencyBonus)
                    {
                        proficiencyName = "ForcedProficiency";
                    }

                    __instance.ActingCharacter.RollAbilityCheck(abilityScoreName, proficiencyName,
                        checkDC, RuleDefinitions.AdvantageType.None,
                        __instance.ActionParams.ActionModifiers[0], false, 0, out var outcome,
                        out var successDelta, true);

                    counterAction.AbilityCheckRollOutcome = outcome;
                    counterAction.AbilityCheckSuccessDelta = successDelta;

                    // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                    switch (counterAction.AbilityCheckRollOutcome)
                    {
                        case RuleDefinitions.RollOutcome.Failure:
                            yield return ServiceRepository.GetService<IGameLocationBattleService>()
                                .HandleFailedAbilityCheck(counterAction, __instance.ActingCharacter,
                                    __instance.ActionParams.ActionModifiers[0]);
                            break;
                        case RuleDefinitions.RollOutcome.Success:
                            __instance.ActionParams.TargetAction.Countered = true;
                            break;
                    }
                }

                if (!__instance.ActionParams.TargetAction.Countered ||
                    __instance.ActingCharacter.RulesetCharacter.SpellCounter == null)
                {
                    continue;
                }

                var unknown = string.IsNullOrEmpty(counteredSpell.IdentifiedBy);

                __instance.ActingCharacter.RulesetCharacter.SpellCounter(
                    __instance.ActingCharacter.RulesetCharacter,
                    __instance.ActionParams.TargetAction.ActingCharacter.RulesetCharacter,
                    counteredSpellDefinition, __instance.ActionParams.TargetAction.Countered,
                    unknown);
            }
        }
    }
}
