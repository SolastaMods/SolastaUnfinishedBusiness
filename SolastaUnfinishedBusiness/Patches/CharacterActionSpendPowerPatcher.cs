using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomValidators;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionSpendPowerPatcher
{
    [HarmonyPatch(typeof(CharacterActionSpendPower), nameof(CharacterActionSpendPower.ExecuteImpl))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ExecuteImpl_Patch
    {
        [UsedImplicitly]
        public static void Postfix(CharacterActionSpendPower __instance)
        {
            //PATCH: support for shared pool powers that character got from conditions to properly consume uses when triggered

            if (__instance.ActionParams.RulesetEffect is not RulesetEffectPower { OriginItem: null } activePower)
            {
                return;
            }

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
