namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IDefinitionApplicationValidator
{
    bool IsValid(BaseDefinition definition, RulesetCharacter character);
}
