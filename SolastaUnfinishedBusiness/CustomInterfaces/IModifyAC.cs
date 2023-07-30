namespace SolastaUnfinishedBusiness.CustomInterfaces;

// Free modification to AC
public interface IModifyAC
{
    void GetAC(RulesetCharacter owner,
        bool callRefresh,
        bool dryRun,
        FeatureDefinition dryRunFeature,
        out RulesetAttributeModifier attributeModifier,
        out RuleDefinitions.TrendInfo trendInfo);
}
