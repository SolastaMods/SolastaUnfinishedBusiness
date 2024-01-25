namespace SolastaUnfinishedBusiness.Interfaces;

/// <summary>
///     Implement on an IAdditionalDamageProvider feature to allow its damage to scale with class level (using
///     DiceByRankTable), even if it isn't directly added to the class (via AddFeaturesAtLevel).
/// </summary>
public interface IClassHoldingFeature
{
    public CharacterClassDefinition Class { get; }
}
