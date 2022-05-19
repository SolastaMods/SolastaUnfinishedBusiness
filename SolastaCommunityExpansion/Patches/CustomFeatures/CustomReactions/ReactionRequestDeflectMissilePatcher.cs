using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Features;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomReactions;

internal static class ReactionRequestDeflectMissilePatcher
{
    [HarmonyPatch(typeof(ReactionRequestDeflectMissile), "FormatDescription")]
    internal static class ReadyActionSelectionPanel_Bind
    {
        internal static void Postfix(ReactionRequestDeflectMissile __instance, ref string __result)
        {
            var rulesetActor = __instance.ReactionParams.ActingCharacter.RulesetCharacter;
            var attacker = __instance.ReactionParams.TargetCharacters[0].RulesetCharacter;

            var customDeflector = rulesetActor.GetSubFeaturesByType<ICustomMissileDeflection>().FirstOrDefault();

            if (customDeflector == null)
            {
                return;
            }

            __result = customDeflector.FormatDescription(rulesetActor, attacker, __result);
        }
    }
}