namespace SolastaUnfinishedBusiness.CustomInterfaces;

/// <summary>
///     Implement on an IAdditionalDamageProvider feature to allow its damage to scale with class level (using
///     DiceByRankTable), even if it isn't directly added to the class (via AddFeaturesAtLevel).
/// </summary>
internal interface IClassHoldingFeature
{
    public CharacterClassDefinition Class { get; }
}
