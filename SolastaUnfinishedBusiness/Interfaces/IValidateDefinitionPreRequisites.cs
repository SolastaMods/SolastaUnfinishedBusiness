using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IValidateDefinitionPreRequisites
{
    [CanBeNull]
    public delegate bool Validate(RulesetCharacter character, BaseDefinition definition, out string requirement);
}
