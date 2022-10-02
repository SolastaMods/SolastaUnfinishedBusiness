namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifySpellEffect
{
    public EffectDescription ModifyEffect(BaseDefinition definition, EffectDescription effect,
        RulesetCharacter character);
}
