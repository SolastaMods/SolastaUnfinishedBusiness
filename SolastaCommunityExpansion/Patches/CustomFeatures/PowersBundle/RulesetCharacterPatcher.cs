using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PowersBundle
{
    [HarmonyPatch(typeof(RulesetCharacter), "TerminateMatchingUniquePower")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_TerminateMatchingUniquePower
    {
        internal static void Postfix(RulesetCharacter __instance, FeatureDefinitionPower powerDefinition)
        {
            if (!Main.Settings.EnablePowersBundlePatch)
            {
                return;
            }
            
            var bundles = PowerBundleContext.GetBundlesBySubpower(powerDefinition);

            var allBubPowers = new HashSet<FeatureDefinitionPower>();

            foreach (var masterPower in bundles)
            {
                var bundle = PowerBundleContext.GetBundle(masterPower);
                if (bundle.TerminateAll)
                {
                    foreach (var subPower in bundle.SubPowers)
                    {
                        allBubPowers.Add(subPower);
                    }
                }
            }

            allBubPowers.Remove(powerDefinition);

            var powersToTerminate = new HashSet<RulesetEffectPower>();
            var usedPowers = __instance.PowersUsedByMe;
            foreach (RulesetEffectPower usedPower in usedPowers)
            {
                if (allBubPowers.Contains(usedPower.PowerDefinition))
                {
                    powersToTerminate.Add(usedPower);
                }
            }

            foreach (RulesetEffectPower activePower in powersToTerminate)
                __instance.TerminatePower(activePower);
        }
    }
}
