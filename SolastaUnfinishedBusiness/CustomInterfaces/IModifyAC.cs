using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

// Free modification to AC
public interface IModifyAC
{
    void GetAC(RulesetCharacter owner,
        [UsedImplicitly] bool callRefresh,
        [UsedImplicitly] bool dryRun,
        [UsedImplicitly] FeatureDefinition dryRunFeature,
        out RulesetAttributeModifier attributeModifier,
        out RuleDefinitions.TrendInfo trendInfo);
}
