using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Validators;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class SavingThrowColumnPatcher
{
    //PATCH: allow ISavingThrowAffinityProvider to be validated with IsCharacterValidHandler
    [HarmonyPatch(typeof(SavingThrowColumn), nameof(SavingThrowColumn.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CBind_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: make ISpellCastingAffinityProvider from dynamic item properties apply to repertoires
            return instructions.ReplaceEnumerateFeaturesToBrowse<ISavingThrowAffinityProvider>(
                "SavingThrowColumn.Bind", EnumerateFeatureDefinitionSavingThrowAffinity);
        }

        private static void EnumerateFeatureDefinitionSavingThrowAffinity(
            RulesetCharacter __instance,
            List<FeatureDefinition> featuresToBrowse,
            Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin)
        {
            __instance.EnumerateFeaturesToBrowse<ISavingThrowAffinityProvider>(featuresToBrowse,
                featuresOrigin);
            featuresToBrowse.RemoveAll(x =>
                !__instance.IsValid(x.GetAllSubFeaturesOfType<IsCharacterValidHandler>()));
        }
    }
}
