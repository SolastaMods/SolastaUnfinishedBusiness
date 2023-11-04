namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IAddExtraAttack
{
    // For sorting subfeatures
    public int Priority();
    public void TryAddExtraAttack(RulesetCharacter character);
}
