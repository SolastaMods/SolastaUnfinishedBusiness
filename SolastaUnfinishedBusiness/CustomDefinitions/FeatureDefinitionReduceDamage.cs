namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal sealed class FeatureDefinitionReduceDamage : FeatureDefinition
{
    public int ReducedDamage { get; set; }
    public string NotificationTag { get; set; }
    public string SourceName { get; set; }
    public RuleDefinitions.FeatureSourceType SourceType { get; set; }
}
