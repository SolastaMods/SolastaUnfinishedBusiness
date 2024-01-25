namespace SolastaUnfinishedBusiness.Interfaces;

public interface IValidatePowerUse
{
    public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power);
}
