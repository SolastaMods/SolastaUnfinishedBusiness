namespace SolastaUnfinishedBusiness.CustomGenericBehaviors;

//currently only supported for `FeatureDefinitionAdditionalDamage` inside GLBM,
//won't affect dice visible in the Attack Mode box in inventory, so use only for conditional features
internal delegate RuleDefinitions.DieType ProvideAdditionalDamageDieType(RulesetCharacter character,
    RuleDefinitions.DieType original);

//currently only supported for `FeatureDefinitionAdditionalDamage` inside GLBM,
//won't affect dice visible in the Attack Mode box in inventory, so use only for conditional features
//we might need to change this to a proper interface if others start using it
//as Close Quarters interaction with Rogue Scoundrel 17 assumes this is only used by Close Quarters
internal delegate RuleDefinitions.DieType DamageDieProviderFromCharacter(
    FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
    DamageForm damageForm,
    RulesetAttackMode attackMode,
    GameLocationCharacter attacker,
    GameLocationCharacter defender);
