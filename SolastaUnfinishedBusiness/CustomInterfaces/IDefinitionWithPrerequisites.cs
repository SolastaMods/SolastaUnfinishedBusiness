using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IDefinitionWithPrerequisites
{
    [CanBeNull]
    public delegate bool Validate(RulesetCharacter character, BaseDefinition definition, out string requirement);
}
