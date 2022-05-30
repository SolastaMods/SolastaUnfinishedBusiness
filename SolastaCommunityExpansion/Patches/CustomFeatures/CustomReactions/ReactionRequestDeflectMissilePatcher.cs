using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomReactions;

[HarmonyPatch(typeof(ReactionRequestDeflectMissile), "FormatDescription")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class ReactionRequestDeflectMissile_FormatDescription
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
