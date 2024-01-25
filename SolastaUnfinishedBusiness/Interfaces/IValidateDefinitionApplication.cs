namespace SolastaUnfinishedBusiness.Interfaces;

/**
 * Validates definition before applying.
 * Currently supports:
 * `IAdditionalActionsProvider`
 * `IActionPerformanceProvider`
 * `FeatureDefinitionAttributeModifier` (except ArmorClass attribute) applied through Conditions
 */
public interface IValidateDefinitionApplication
{
    public bool IsValid(BaseDefinition definition, RulesetCharacter character);
}
