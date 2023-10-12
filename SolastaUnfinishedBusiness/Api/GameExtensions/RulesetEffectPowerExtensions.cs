using System.Linq;
using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

public static class RulesetEffectPowerExtensions
{
    /**
     * Adds this RulesetEffectPower to the `PowersUsedByMe` of the user. If user is null, tries to get one from the effect.
     * Needs to be used when we instantiate power manually (not through CharacterActionUsePower).
     * Otherwise this RulesetEffectPower won't be serialized in the save file, leading to `Cannot reconcile RulesetEffect` errors on loading a save.
     */
    internal static RulesetEffectPower AddAsActivePowerToSource(this RulesetEffectPower rulesetPower)
    {
        var finalUser = EffectHelpers.GetCharacterByGuid(rulesetPower.SourceGuid);

        if (finalUser != null
            && finalUser.UsablePowers.All(x => x.PowerDefinition != rulesetPower.PowerDefinition))
        {
            Main.Info($"AddAsActivePowerToSource: {rulesetPower.PowerDefinition.Name} not in UsablePowers");

            finalUser.AddActivePower(rulesetPower);
        }
        else
        {
            Main.Info($"AddAsActivePowerToSource: {rulesetPower.PowerDefinition.Name} already in UsablePowers");
        }

        return rulesetPower;
    }
}
