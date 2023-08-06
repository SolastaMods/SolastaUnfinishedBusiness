using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class ModifyProviderRankByClassLevel : IModifyProviderRank
{
    private readonly string _className;

    private ModifyProviderRankByClassLevel(string className)
    {
        _className = className;
    }

    public ModifyProviderRankByClassLevel(BaseDefinition classDefinition) : this(classDefinition.Name)
    {
    }

    public int GetRank(RulesetCharacter character)
    {
        return character.GetClassLevel(_className);
    }
}
