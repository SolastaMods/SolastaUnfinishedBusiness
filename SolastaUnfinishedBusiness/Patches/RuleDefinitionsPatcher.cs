using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Subclasses;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RuleDefinitionsPatcher
{
    [HarmonyPatch(typeof(RuleDefinitions), nameof(ComputeAdvantage))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeAdvantage_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            [NotNull] List<TrendInfo> trends,
            ref AdvantageType __result)
        {
            //PATCH: Apply SRD setting `UseOfficialAdvantageDisadvantageRules`
            if (!Main.Settings.UseOfficialAdvantageDisadvantageRules)
            {
                return;
            }

            var hasAdvantage = trends.Any(t => t.value > 0);
            var hasDisadvantage = trends.Any(t => t.value < 0);

            if (!(hasAdvantage ^ hasDisadvantage))
            {
                __result = AdvantageType.None;
            }
            else if (hasAdvantage)
            {
                __result = AdvantageType.Advantage;
            }
            else
            {
                __result = AdvantageType.Disadvantage;
            }
        }
    }

    [HarmonyPatch(typeof(RuleDefinitions), nameof(TryGetAncestryDamageTypeFromCharacter))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TryGetAncestryDamageTypeFromCharacter_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            out bool __result,
            ulong guid,
            AncestryType ancestryType,
            out string ancestryDamageType)
        {
            __result = false;
            ancestryDamageType = string.Empty;

            if (!RulesetEntity.TryGetEntity<RulesetActor>(guid, out var entity))
            {
                return false;
            }

            // BEGIN PATCH

            //PATCH: allow Path of Elements and Way of Dragon to use proper condition effects related to their ancestry
            if (entity is RulesetCharacter rulesetCharacter)
            {
                if (rulesetCharacter.GetSubclassLevel(Barbarian, PathOfTheElements.Name) > 0)
                {
                    ancestryType = (AncestryType)ExtraAncestryType.PathOfTheElements;
                }
                else if (rulesetCharacter.GetSubclassLevel(Monk, WayOfTheDragon.Name) > 0)
                {
                    ancestryType = (AncestryType)ExtraAncestryType.WayOfTheDragon;
                }
            }

            // END PATCH

            entity.EnumerateFeaturesToBrowse<FeatureDefinitionAncestry>(FeatureDefinitionAncestry.FeaturesToBrowse);

            foreach (var definitionAncestry in FeatureDefinitionAncestry.FeaturesToBrowse
                         .OfType<FeatureDefinitionAncestry>()
                         .Where(x => x.Type == ancestryType && !string.IsNullOrEmpty(x.DamageType)))
            {
                ancestryDamageType = definitionAncestry.DamageType;

                __result = true;

                return false;
            }

            return false;
        }
    }
}
