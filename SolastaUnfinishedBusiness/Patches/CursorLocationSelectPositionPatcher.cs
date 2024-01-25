using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Interfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CursorLocationSelectPositionPatcher
{
    [HarmonyPatch(typeof(CursorLocationSelectPosition), nameof(CursorLocationSelectPosition.Activate))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Activate_Patch
    {
        private static int ComputeAdditionalSummonsBySlotDelta(
            EffectAdvancement __instance,
            int effectAdvancement,
            // ReSharper disable once SuggestBaseTypeForParameter
            CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            var result = __instance.ComputeAdditionalSummonsBySlotDelta(effectAdvancement);

            if (__instance is
                    not
                    {
                        EffectIncrementMethod: EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalSummonsPerIncrement: > 0
                    } ||
                cursorLocationSelectPosition.ActionParams.RulesetEffect is not RulesetEffectSpell rulesetEffectSpell)
            {
                return result;
            }

            var effectLevel = rulesetEffectSpell.EffectLevel;
            var slotLevel = rulesetEffectSpell.SlotLevel;
            var additionalSummons = __instance.ComputeAdditionalSummonsBySlotDelta(effectLevel - slotLevel);

            result += additionalSummons;

            return result;
        }

        //PATCH: supports IFilterTargetingPosition and bugfix on war cast lists not adding additional targets
        [UsedImplicitly]
        public static bool Prefix(CursorLocationSelectPosition __instance, params object[] parameters)
        {
            // Cursor

            __instance.inputService.InputControlSchemeChanged -= __instance.HandleInputControlSchemeChangedForActivate;
            __instance.inputService.InputControlSchemeChanged += __instance.HandleInputControlSchemeChangedForActivate;
            __instance.gameObject.SetActive(true);
            __instance.cursorService.SetCursorTexture(CursorDefinitions.CursorType.Default);

            if (!Gui.GamepadActive || __instance.BoundCommands)
            {
                __instance.RegisterCommands();
                __instance.BoundCommands = true;
            }

            // CursorLocation

            __instance.actionParams = parameters.Length == 0 || parameters[0] is not CharacterActionParams
                ? null
                : parameters[0] as CharacterActionParams;
            __instance.BattleService = ServiceRepository.GetService<IGameLocationBattleService>();
            __instance.Battle = __instance.BattleService.Battle;
            __instance.planarTargettingMode = false;
            __instance.planarTargettingHeight = 0;

            var worldLocationPingService = ServiceRepository.GetService<IWorldLocationPingService>();

            if (worldLocationPingService != null)
            {
                worldLocationPingService.OnContextualPingRequested += __instance.OnContextualPingRequested;
            }

            // CursorSelectPosition

            if (__instance.ActionParams == null)
            {
                Trace.LogError(
                    "Cursor Activate method called with invalid actionParams : " + __instance.GetType().Name);
            }
            else
            {
                __instance.ActionParams.Positions.Clear();
                __instance.captionTitle = string.Empty;
                __instance.selectedPositions.Clear();
                __instance.summonedMonster = null;
                __instance.summonedProxy = null;
                __instance.isTeleportingSpell = false;
                __instance.markedPositions.Clear();
                __instance.effectDescription = __instance.ActionParams.RulesetEffect.EffectDescription;

                foreach (var effectForm in __instance.effectDescription.EffectForms)
                {
                    if (effectForm.FormType == EffectForm.EffectFormType.Summon &&
                        effectForm.SummonForm.SummonType == SummonForm.Type.Creature)
                    {
                        __instance.summonedMonster = DatabaseRepository.GetDatabase<MonsterDefinition>()
                            .GetElement(effectForm.SummonForm.MonsterDefinitionName);
                        break;
                    }

                    if (effectForm.FormType == EffectForm.EffectFormType.Summon &&
                        effectForm.SummonForm.SummonType == SummonForm.Type.EffectProxy)
                    {
                        __instance.summonedProxy = DatabaseRepository.GetDatabase<EffectProxyDefinition>()
                            .GetElement(effectForm.SummonForm.EffectProxyDefinitionName);
                        break;
                    }

                    if (effectForm.FormType == EffectForm.EffectFormType.Motion &&
                        effectForm.MotionForm.Type == MotionForm.MotionType.TeleportToDestination &&
                        __instance.ActionParams?.ActingCharacter?.RulesetCharacter != null)
                    {
                        __instance.isTeleportingSpell = true;
                    }
                }

                __instance.captionTitle = __instance.ActionParams switch
                {
                    { RulesetEffect: RulesetEffectSpell spell } => spell.SpellDefinition.GuiPresentation.Title,
                    { RulesetEffect: RulesetEffectPower power } => power.PowerDefinition.GuiPresentation.Title,
                    _ => __instance.captionTitle
                };

                __instance.maxDistance = __instance.effectDescription.RangeParameter;
                __instance.maxPositions = __instance.effectDescription.TargetParameter;

                if (__instance.ActionParams is { ActingCharacter: not null })
                {
                    __instance.centerPosition = __instance.ActionParams.ActingCharacter.LocationPosition;
                }

                __instance.requiresVisibilityForPosition = __instance.effectDescription.RequiresVisibilityForPosition;
                __instance.inviteOptionalAlly = __instance.effectDescription.InviteOptionalAlly;

                if (__instance.ActionParams is { RulesetEffect: RulesetEffectSpell rulesetEffect } &&
                    ActionDefinitions.IsSpellAction(__instance.ActionParams.ActionDefinition.Id))
                {
                    if (__instance.effectDescription.HasAdditionalSlotAdvancement)
                    {
                        // BEGIN PATCH
                        //BUGFIX: fix vanilla not considering war lists when casting spells with position target
                        __instance.maxPositions +=
                            ComputeAdditionalSummonsBySlotDelta(
                                __instance.effectDescription.EffectAdvancement,
                                rulesetEffect.SlotLevel - rulesetEffect.SpellDefinition.SpellLevel,
                                __instance);
                        // END PATCH
                    }
                    else if (__instance.effectDescription.HasCasterLevelAdvancement && __instance.maxPositions > 0)
                    {
                        __instance.maxPositions +=
                            __instance.effectDescription.EffectAdvancement.ComputeAdditionalSummonsBySlotDelta(
                                rulesetEffect.SpellRepertoire.SpellCastingLevel);
                    }
                }

                __instance.remainingPositions = __instance.maxPositions;
                __instance.RefreshCaption();
                __instance.RefreshSelectedPositions();
                __instance.validPositionsCache.Clear();
                __instance.validCellsComputationCoroutine.Reset();

                // BEGIN PATCH
                //PATCH: supports IFilterTargetingPosition
                var triggerDefaultCompute = true;

                switch (__instance.ActionParams)
                {
                    case { RulesetEffect: RulesetEffectPower rulesetEffectPower }:
                    {
                        var filterTargetingPosition =
                            rulesetEffectPower.PowerDefinition.GetFirstSubFeatureOfType<IFilterTargetingPosition>();

                        if (filterTargetingPosition != null)
                        {
                            triggerDefaultCompute = false;

                            __instance.validCellsComputationCoroutine.Start(
                                filterTargetingPosition.ComputeValidPositions(__instance));
                        }

                        break;
                    }
                    case { RulesetEffect: RulesetEffectSpell rulesetEffectSpell }:
                    {
                        var filterTargetingPosition =
                            rulesetEffectSpell.SpellDefinition.GetFirstSubFeatureOfType<IFilterTargetingPosition>();

                        if (filterTargetingPosition != null)
                        {
                            triggerDefaultCompute = false;

                            __instance.validCellsComputationCoroutine.Start(
                                filterTargetingPosition.ComputeValidPositions(__instance));
                        }

                        break;
                    }
                }

                if (triggerDefaultCompute)
                {
                    if (__instance.effectDescription.HasFormOfType(EffectForm.EffectFormType.Motion) &&
                        __instance.effectDescription.RangeType == RangeType.Distance &&
                        __instance.effectDescription.GetFirstFormOfType(EffectForm.EffectFormType.Motion).MotionForm
                            .Type ==
                        MotionForm.MotionType.TeleportToDestination)
                    {
                        __instance.validCellsComputationCoroutine.Start(__instance.MyComputeValidPositions());
                    }
                }

                // ORIGINAL CODE
                // if (__instance.effectDescription.HasFormOfType(EffectForm.EffectFormType.Motion) &&
                //     __instance.effectDescription.RangeType == RangeType.Distance &&
                //     __instance.effectDescription.GetFirstFormOfType(EffectForm.EffectFormType.Motion).MotionForm
                //         .Type ==
                //     MotionForm.MotionType.TeleportToDestination)
                // {
                //     __instance.validCellsComputationCoroutine.Start(__instance.ComputeValidPositions());
                // }
                // END PATCH

                var feedbackCommandService = ServiceRepository.GetService<IFeedbackCommandService>();

                if (feedbackCommandService == null)
                {
                    return false;
                }

                feedbackCommandService.ChangeAllyTurnContext(
                    __instance.ActionParams.RulesetEffect is RulesetEffectSpell or RulesetEffectPower
                        ? BattleDefinitions.AllyTurnContext.PlaningMagicEffect
                        : BattleDefinitions.AllyTurnContext.ChoosingPosition);
            }

            return false;
        }
    }

    //PATCH: supports `IFilterTargetingPosition`
    [HarmonyPatch(typeof(CursorLocationSelectPosition), nameof(CursorLocationSelectPosition.OnClickMainPointer))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnClickMainPointer_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            CursorLocationSelectPosition __instance,
            out CursorDefinitions.CursorActionResult actionResult)
        {
            actionResult = CursorDefinitions.CursorActionResult.None;

            var actionParams = __instance.ActionParams;
            var effectDescription = actionParams.RulesetEffect.EffectDescription;

            return __instance.validPositionsCache.Contains(__instance.HoveredLocation) ||
                   !effectDescription.EffectForms.Any(x =>
                       x.FormType == EffectForm.EffectFormType.Motion &&
                       x.MotionForm.Type == MotionForm.MotionType.TeleportToDestination);
        }
    }
}
