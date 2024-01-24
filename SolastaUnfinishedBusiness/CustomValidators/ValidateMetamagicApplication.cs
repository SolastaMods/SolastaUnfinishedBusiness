namespace SolastaUnfinishedBusiness.CustomValidators;

internal delegate void ValidateMetamagicApplication(
    RulesetCharacter caster,
    RulesetEffectSpell rulesetEffectSpell,
    MetamagicOptionDefinition metamagicOption,
    ref bool result,
    ref string failure);
