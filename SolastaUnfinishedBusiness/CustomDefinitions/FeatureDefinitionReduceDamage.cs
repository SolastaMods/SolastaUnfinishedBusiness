using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

public sealed class FeatureDefinitionReduceDamage : FeatureDefinition, IReduceDamage
{
    public int ReducedDamage { get; set; }
    public string NotificationTag { get; set; }
    public string SourceName { get; set; }
    public RuleDefinitions.FeatureSourceType SourceType { get; set; }
}
