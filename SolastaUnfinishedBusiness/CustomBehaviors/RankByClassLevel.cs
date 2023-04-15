using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomValidators;

public class RankByClassLevel : IRankProvider
{
    private readonly string className;

    private RankByClassLevel(string className)
    {
        this.className = className;
    }

    public RankByClassLevel(BaseDefinition classDefinition) : this(classDefinition.Name)
    {
    }

    public int GetRank(RulesetCharacter character)
    {
        return character.GetClassLevel(className);
    }
}
