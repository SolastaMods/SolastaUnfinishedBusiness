using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomBehaviors;

namespace SolastaUnfinishedBusiness.Patches;

public static class CharacterActionSpendPowerPatcher
{
    [HarmonyPatch(typeof(CharacterActionSpendPower), "ExecuteImpl")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class ExecuteImpl_Patch
    {
        public static void Postfix(CharacterActionSpendPower __instance)
        {
            //PATCH: support for shared pool powers that character got from conditions to properly consume uses when triggered
            var activePower = __instance.ActionParams.RulesetEffect as RulesetEffectPower;
            if (activePower == null || activePower.OriginItem != null) { return; }

            var usablePower = activePower.UsablePower;
            if (usablePower.OriginClass != null || usablePower.OriginRace != null)
            {
                //in this case base game already calls `UsePower`
                return;
            }

            if (usablePower.powerDefinition.HasSubFeatureOfType<ForcePowerUseInSpendPowerAction>())
            {
                __instance.ActingCharacter.RulesetCharacter.UsePower(usablePower);
            }
        }
    }
}
