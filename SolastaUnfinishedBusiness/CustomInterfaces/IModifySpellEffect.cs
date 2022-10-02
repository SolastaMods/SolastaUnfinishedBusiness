namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface IModifySpellEffect
{
    public EffectDescription ModifyEffect(BaseDefinition definition, EffectDescription effect,
        RulesetCharacter character);
}
