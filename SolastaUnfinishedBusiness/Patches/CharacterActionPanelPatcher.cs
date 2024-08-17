using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using UnityEngine.UI;
using static RuleDefinitions;
using Object = UnityEngine.Object;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionPanelPatcher
{
    private static bool HasShapeChangeForm(RulesetEffectSpell rulesetEffectSpell)
    {
        return rulesetEffectSpell.SpellDefinition.EffectDescription.EffectForms.Any(
            x => x.FormType == EffectForm.EffectFormType.ShapeChange);
    }

    [HarmonyPatch(typeof(CharacterActionPanel), nameof(CharacterActionPanel.ReadyActionEngaged))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ReadyActionEngaged_Patch
    {
        [UsedImplicitly]
        public static void Prefix(CharacterActionPanel __instance, ActionDefinitions.ReadyActionType readyActionType)
        {
            //PATCH: used for `force preferred cantrip` option
            CustomReactionsContext.SaveReadyActionPreferredCantrip(__instance.actionParams, readyActionType);
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), nameof(CharacterActionPanel.BindCharacterActionItem))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class BindCharacterActionItem_Patch
    {
        [UsedImplicitly]
        public static void Prefix(CharacterActionPanel __instance, GuiCharacterAction guiCharacterAction)
        {
            //PATCH: allow cast Quickened and Bonus spell to be small if both present
            CustomActionIdContext.UpdateCastActionForm(guiCharacterAction, __instance.filteredActions);
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), nameof(CharacterActionPanel.ComputeMultipleGuiCharacterActions))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeMultipleGuiCharacterActions_Patch
    {
        [UsedImplicitly]
        public static void Postfix(CharacterActionPanel __instance, ref int __result, ActionDefinitions.Id actionId)
        {
            //PATCH: Support for ExtraAttacksOnActionPanel
            //Allows multiple actions on panel for off-hand attacks and main attacks for non-guests
            __result = ExtraAttacksOnActionPanel.ComputeMultipleGuiCharacterActions(__instance, actionId, __result);
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), nameof(CharacterActionPanel.RefreshActions))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshActions_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: hide power button on action panel if no valid powers to use or see
            var method = new Func<
                List<ActionDefinitions.Id>,
                CharacterActionPanel,
                int
            >(FilterActions).Method;

            //TODO: it replaces first call to get_Count, find a better way to find proper place
            return instructions.ReplaceCode(
                code => code.opcode == OpCodes.Callvirt && $"{code.operand}".Contains("get_Count"), 1,
                "CharacterActionPanel.RefreshActions",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, method));
        }

        private static int FilterActions(List<ActionDefinitions.Id> actions, CharacterActionPanel panel)
        {
            var character = panel.GuiCharacter.RulesetCharacter;
            var inBattle = Gui.Battle != null;

            //PATCH: reorder the actions panel in case we have custom toggles
            CustomActionIdContext.ReorderToggles(actions);

            //PATCH: hide power button on action panel if no valid powers to use or see
            actions.RemoveAll(id => ActionIsInvalid(id, character, inBattle));

            return actions.Count;
        }

        private static bool ActionIsInvalid(ActionDefinitions.Id id, RulesetCharacter character, bool battle)
        {
            return id switch
            {
                ActionDefinitions.Id.PowerMain => !character.CanSeeAndUseAtLeastOnePower(
                    ActionDefinitions.ActionType.Main, battle),
                ActionDefinitions.Id.PowerBonus => !character.CanSeeAndUseAtLeastOnePower(
                    ActionDefinitions.ActionType.Bonus, battle),
                ActionDefinitions.Id.PowerNoCost => !character.CanSeeAndUseAtLeastOnePower(
                    ActionDefinitions.ActionType.NoCost, battle),
                _ => false
            };
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), nameof(CharacterActionPanel.OnActivateAction))]
    [HarmonyPatch([typeof(ActionDefinitions.Id), typeof(GuiCharacterAction)])]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnActivateAction_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: Support for ExtraAttacksOnActionPanel
            //replaces calls to FindExtraActionAttackModes to custom method which supports forced attack modes for offhand attacks
            var findAttacks = typeof(GameLocationCharacter).GetMethod("FindActionAttackMode");
            var method = new Func<
                GameLocationCharacter,
                ActionDefinitions.Id,
                bool,
                bool,
                bool,
                ActionDefinitions.ReadyActionType,
                GuiCharacterAction,
                RulesetAttackMode
            >(ExtraAttacksOnActionPanel.FindExtraActionAttackModesFromGuiAction).Method;

            return instructions.ReplaceCalls(findAttacks, "CharacterActionPanel.OnActivateAction",
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Call, method));
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), nameof(CharacterActionPanel.SelectSpell))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SelectSpell_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: Support for Quickened Spell action
            //replaces calls to ActionType to custom method which returns Main for Quickened action
            var getActionType = typeof(CharacterActionPanel).GetProperty(nameof(CharacterActionPanel.ActionType))!
                .GetGetMethod();
            var method = new Func<
                CharacterActionPanel,
                ActionDefinitions.ActionType
            >(GetActionType).Method;

            return instructions.ReplaceCalls(getActionType, "CharacterActionPanel.SelectSpell",
                new CodeInstruction(OpCodes.Call, method));
        }

        private static ActionDefinitions.ActionType GetActionType(CharacterActionPanel panel)
        {
            return panel.actionId == (ActionDefinitions.Id)ExtraActionId.CastQuickened
                ? ActionDefinitions.ActionType.Main
                : panel.ActionType;
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), nameof(CharacterActionPanel.SpellCastConfirmed))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SpellCastConfirmed_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(CharacterActionPanel __instance)
        {
            //PATCH: Support for Quickened Spell action
            if (__instance.actionId != (ActionDefinitions.Id)ExtraActionId.CastQuickened)
            {
                return true;
            }

            __instance.actionId = ActionDefinitions.Id.CastBonus;
            __instance.MetamagicSelected(
                __instance.GuiCharacter.GameLocationCharacter,
                (RulesetEffectSpell)__instance.actionParams.activeEffect,
                DatabaseHelper.MetamagicOptionDefinitions.MetamagicQuickenedSpell
            );
            return false;
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), nameof(CharacterActionPanel.InvocationCastEngaged))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class InvocationCastEngaged_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(CharacterActionPanel __instance, RulesetInvocation invocation, int subspellIndex)
        {
            var definition = invocation.InvocationDefinition;
            var power = definition.GetPower();
            var actionDefinitions =
                ServiceRepository.GetService<IGameLocationActionService>().AllActionDefinitions;

            if (power)
            {
                var bundle = power.GetBundle();

                if (bundle != null)
                {
                    if (subspellIndex >= 0 && bundle.SubPowers.Count > subspellIndex)
                    {
                        power = bundle.SubPowers[subspellIndex];
                    }
                    else
                    {
                        Main.Error($"Wrong index for power bundle in '{definition.Name}' invocation: {subspellIndex}");
                    }
                }

                __instance.actionId = power.BattleActionId;
                __instance.actionParams.actionDefinition = actionDefinitions[__instance.actionId];
                __instance.PowerEngaged(PowerProvider.Get(power, __instance.GuiCharacter.RulesetCharacter));

                return false;
            }

            if (definition.GrantedSpell)
            {
                if (__instance.actionId == ActionDefinitions.Id.CastInvocation)
                {
                    return true;
                }

                __instance.actionId = definition.GrantedSpell.BattleActionId;
                __instance.actionParams.actionDefinition = actionDefinitions[__instance.actionId];

                return true;
            }

            //Shouldn't happen - it should return from earlier, but just in case, to prevent crash
            Main.Error("InvocationCastEngaged with null spell and no power feature");
            return false;
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), nameof(CharacterActionPanel.SelectInvocation))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SelectInvocation_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: Support for bonus action invocations
            //replaces calls to InvocationSelectionPanel.Bind to custom method which supports filtering bonus and main action invocations
            var bindInvocationSelectionPanel = typeof(InvocationSelectionPanel).GetMethod("Bind");
            var method = new Action<
                InvocationSelectionPanel,
                GameLocationCharacter, // caster,
                InvocationSelectionPanel.InvocationSelectedHandler, // selected,
                InvocationSelectionPanel.InvocationCancelledHandler, // canceled,
                CharacterActionPanel // panel
            >(InvocationSelectionPanelExtensions.CustomBind).Method;

            return instructions.ReplaceCalls(bindInvocationSelectionPanel, "CharacterActionPanel.SelectInvocation",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, method));
        }
    }

    //PATCH: don't display the break free selection panel if restrained by web or or ice bound
    [HarmonyPatch(typeof(CharacterActionPanel), nameof(CharacterActionPanel.SelectBreakFreeMode))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SelectBreakFreeMode_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(CharacterActionPanel __instance)
        {
            var rulesetCharacter = __instance.GuiCharacter.RulesetCharacter;

            RulesetCondition restrainingCondition = null;

            rulesetCharacter.EnumerateFeaturesToBrowse<FeatureDefinitionActionAffinity>(
                rulesetCharacter.FeaturesToBrowse);

            foreach (var definitionActionAffinity in rulesetCharacter.FeaturesToBrowse
                         .Cast<FeatureDefinitionActionAffinity>()
                         .Where(definitionActionAffinity => definitionActionAffinity.AuthorizedActions
                             .Contains(ActionDefinitions.Id.BreakFree)))
            {
                restrainingCondition = rulesetCharacter.FindFirstConditionHoldingFeature(definitionActionAffinity);
            }

            if (restrainingCondition?.ConditionDefinition.Name is not
                ("ConditionVileBrew" or
                "ConditionGrappledRestrainedIceBound" or
                "ConditionGrappledRestrainedSpellWeb" or
                "ConditionRestrainedByEntangle"))
            {
                return true;
            }

            __instance.actionParams.BreakFreeMode = ActionDefinitions.BreakFreeMode.Athletics;

            ServiceRepository.GetService<ICommandService>()
                .ExecuteAction(__instance.actionParams.Clone(), __instance.ActionExecuted, false);

            return false;
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), nameof(CharacterActionPanel.RefreshActionPerformances))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshActionPerformances_Patch
    {
        [UsedImplicitly]
        public static void Postfix(CharacterActionPanel __instance)
        {
            if (!Main.Settings.EnableActionSwitching)
            {
                return;
            }

            var table = __instance.actionPerformanceTable;

            if (!table)
            {
                return;
            }

            if (!table.gameObject.activeSelf)
            {
                return;
            }

            var filters = __instance.GuiCharacter.GameLocationCharacter.ActionPerformancesByType[__instance.ActionType];

            if (table.gameObject.TryGetComponent<HorizontalLayoutGroup>(out var group))
            {
                Object.DestroyImmediate(group);
            }

            if (!table.gameObject.TryGetComponent<GridLayoutGroup>(out var grid))
            {
                grid = table.gameObject.AddComponent<GridLayoutGroup>();
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                grid.childAlignment = TextAnchor.MiddleCenter;
                grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
                grid.startAxis = GridLayoutGroup.Axis.Horizontal;
                grid.cellSize = new Vector2(32, 10);
                grid.spacing = new Vector2(3, 5);
            }

            if (grid)
            {
                var width = (int)__instance.RectTransform.rect.width;
                var constraint = width / 35;

                if (constraint > filters.Count)
                {
                    constraint = filters.Count;
                }

                grid.constraintCount = constraint;
            }

            var activeCount = 0;

            for (var i = 0; i < table.childCount; i++)
            {
                var child = table.GetChild(i);

                if (!child.gameObject.activeSelf)
                {
                    continue;
                }

                var item = child.GetComponent<ActionTypePerformanceItem>();

                if (!item)
                {
                    continue;
                }

                activeCount++;

                var k = child.GetSiblingIndex();
                var f = k >= 0 && k < filters.Count
                    ? PerformanceFilterExtraData.GetData(filters[k])
                    : null;

                var featureName = f?.FormatTitle();

                if (!string.IsNullOrEmpty(featureName))
                {
                    item.Tooltip.Content += $"\n{featureName}";
                }


                var btn = item.GetComponent<Button>();

                if (btn)
                {
                    continue;
                }

                btn = item.gameObject.AddComponent<Button>();
                btn.enabled = true;
                btn.interactable = true;
                btn.onClick.AddListener(() =>
                {
                    if (!Main.Settings.EnableActionSwitching)
                    {
                        return;
                    }

                    var panel = item.GetComponentInParent<CharacterActionPanel>();

                    if (item.availableSymbol.IsActive())
                    {
                        ActionSwitching.PrioritizeAction(
                            panel.GuiCharacter.GameLocationCharacter, panel.ActionType,
                            item.transform.GetSiblingIndex());
                    }
                });
            }

            var rank = __instance.GuiCharacter.GameLocationCharacter.CurrentActionRankByType[__instance.ActionType];

            if (activeCount - rank >= 2) //at least 2 non-spent actions
            {
                ServiceRepository.GetService<IGuiService>()
                    .ShowTutorial(ActionSwitching.Tutorial);
            }
        }
    }

    //BUGFIX: allows shape change spells to correctly interact with metamagic
    //it displays a shape prompt and avoid the delegate to call ExecuteEffectOfAction
    [HarmonyPatch(typeof(CharacterActionPanel), nameof(CharacterActionPanel.MetamagicIgnored))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class MetamagicIgnored_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(CharacterActionPanel __instance)
        {
            var hasShapeChangeForm = HasShapeChangeForm(__instance.actionParams.activeEffect as RulesetEffectSpell);

            if (!hasShapeChangeForm)
            {
                return true;
            }

            // SelectShape will call ExecuteEffectOfAction
            __instance.SelectShape(__instance.actionParams.RulesetEffect);

            return false;
        }
    }

    //BUGFIX: allows shape change spells to correctly interact with metamagic
    //it displays a shape prompt and avoid the delegate to call ExecuteEffectOfAction
    [HarmonyPatch(typeof(CharacterActionPanel), nameof(CharacterActionPanel.MetamagicSelected))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class MetamagicSelected_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            CharacterActionPanel __instance,
            RulesetEffectSpell spellEffect,
            MetamagicOptionDefinition metamagicOption)
        {
            var hasShapeChangeForm = HasShapeChangeForm(spellEffect);

            if (!hasShapeChangeForm)
            {
                return true;
            }

            // BEGIN VANILLA CODE
            spellEffect.MetamagicOption = metamagicOption;

            if (metamagicOption.Type == MetamagicType.QuickenedSpell)
            {
                __instance.actionParams.ActionDefinition = ServiceRepository.GetService<IGameLocationActionService>()
                    .AllActionDefinitions[ActionDefinitions.Id.CastBonus];
            }
            // END VANILLA CODE

            // SelectShape will call ExecuteEffectOfAction
            __instance.SelectShape(spellEffect);

            return false;
        }
    }
}
