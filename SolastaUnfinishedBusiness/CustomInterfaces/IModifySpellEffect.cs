namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface IModifySpellEffect
{
    EffectDescription ModifyEffect(BaseDefinition definition, EffectDescription effect, RulesetCharacter character);
}
