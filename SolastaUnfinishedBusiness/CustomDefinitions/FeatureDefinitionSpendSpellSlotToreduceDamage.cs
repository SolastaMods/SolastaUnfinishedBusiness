namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal sealed class FeatureDefinitionSpendSpellSlotToReduceDamage : FeatureDefinition
{
    public int ReducedDamage { get; set; }
    public string NotificationTag { get; set; }
    public string SourceName { get; set; }
    public RuleDefinitions.FeatureSourceType SourceType { get; set; }
}
