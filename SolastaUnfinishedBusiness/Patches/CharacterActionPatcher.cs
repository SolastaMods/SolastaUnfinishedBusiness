using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalActions;
using static SolastaUnfinishedBusiness.Subclasses.MartialRoyalKnight;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionPatcher
{
    [HarmonyPatch(typeof(CharacterAction), nameof(CharacterAction.InstantiateAction))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class InstantiateAction_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(CharacterActionParams actionParams, ref CharacterAction __result)
        {
            //PATCH: creates action objects for actions defined in mod

            // required when interacting with some game inanimate objects (like minor gates)
            if (actionParams == null)
            {
                return true;
            }

            var name = CharacterAction.GetTypeName(actionParams);

            //Actions defined in mod will be non-null, actions from base game will be null
            var type = Type.GetType(name);

            if (type == null)
            {
                return true;
            }

            __result = Activator.CreateInstance(type, actionParams) as CharacterAction;

            return false;
        }
    }

    [HarmonyPatch(typeof(CharacterAction), nameof(CharacterAction.Execute))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Execute_Patch
    {
        [UsedImplicitly]
        public static void Prefix(CharacterAction __instance)
        {
            //PATCH: support for character action tracking
            Global.CurrentAction = __instance;

            switch (__instance)
            {
                case CharacterActionCastSpell or CharacterActionSpendSpellSlot:
                    //PATCH: Hold the state of the SHIFT key on bool 5 to determine which slot to use on MC Warlock
                    var isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                    __instance.actionParams.BoolParameter5 = isShiftPressed;
                    break;

                case CharacterActionReady:
                    CustomReactionsContext.ReadReadyActionPreferredCantrip(__instance.actionParams);
                    break;

                case CharacterActionSpendPower spendPower:
                    PowerBundle.SpendBundledPowerIfNeeded(spendPower);
                    break;
            }
        }

        [UsedImplicitly]
        public static IEnumerator Postfix(IEnumerator values, CharacterAction __instance)
        {
            var rulesetCharacter = __instance.ActingCharacter.RulesetCharacter;

            //PATCH: IActionInitiated
            if (rulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
            {
                var iActionsInitiated = rulesetCharacter.GetSubFeaturesByType<IActionInitiated>();

                foreach (var iActionInitiated in iActionsInitiated)
                {
                    yield return iActionInitiated.OnActionInitiated(__instance);
                }
            }

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            if (rulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
            {
                //BUGFIX: fix Haste spell not allowing the sequence attack / cast spell
                if (__instance.ActionType == ActionDefinitions.ActionType.Main &&
                    rulesetCharacter.HasAnyConditionOfType(DatabaseHelper.ConditionDefinitions.ConditionHasted.Name))
                {
                    var gameLocationCharacter = __instance.ActingCharacter;

                    if (gameLocationCharacter.UsedMainCantrip || gameLocationCharacter.UsedMainSpell)
                    {
                        AdditionalActionHasted.RestrictedActions.Remove(ActionDefinitions.Id.CastMain);
                    }
                    else
                    {
                        AdditionalActionHasted.RestrictedActions.TryAdd(ActionDefinitions.Id.CastMain);
                    }
                }

                //PATCH: allows characters surged from Royal Knight to be able to cast spell main on each action
                if (__instance.ActionType == ActionDefinitions.ActionType.Main &&
                    rulesetCharacter.HasAnyConditionOfType(ConditionInspiringSurge, ConditionSpiritedSurge))
                {
                    __instance.ActingCharacter.UsedMainSpell = false;
                    __instance.ActingCharacter.UsedMainCantrip = false;
                }

                //PATCH: IActionFinished
                var iActionsFinished = rulesetCharacter.GetSubFeaturesByType<IActionFinished>();

                foreach (var iActionFinished in iActionsFinished)
                {
                    yield return iActionFinished.OnActionFinished(__instance);
                }
            }

            //PATCH: support for character action tracking
            Global.CurrentAction = null;
        }
    }
}
