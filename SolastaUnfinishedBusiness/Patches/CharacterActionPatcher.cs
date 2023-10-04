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
using static RuleDefinitions;

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
            // keep a tab on last cantrip weapon attack status
            Global.LastAttackWasCantripWeaponAttackHit = false;

            //PATCH: support for character action tracking
            Global.CurrentAction = __instance;

            switch (__instance)
            {
#if false
                case CharacterActionCastSpell or CharacterActionSpendSpellSlot:
                    //PATCH: Hold the state of the SHIFT key on bool 5 to determine which slot to use on MC Warlock
                    var isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                    __instance.actionParams.BoolParameter5 = isShiftPressed;
                    break;
#endif
                case CharacterActionReady:
                    CustomReactionsContext.ReadReadyActionPreferredCantrip(__instance.actionParams);
                    break;

                case CharacterActionSpendPower spendPower:
                    PowerBundle.SpendBundledPowerIfNeeded(spendPower);
                    break;

                // BUGFIX: saving throw not passing correct saving delta on attack actions
                case CharacterActionAttack:
                    Global.CurrentAttackAction = __instance;
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

            //PATCH: clear flanking rules determination cache on every action end
            if (Main.Settings.UseOfficialFlankingRules && Gui.Battle != null)
            {
                FlankingAndHigherGroundRules.ClearFlankingDeterminationCache();
            }

            var actingCharacter = __instance.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            if (rulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
            {
                //PATCH: support for `IActionFinishedByMe`
                foreach (var actionFinished in rulesetCharacter.GetSubFeaturesByType<IActionFinishedByMe>())
                {
                    yield return actionFinished.OnActionFinishedByMe(__instance);
                }

                //PATCH: support for `IActionFinishedByEnemy`
                if (Gui.Battle != null && rulesetCharacter.Side != Side.Ally)
                {
                    foreach (var target in Gui.Battle.AllContenders
                                 .Where(x => x.IsOppositeSide(actingCharacter.Side) && x.CanAct())
                                 .ToList()) // avoid changing enumerator
                    {
                        var rulesetTarget = target.RulesetCharacter;

                        foreach (var actionFinishedByEnemy in rulesetTarget
                                     .GetSubFeaturesByType<IActionFinishedByEnemy>())
                        {
                            yield return actionFinishedByEnemy.OnActionFinishedByEnemy(__instance, target);
                        }
                    }
                }
            }

            //PATCH: support for character action tracking
            Global.CurrentAction = null;

            // BUGFIX: saving throw not passing correct saving delta on attack actions
            Global.CurrentAttackAction = null;
        }
    }
}
