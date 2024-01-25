using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.BehaviorsGeneric;
using SolastaUnfinishedBusiness.Validators;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class PowerSelectionPanelPatcher
{
    [HarmonyPatch(typeof(PowerSelectionPanel), nameof(PowerSelectionPanel.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var setPowerCancelledMethod = typeof(PowerSelectionPanel).GetMethod("set_PowerCancelled");
            var removeInvalidPowersMethod =
                new Action<PowerSelectionPanel, RulesetCharacter>(RemoveInvalidPowers).Method;

            return instructions.ReplaceCall(
                setPowerCancelledMethod,
                1,
                "PowerSelectionPanel.Bind",
                new CodeInstruction(OpCodes.Call, setPowerCancelledMethod), // checked for Call vs CallVirtual
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, removeInvalidPowersMethod));
        }

        //PATCH: remove invalid powers from display using our custom validators
        private static void RemoveInvalidPowers(PowerSelectionPanel panel, RulesetCharacter character)
        {
            var usablePowers = character.UsablePowers;
            var relevantPowers = panel.relevantPowers;
            var actionType = panel.ActionType;

            foreach (var power in usablePowers
                         .Select(power => new
                         {
                             power,
                             feature = power.PowerDefinition.GetFirstSubFeatureOfType<ModifyPowerVisibility>()
                         })
                         .Where(t => t.feature != null)
                         .Where(t => t.feature.IsVisible(character, t.power.PowerDefinition, actionType))
                         .Select(t => t.power))
            {
                relevantPowers.TryAdd(power);
            }

            for (var i = relevantPowers.Count - 1; i >= 0; i--)
            {
                var power = relevantPowers[i];

                if (ValidatorsValidatePowerUse.IsPowerNotValid(character, power)
                    || ModifyPowerVisibility.IsPowerHidden(character, power, actionType))
                {
                    relevantPowers.RemoveAt(i);
                }
            }
        }
    }
}
