using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.FeatDefinitionWithPrereqs
{
    [HarmonyPatch(typeof(GuiFeatDefinition), "IsFeatMacthingPrerequisites")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GuiFeatDefinition_IsFeatMatchingPrerequisites
    {
        internal static void Postfix(
            ref bool __result,
            FeatDefinition feat,
            RulesetCharacterHero hero,
            ref string prerequisiteOutput)
        {
            if (feat is not FeatDefinitionWithPrerequisites featDefinitionWithPrerequisites || featDefinitionWithPrerequisites.Validators.Count == 0)
            {
                return;
            }

            var (result, output) = featDefinitionWithPrerequisites.Validate(featDefinitionWithPrerequisites, hero);

            __result = __result && result;
            prerequisiteOutput += "\n" + output;
        }
    }
}
