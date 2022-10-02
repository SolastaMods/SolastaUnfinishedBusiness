namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface IReduceDamage
{
    internal string NotificationTag { get; set; }
    internal int ReducedDamage { get; set; }
    internal string SourceName { get; set; }
    internal RuleDefinitions.FeatureSourceType SourceType { get; set; }
}
