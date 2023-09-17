using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IValidateDefinitionPreRequisites
{
    [CanBeNull]
    public delegate bool Validate(RulesetCharacter character, BaseDefinition definition, out string requirement);
}
