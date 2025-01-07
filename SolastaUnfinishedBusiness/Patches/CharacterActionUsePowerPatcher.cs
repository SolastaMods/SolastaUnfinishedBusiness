using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Interfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionUsePowerPatcher
{
    private static bool IgnoreInterruptionProcessForPowerFunction(CharacterActionUsePower __instance)
    {
        var isPowerFunction = __instance.ActionParams.RulesetEffect.Name.Contains("PowerFunction");

        if (isPowerFunction && Main.Settings.KeepInvisibilityWhenUsingItems)
        {
            return false;
        }

        return !__instance.ActionParams.RulesetEffect.SourceDefinition
            .HasSubFeatureOfType<IIgnoreInvisibilityInterruptionCheck>();
    }

    [HarmonyPatch(typeof(CharacterActionUsePower), nameof(CharacterActionUsePower.CheckInterruptionBefore))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CheckInterruptionBefore_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] CharacterActionUsePower __instance)
        {
            //PATCH: ignores interruptions processing for certain powers so they won't interrupt invisibility
            return IgnoreInterruptionProcessForPowerFunction(__instance);
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
            return IgnoreInterruptionProcessForPowerFunction(__instance);
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
            ForceGlobalUniqueEffects.TerminateMatchingUniqueEffect(
                __instance.ActingCharacter.RulesetCharacter, __instance.actionParams.RulesetEffect);

            //PATCH: Support for limited power effect instances
            //terminates earliest power effect instances of same limit, if limit reached
            //used to limit Inventor's infusions
            ForceGlobalUniqueEffects.EnforceLimitedInstancePower(__instance);

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
            if (Global.IsMultiplayer &&
                __instance.activePower.OriginItem == null &&
                __instance.ActingCharacter.RulesetCharacter is RulesetCharacterHero)
            {
                var provider = __instance.activePower.PowerDefinition.GetFirstSubFeatureOfType<PowerPoolDevice>();

                if (provider != null)
                {
                    __instance.activePower.originItem = provider.GetDevice(__instance.ActingCharacter.RulesetCharacter);
                }
            }

            //PATCH: Calculate extra charge usage for `RulesetEffectPowerWithAdvancement`
            if (__instance.actionParams.RulesetEffect.OriginItem == null ||
                __instance.actionParams.RulesetEffect is not RulesetEffectPowerWithAdvancement power)
            {
                return true;
            }

            CalculateExtraChargeUsage(__instance, power);

            return false;
        }

        private static void CalculateExtraChargeUsage(
            CharacterActionUsePower __instance, RulesetEffectPowerWithAdvancement power)
        {
            var usableDevice = power.OriginItem;
            var usableFunction = usableDevice.UsableFunctions
                .Select(usableFunction => new
                {
                    usableFunction, functionDescription = usableFunction.DeviceFunctionDescription
                })
                .Where(t =>
                    t.functionDescription.Type == DeviceFunctionDescription.FunctionType.Power &&
                    t.functionDescription.FeatureDefinitionPower == power.PowerDefinition)
                .Select(t => t.usableFunction)
                .FirstOrDefault();

            if (usableFunction != null)
            {
                __instance.ActingCharacter.RulesetCharacter
                    .UseDeviceFunction(usableDevice, usableFunction, power.ExtraCharges);
            }

            ServiceRepository.GetService<IGameLocationActionService>()
                .ItemUsed?.Invoke(usableDevice.ItemDefinition.Name);
        }
    }

    //PATCH: allow check reactions on cast spell regardless of success / failure
    [HarmonyPatch(typeof(CharacterActionUsePower), nameof(CharacterActionUsePower.CounterEffectAction))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CounterEffectAction_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            ref IEnumerator __result, CharacterActionUsePower __instance, CharacterAction counterAction)
        {
            __result = Process(__instance, counterAction);

            return false;
        }

        private static IEnumerator Process(CharacterActionUsePower actionUsePower, CharacterAction counterAction)
        {
            if (actionUsePower.ActionParams.TargetAction == null)
            {
                yield break;
            }

            var actingCharacter = actionUsePower.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var actionParams = actionUsePower.ActionParams.TargetAction.ActionParams;
            var actionModifier = actionParams.ActionModifiers[0];

            foreach (var effectForm in actionParams.RulesetEffect.EffectDescription.EffectForms)
            {
                if (effectForm.FormType != EffectForm.EffectFormType.Counter)
                {
                    continue;
                }

                var counterForm = effectForm.CounterForm;
                var counteredSpell = actionParams.TargetAction.ActionParams.RulesetEffect as RulesetEffectSpell;
                var counteredSpellDefinition = counteredSpell!.SpellDefinition;
                var slotLevel = counteredSpell.SlotLevel;

                if (counterForm.AutomaticSpellLevel >= slotLevel)
                {
                    actionUsePower.ActionParams.TargetAction.Countered = true;
                }
                else if (counterForm.CheckBaseDC != 0)
                {
                    var checkDC = counterForm.CheckBaseDC + slotLevel;

                    rulesetCharacter
                        .EnumerateFeaturesToBrowse<FeatureDefinitionMagicAffinity>(rulesetCharacter.FeaturesToBrowse);

                    foreach (var featureDefinition in rulesetCharacter.FeaturesToBrowse)
                    {
                        var definitionMagicAffinity = (FeatureDefinitionMagicAffinity)featureDefinition;

                        if (definitionMagicAffinity.CounterspellAffinity == AdvantageType.None)
                        {
                            continue;
                        }

                        var advTrend = definitionMagicAffinity.CounterspellAffinity == AdvantageType.Advantage
                            ? 1
                            : -1;

                        actionModifier.AbilityCheckAdvantageTrends.Add(new TrendInfo(
                            advTrend, FeatureSourceType.CharacterFeature, definitionMagicAffinity.Name, null));
                    }

                    if (counteredSpell.CounterAffinity != AdvantageType.None)
                    {
                        var advTrend = counteredSpell.CounterAffinity == AdvantageType.Advantage
                            ? 1
                            : -1;

                        actionModifier.AbilityCheckAdvantageTrends
                            .Add(new TrendInfo(advTrend,
                                FeatureSourceType.CharacterFeature,
                                counteredSpell.CounterAffinityOrigin, null));
                    }

                    var abilityScoreName = AttributeDefinitions.Charisma;

                    foreach (var spellRepertoire in rulesetCharacter.SpellRepertoires
                                 .Where(repertoire =>
                                     repertoire.SpellCastingFeature.SpellCastingOrigin
                                         is FeatureDefinitionCastSpell.CastingOrigin.Class
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

                    var abilityCheckRoll = actingCharacter.RollAbilityCheck(
                        abilityScoreName,
                        proficiencyName,
                        checkDC,
                        AdvantageType.None,
                        actionModifier,
                        false,
                        0,
                        out var outcome,
                        out var successDelta,
                        true);

                    var abilityCheckData = new AbilityCheckData
                    {
                        AbilityCheckRoll = abilityCheckRoll,
                        AbilityCheckRollOutcome = outcome,
                        AbilityCheckSuccessDelta = successDelta,
                        AbilityCheckActionModifier = actionModifier,
                        Action = actionUsePower
                    };

                    yield return TryAlterOutcomeAttributeCheck
                        .HandleITryAlterOutcomeAttributeCheck(actingCharacter, abilityCheckData);

                    actionUsePower.AbilityCheckRoll = abilityCheckData.AbilityCheckRoll;
                    actionUsePower.AbilityCheckRollOutcome = abilityCheckData.AbilityCheckRollOutcome;
                    actionUsePower.AbilityCheckSuccessDelta = abilityCheckData.AbilityCheckSuccessDelta;

                    if (counterAction.AbilityCheckRollOutcome == RollOutcome.Success)
                    {
                        actionUsePower.ActionParams.TargetAction.Countered = true;
                    }
                }

                if (!actionParams.TargetAction.Countered ||
                    rulesetCharacter.SpellCounter == null)
                {
                    continue;
                }

                var unknown = string.IsNullOrEmpty(counteredSpell.IdentifiedBy);

                rulesetCharacter.SpellCounter(
                    rulesetCharacter,
                    actionUsePower.ActionParams.TargetAction.ActingCharacter.RulesetCharacter,
                    counteredSpellDefinition,
                    actionUsePower.ActionParams.TargetAction.Countered,
                    unknown);
            }
        }
    }
}
