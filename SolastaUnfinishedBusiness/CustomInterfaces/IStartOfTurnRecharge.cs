namespace SolastaUnfinishedBusiness.CustomInterfaces;

/// <summary>
///     Implement on a FeatureDefinitionPower to allow it to recharge at the start of your turn.
/// </summary>
internal interface IStartOfTurnRecharge
{
    bool IsRechargeSilent { get; }
}
