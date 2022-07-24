namespace SolastaCommunityExpansion.CustomInterfaces;

public interface IReduceDamage
{
    public string NotificationTag { get; set; }
    public int ReducedDamage { get; set; }
    public string SourceName { get; set; }
    public RuleDefinitions.FeatureSourceType SourceType { get; set; }
}
