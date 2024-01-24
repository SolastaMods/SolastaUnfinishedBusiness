namespace SolastaUnfinishedBusiness.CustomValidators;

/**
 * Used to determine whether device function should be usable
 * So far implemented only for FeatureDefinitionActionAffinity and FeatureDefinitionAdditionalAction authorizing Bonus Action device use
 * Used for Poisoner feat
 * Can be extended to affect any specific device if needed
 */
public delegate bool ValidateDeviceFunctionUse(
    RulesetCharacter user,
    RulesetItemDevice device,
    RulesetDeviceFunction deviceFunction);
