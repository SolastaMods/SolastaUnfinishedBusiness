namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifyMagicEffect
{
    public EffectDescription ModifyEffect(
        BaseDefinition definition,
        EffectDescription effectDescription,
        RulesetCharacter character,
        RulesetEffect rulesetEffect);
}
