namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IProvideMetamagicBehavior
{
    // return false here if not the metamagicOption you need to handle
    public bool IsMetamagicOptionAvailable(
        RulesetEffectSpell rulesetEffectSpell,
        RulesetCharacter caster,
        MetamagicOptionDefinition metamagicOption,
        ref string failure,
        ref bool result);

    public void MetamagicSelected(
        GameLocationCharacter caster,
        RulesetEffectSpell spellEffect,
        MetamagicOptionDefinition metamagicOption);
}
