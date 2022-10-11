namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifyMagicEffect
{
    public EffectDescription ModifyEffect(BaseDefinition definition, EffectDescription effect,
        RulesetCharacter character);
}
