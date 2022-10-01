namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifySpellEffect
{
    EffectDescription ModifyEffect(BaseDefinition definition, EffectDescription effect, RulesetCharacter character);
}
