namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal delegate void MetamagicApplicationValidator(
    RulesetCharacter caster,
    RulesetEffectSpell rulesetEffectSpell,
    MetamagicOptionDefinition metamagicOption,
    ref bool result,
    ref string failure);
