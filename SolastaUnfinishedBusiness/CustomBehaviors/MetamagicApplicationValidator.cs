namespace SolastaUnfinishedBusiness.CustomBehaviors;

delegate void MetamagicApplicationValidator(
    RulesetCharacter caster,
    RulesetEffectSpell rulesetEffectSpell,
    MetamagicOptionDefinition metamagicOption,
    ref bool result,
    ref string failure);
