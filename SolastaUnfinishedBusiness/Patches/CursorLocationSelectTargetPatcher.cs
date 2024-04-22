using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Spells;
using UnityEngine;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Subclasses.SorcerousFieldManipulator;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CursorLocationSelectTargetPatcher
{
    [HarmonyPatch(typeof(CursorLocationSelectTarget), nameof(CursorLocationSelectTarget.IsFilteringValid))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsFilteringValid_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            CursorLocationSelectTarget __instance,
            GameLocationCharacter target,
            ref bool __result)
        {
            var definition = __instance.ActionParams.activeEffect.SourceDefinition;
            var actingCharacter = __instance.actionParams.actingCharacter;

            // required for familiar attack
            actingCharacter.UsedSpecialFeatures.Remove("FamiliarAttack");

            //PATCH: supports `UseOfficialLightingObscurementAndVisionRules`
            if (__result &&
                definition is IMagicEffect magicEffect &&
                !LightingAndObscurementContext.IsMagicEffectValidIfHeavilyObscuredOrInNaturalDarkness(
                    actingCharacter, magicEffect, target))
            {
                __instance.actionModifier.FailureFlags.Add("Failure/&FailureFlagNoPerceptionOfTargetDescription");
                __result = false;

                return;
            }

            //PATCH: supports `IFilterTargetingCharacter`
            foreach (var filterTargetingMagicEffect in
                     definition.GetAllSubFeaturesOfType<IFilterTargetingCharacter>())
            {
                __result = filterTargetingMagicEffect.IsValid(__instance, target);

                if (__result)
                {
                    return;
                }
            }

            //PATCH: supports Find Familiar specific case for any caster as spell can be granted to other classes
            if (Gui.Battle != null &&
                actingCharacter.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                __instance.actionParams.RulesetEffect is RulesetEffectSpell rulesetEffectSpell &&
                rulesetEffectSpell.EffectDescription.RangeType is RangeType.Touch or RangeType.MeleeHit)
            {
                var familiar = Gui.Battle.AllContenders
                    .FirstOrDefault(x =>
                        x.RulesetCharacter is RulesetCharacterMonster rulesetCharacterMonster &&
                        rulesetCharacterMonster.MonsterDefinition.Name == SpellBuilders.OwlFamiliar &&
                        rulesetCharacterMonster.AllConditions.Exists(y =>
                            y.ConditionDefinition == ConditionDefinitions.ConditionConjuredCreature &&
                            y.SourceGuid == actingCharacter.Guid));

                var canAttack = familiar != null && familiar.IsWithinRange(target, 1);

                if (canAttack)
                {
                    var effectDescription = new EffectDescription();

                    effectDescription.Copy(__instance.effectDescription);
                    effectDescription.rangeParameter = 24;

                    __instance.effectDescription = effectDescription;
                    actingCharacter.UsedSpecialFeatures.Add("FamiliarAttack", 0);
                }
                else
                {
                    __instance.effectDescription = __instance.ActionParams.RulesetEffect.EffectDescription;
                }
            }

            //PATCH: support for target spell filtering based on custom spell filters
            // used for melee cantrips to limit targets to weapon attack range
            if (!__result)
            {
                return;
            }

            __result = IsFilteringValidMeleeCantrip(__instance, target);
        }

        private static bool IsFilteringValidMeleeCantrip(
            CursorLocationSelectTarget __instance,
            GameLocationCharacter target)
        {
            var actionParams = __instance.actionParams;
            var canBeUsedToAttack = actionParams?.RulesetEffect
                ?.SourceDefinition.GetFirstSubFeatureOfType<IAttackAfterMagicEffect>()?.CanBeUsedToAttack;

            if (canBeUsedToAttack == null || canBeUsedToAttack(__instance, actionParams.actingCharacter, target,
                    out var failure))
            {
                return true;
            }

            __instance.actionModifier.FailureFlags.Add(failure);

            return false;
        }
    }

    [HarmonyPatch(typeof(CursorLocationSelectTarget), nameof(CursorLocationSelectTarget.Activate))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Activate_Patch
    {
        [UsedImplicitly]
        public static void Prefix(params object[] parameters)
        {
            //PATCH: allows Sorcerous Field Manipulator displacement to select any character
            if (parameters.Length <= 0 ||
                parameters[0] is not CharacterActionParams { RulesetEffect: RulesetEffectPower rulesetEffectPower })
            {
                return;
            }

            if (rulesetEffectPower.PowerDefinition == PowerSorcerousFieldManipulatorDisplacement)
            {
                // allows any target to be selected as well as automatically presents a better UI description
                rulesetEffectPower.EffectDescription.inviteOptionalAlly = false;
            }
        }
    }

    [HarmonyPatch(typeof(CursorLocationSelectTarget), nameof(CursorLocationSelectTarget.Deactivate))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Deactivate_Patch
    {
        [UsedImplicitly]
        public static void Prefix(CursorLocationSelectTarget __instance)
        {
            //PATCH: allows Sorcerous Field Manipulator displacement to select any character
            if (__instance.actionParams is not { RulesetEffect: RulesetEffectPower rulesetEffectPower })
            {
                return;
            }

            if (rulesetEffectPower.PowerDefinition == PowerSorcerousFieldManipulatorDisplacement)
            {
                // brings back power effect to it's original definition
                rulesetEffectPower.EffectDescription.inviteOptionalAlly = true;
            }
        }
    }

    //PATCH: support EnforceFullSelection on IFilterTargetingCharacter (vanilla code except for patch block)
    [HarmonyPatch(typeof(CursorLocationSelectTarget), nameof(CursorLocationSelectTarget.RefreshCaption))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshCaption_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(CursorLocationSelectTarget __instance)
        {
            if (CursorLocation.CaptionLineChanged == null)
            {
                return false;
            }

            var captionCounter = string.Empty;
            var str = string.Empty;

            if (__instance.effectDescription != null)
            {
                if (__instance.effectDescription.TargetType == TargetType.Position &&
                    __instance.effectDescription.InviteOptionalAlly)
                {
                    CursorLocation.CaptionLineChanged(__instance.captionTitle,
                        Gui.Localize("Caption/&InviteOptionalAllyCaption"), captionCounter, string.Empty, string.Empty,
                        string.Empty, true, true);

                    return false;
                }

                if (__instance.effectDescription.TargetFilteringMethod is TargetFilteringMethod.AllCharacterAndGadgets
                    or TargetFilteringMethod.CharacterOnly or TargetFilteringMethod.CharacterGadgetEffectProxy)
                {
                    // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                    switch (__instance.TargetSide)
                    {
                        case Side.All:
                            str += Gui.Localize("Caption/&TargetFilteringCreature");
                            break;
                        case Side.Ally:
                            str += Gui.Localize("Caption/&TargetFilteringAllyCreature");
                            break;
                        case Side.Enemy:
                            str += Gui.Localize("Caption/&TargetFilteringEnemyCreature");
                            break;
                    }
                }

                if (__instance.effectDescription.TargetFilteringMethod is TargetFilteringMethod.AllCharacterAndGadgets
                    or TargetFilteringMethod.GadgetOnly or TargetFilteringMethod.CharacterGadgetEffectProxy)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        str += Gui.ListSeparator();
                    }

                    str += Gui.Localize("Caption/&TargetFilteringGadget");
                }

                if (__instance.effectDescription.TargetFilteringMethod ==
                    TargetFilteringMethod.CharacterGadgetEffectProxy)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        str += Gui.ListSeparator();
                    }

                    str += Gui.Localize("Caption/&TargetFilteringEffectProxy");
                }
            }
            else
            {
                str = __instance.TargetSide != Side.Ally
                    ? str + Gui.Localize("Caption/&TargetFilteringCreature")
                    : str + Gui.Localize("Caption/&TargetFilteringAllyCreature");
            }

            string captionContent;
            if (__instance.effectDescription is { TargetType: TargetType.SharedAmongIndividuals })
            {
                captionContent = Gui.Format("Caption/&TargetShareUniqueCaption", str,
                    __instance.GameLocationSelectionService.SelectedTargets.Count.ToString());
            }
            else
            {
                string baseText;

                if (__instance.maxTargets == 1)
                {
                    baseText = "Caption/&TargetSingleCaption";
                }
                else
                {
                    baseText = !__instance.uniqueTargets
                        ? "Caption/&TargetMultipleCaption"
                        : "Caption/&TargetMultipleUniqueCaption";
                    captionCounter = Gui.FormatCurrentOverMax(__instance.maxTargets - __instance.remainingTargets,
                        __instance.maxTargets);
                }

                captionContent = Gui.Format(baseText, str, __instance.remainingTargets.ToString());
            }

            var captionProximity = string.Empty;

            if (__instance.effectDescription is { RequiresTargetProximity: true, TargetProximityDistance: > 0 })
            {
                captionProximity = Gui.Format(
                    __instance.effectDescription.TargetProximityDistance == 1
                        ? "Caption/&TargetProximitySingleCaption"
                        : "Caption/&TargetProximityMultipleCaption",
                    __instance.effectDescription.TargetProximityDistance.ToString());
            }

            if (__instance.effectDescription is { TargetType: TargetType.ArcFromIndividual })
            {
                var targetParameter = __instance.ActionParams.RulesetEffect.ComputeTargetParameter();
                var targetParameter2 = __instance.ActionParams.RulesetEffect.ComputeTargetParameter2();

                captionProximity +=
                    Gui.Format(
                        targetParameter <= 1
                            ? "Caption/&TargetArcSubtargetSingleCaption"
                            : "Caption/&TargetArcSubtargetMultipleCaption", targetParameter.ToString(),
                        targetParameter2.ToString());
            }

            var captionRequiredCondition = string.Empty;

            if (__instance.effectDescription != null && __instance.effectDescription.TargetCondition != null)
            {
                captionRequiredCondition = Gui.Format("Caption/&TargetRequiredConditionCaption",
                    __instance.effectDescription.TargetCondition.GuiPresentation.Title);
            }

            if (__instance.effectDescription is { RestrictedCreatureFamilies.Count: > 0 })
            {
                if (!string.IsNullOrEmpty(captionRequiredCondition))
                {
                    captionRequiredCondition += " ";
                }

                captionRequiredCondition += Gui.Format("Caption/&TargetRequiredCreatureTypeCaption",
                    __instance.effectDescription.EnumerateRestrictedCreatureFamilies());
            }

            if (__instance.effectDescription != null &&
                (__instance.effectDescription.TargetFilteringTag & TargetFilteringTag.Unarmored) !=
                TargetFilteringTag.No)
            {
                if (!string.IsNullOrEmpty(captionRequiredCondition))
                {
                    captionRequiredCondition += " ";
                }

                captionRequiredCondition += Gui.Localize("Caption/&TargetRequiredUnarmoredCaption");
            }

            if (__instance.effectDescription != null &&
                (__instance.effectDescription.TargetFilteringTag & TargetFilteringTag.MetalArmor) !=
                TargetFilteringTag.No)
            {
                if (!string.IsNullOrEmpty(captionRequiredCondition))
                {
                    captionRequiredCondition += " ";
                }

                captionRequiredCondition += Gui.Localize("Caption/&TargetRequiredMetalArmorCaption");
            }

            var creatureSizeCaption =
                CursorLocationSelectTarget.GenerateCreatureSizeCaption(__instance.effectDescription);

            // BEGIN PATCH
            // var canProceed = __instance.maxTargets < 0 || (__instance.maxTargets > 1 && __instance.remainingTargets < __instance.maxTargets);

            bool canProceed;
            var enforceFullSelection = false;
            if (__instance.actionParams is { RulesetEffect: RulesetEffectPower rulesetEffectPower })
            {
                var filterTargetingCharacter =
                    rulesetEffectPower.PowerDefinition.GetFirstSubFeatureOfType<IFilterTargetingCharacter>();

                if (filterTargetingCharacter != null)
                {
                    enforceFullSelection = filterTargetingCharacter.EnforceFullSelection;
                }
            }

            if (enforceFullSelection)
            {
                canProceed = __instance.maxTargets < 0 ||
                             (__instance.maxTargets > 1 && __instance.remainingTargets == 0);
            }
            else
            {
                canProceed = __instance.maxTargets < 0 ||
                             (__instance.maxTargets > 1 && __instance.remainingTargets < __instance.maxTargets);
            }
            // END PATCH

            CursorLocation.CaptionLineChanged(__instance.captionTitle, captionContent, captionCounter, captionProximity,
                captionRequiredCondition, creatureSizeCaption, canProceed, true);

            return false;
        }
    }

    [HarmonyPatch(typeof(CursorLocationSelectTarget), nameof(CursorLocationSelectTarget.OnClickMainPointer))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnClickMainPointer_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            CursorLocationSelectTarget __instance,
            out CursorDefinitions.CursorActionResult actionResult)
        {
            actionResult = CursorDefinitions.CursorActionResult.None;
            if (__instance.RefreshTargetedCharacter())
            {
                __instance.actionModifier.Reset();

                var isValid = false;
                var isMagic = false;

                if (ActionDefinitions.IsAttackAction(__instance.ActionParams.ActionDefinition.Id))
                {
                    if (__instance.IsValidAttack(__instance.ActionParams.AttackMode, out _))
                    {
                        isValid = true;
                    }
                }
                else if (__instance.effectDescription != null)
                {
                    var rangeType = __instance.effectDescription.RangeType;
                    var targetType = __instance.effectDescription.TargetType;

                    // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                    switch (rangeType)
                    {
                        case RangeType.Touch:
                        case RangeType.Distance:
                            if (__instance.IsValidMagicTarget(out _))
                            {
                                isMagic = true;
                                isValid = true;
                            }

                            break;
                        case RangeType.MeleeHit:
                        case RangeType.RangeHit:
                            if (__instance.IsValidMagicAttack(out _))
                            {
                                isMagic = true;
                                isValid = true;
                            }

                            break;
                        default:
                            if (targetType == TargetType.Position && __instance.effectDescription.InviteOptionalAlly &&
                                __instance.IsValidMagicTarget(out _))
                            {
                                isMagic = true;
                                isValid = true;
                            }

                            break;
                    }
                }

                if (!isValid)
                {
                    return false;
                }

                __instance.GameLocationSelectionService.SelectTarget(__instance.targetedCharacter);
                __instance.actionModifiersList.Add(__instance.actionModifier.Clone());

                if (__instance.maxTargets > 0)
                {
                    --__instance.remainingTargets;

                    if (__instance.remainingTargets > 0)
                    {
                        actionResult = CursorDefinitions.CursorActionResult.SelectTarget;
                        __instance.RefreshCaption();
                    }
                    else
                    {
                        // BEGIN PATCH
                        var modifier = __instance.ActionParams.RulesetEffect switch
                        {
                            RulesetEffectPower rulesetEffectPower => rulesetEffectPower.PowerDefinition
                                .GetFirstSubFeatureOfType<ISelectPositionAfterCharacter>(),
                            RulesetEffectSpell rulesetEffectSpell => rulesetEffectSpell.SpellDefinition
                                .GetFirstSubFeatureOfType<ISelectPositionAfterCharacter>(),
                            _ => null
                        };

                        // enable select position if any modifier found
                        if (modifier != null)
                        {
                            var actionParams = __instance.ActionParams;

                            actionParams.ActionModifiers.SetRange(__instance.ActionModifiersList);
                            actionParams.TargetCharacters.SetRange(__instance.SelectionService.SelectedTargets);
                            actionParams.RulesetEffect.EffectDescription.rangeParameter = modifier.PositionRange;

                            __instance.CursorService
                                .ActivateCursor<CursorLocationSelectPosition>(__instance.ActionParams);

                            return false;
                        }
                        // END PATCH

                        actionResult = isMagic
                            ? CursorDefinitions.CursorActionResult.CastSpell
                            : CursorDefinitions.CursorActionResult.Attack;

                        if (isMagic)
                        {
                            var flag3 = true;

                            if (__instance.ActionParams.RulesetEffect is RulesetEffectPower rulesetEffect)
                            {
                                if (rulesetEffect.PowerDefinition.RechargeRate == RechargeRate.HealingPool &&
                                    rulesetEffect.PowerDefinition.CostPerUse <= 0)
                                {
                                    if (__instance.effectDescription.EffectForms.Any(effectForm =>
                                            effectForm.FormType == EffectForm.EffectFormType.Healing &&
                                            effectForm.HealingForm.HealingComputation == HealingComputation.Pool &&
                                            effectForm.HealingForm.VariablePool))
                                    {
                                        flag3 = false;

                                        var num = Mathf.Min(rulesetEffect.UsablePower.SpentPoints,
                                            __instance.targetedCharacter.RulesetCharacter.MissingHitPoints);

                                        Gui.GuiService.GetScreen<NumberSelectionModal>()
                                            .ShowPower(rulesetEffect.PowerDefinition, 1, num, num,
                                                rulesetEffect.UsablePower);
                                    }
                                }
                            }

                            if (!flag3)
                            {
                                return false;
                            }

                            Gui.GuiService.GetScreen<CursorCaptionScreen>().ProceedCb();
                        }
                        else
                        {
                            __instance.ProcessAction();
                        }
                    }
                }
                else
                {
                    if (__instance.maxTargets >= 0)
                    {
                        return false;
                    }

                    actionResult = CursorDefinitions.CursorActionResult.SelectTarget;
                    __instance.RefreshCaption();
                }
            }
            else
            {
                if (__instance.SelectionService.HoveredCharacters.Count != 1)
                {
                    return false;
                }

                actionResult = CursorDefinitions.CursorActionResult.Invalid;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(CursorLocationSelectTarget), nameof(CursorLocationSelectTarget.RefreshHover))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshHover_Patch
    {
        [UsedImplicitly]
        public static void Postfix(CursorLocationSelectTarget __instance)
        {
            __instance.affectedCharacterColor =
                GameUiContext.HighContrastColors[Main.Settings.HighContrastTargetingSingleSelectedColor];
        }
    }
}
