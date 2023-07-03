using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
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

            //PATCH: IActionInitiatedByMe
            if (rulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
            {
                var iActionsInitiated = rulesetCharacter.GetSubFeaturesByType<IActionInitiatedByMe>();

                foreach (var iActionInitiated in iActionsInitiated)
                {
                    yield return iActionInitiated.OnActionInitiatedByMe(__instance);
                }
            }

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            if (rulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
            {
                //PATCH: allows characters surged from Royal Knight to be able to cast spell main on each action
                if (__instance.ActionType == ActionDefinitions.ActionType.Main &&
                    Gui.Battle != null &&
                    rulesetCharacter.HasAnyConditionOfType(ConditionInspiringSurge, ConditionSpiritedSurge))
                {
                    __instance.ActingCharacter.UsedMainSpell = false;
                    __instance.ActingCharacter.UsedMainCantrip = false;
                }

                //PATCH: clear determination cache on every action end
                if (Main.Settings.UseOfficialFlankingRules &&
                    Gui.Battle != null)
                {
                    FlankingAndHigherGroundRules.ClearFlankingDeterminationCache();
                }

                //PATCH: IActionFinishedByMe
                var iActionsFinished = rulesetCharacter.GetSubFeaturesByType<IActionFinishedByMe>();

                foreach (var iActionFinished in iActionsFinished)
                {
                    yield return iActionFinished.OnActionFinishedByMe(__instance);
                }

                //PATCH: support for IActionFinishedByEnemy
                if (Gui.Battle != null)
                {
                    foreach (var target in Gui.Battle.AllContenders
                                 .Where(x => x.Side != __instance.ActingCharacter.Side)
                                 .ToList()) // avoid changing enumerator
                    {
                        var character = target.RulesetCharacter;

                        if (character is not { IsDeadOrDyingOrUnconscious: false })
                        {
                            continue;
                        }

                        var features = character.GetSubFeaturesByType<IActionFinishedByEnemy>()
                            .Where(x => x.ActionDefinition == __instance.ActionDefinition);

                        foreach (var feature in features)
                        {
                            yield return feature.OnActionFinishedByEnemy(target, __instance);
                        }
                    }
                }
            }

            //PATCH: support for character action tracking
            Global.CurrentAction = null;
        }
    }
}
