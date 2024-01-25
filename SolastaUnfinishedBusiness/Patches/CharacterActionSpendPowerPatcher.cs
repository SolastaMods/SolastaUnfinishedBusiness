using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.BehaviorsGeneric;

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
        public static IEnumerator Postfix(
            [NotNull] IEnumerator values,
            CharacterActionSpendPower __instance)
        {
            while (values.MoveNext())
            {
                yield return values.Current;
            }

            //PATCH: support for shared pool powers that character got from conditions to properly consume uses when triggered
            if (__instance.ActionParams.RulesetEffect is not RulesetEffectPower { OriginItem: null } activePower)
            {
                yield break;
            }

            var usablePower = activePower.UsablePower;

            // in this case base game already calls `UsePower`
            if (usablePower.OriginClass != null || usablePower.OriginRace != null)
            {
                yield break;
            }

            if (usablePower.powerDefinition.HasSubFeatureOfType<ForcePowerUseInSpendPowerAction>())
            {
                __instance.ActingCharacter.RulesetCharacter.UsePower(usablePower);
            }
        }
    }
}
