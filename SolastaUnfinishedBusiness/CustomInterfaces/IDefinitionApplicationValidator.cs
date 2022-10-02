namespace SolastaUnfinishedBusiness.CustomInterfaces;

/**
 * Validates definition before applying.
 * Currently supports:
 * `IAdditionalActionsProvider`
 * `IActionPerformanceProvider`
 * `FeatureDefinitionAttributeModifier` (except ArmorClass attribute) applied through Conditions
 */
internal interface IDefinitionApplicationValidator
{
    bool IsValid(BaseDefinition definition, RulesetCharacter character);
}
