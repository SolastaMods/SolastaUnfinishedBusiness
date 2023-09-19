namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IValidatePowerUse
{
    public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power);
}
