namespace SolastaUnfinishedBusiness.Interfaces;

public interface IAddExtraAttack
{
    // sort sub features [used on race claw attacks]
    public int Priority();
    public void TryAddExtraAttack(RulesetCharacter character);
}
