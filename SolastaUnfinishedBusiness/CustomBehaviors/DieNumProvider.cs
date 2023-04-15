namespace SolastaUnfinishedBusiness.CustomValidators;

//Currently only supported for `FeatureDefinitionAdditionalDamage` inside GLBM,
//won't affect dice visible in the Attack Mode box in inventory, so use only for conditional features
internal delegate int DieNumProvider(RulesetCharacter character,
    FeatureDefinition original);
