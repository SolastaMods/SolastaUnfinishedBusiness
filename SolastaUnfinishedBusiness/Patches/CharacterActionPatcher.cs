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
            while (values.MoveNext())
            {
                yield return values.Current;
            }

            var rulesetCharacter = __instance.ActingCharacter.RulesetCharacter;

            if (rulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
            {
                //PATCH: clear determination cache on every action end
                if (Main.Settings.UseOfficialFlankingRules && Gui.Battle != null)
                {
                    FlankingAndHigherGroundRules.ClearFlankingDeterminationCache();
                }

                //PATCH: IActionFinishedByMe
                var iActionsFinished = rulesetCharacter.GetSubFeaturesByType<IActionFinishedByMe>();

                foreach (var actionFinished in iActionsFinished)
                {
                    yield return actionFinished.OnActionFinishedByMe(__instance);
                }

                //PATCH: support for IActionFinishedByEnemy
                if (Gui.Battle != null)
                {
                    foreach (var enemy in Gui.Battle.GetOpposingContenders(rulesetCharacter.Side)
                                 .ToList()) // avoid changing enumerator
                    {
                        var rulesetEnemy = enemy.RulesetCharacter;

                        if (rulesetEnemy is not { IsDeadOrDyingOrUnconscious: false })
                        {
                            continue;
                        }

                        foreach (var actionFinishedByEnemy in rulesetEnemy
                                     .GetSubFeaturesByType<IActionFinishedByEnemy>()
                                     .Where(x => x.ActionDefinition == __instance.ActionDefinition))
                        {
                            yield return actionFinishedByEnemy.OnActionFinishedByEnemy(enemy, __instance);
                        }
                    }
                }
            }

            //PATCH: support for character action tracking
            Global.CurrentAction = null;
        }
    }
}
