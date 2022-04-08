namespace SolastaCommunityExpansion.CustomDefinitions
{
    /// <summary>
    /// Implement on a FeatureDefinitionPower to allow it to recharge at the start of your turn.
    /// </summary>
    public interface IStartOfTurnRecharge
    {
        bool IsRechargeSilent { get; }
    }
}
