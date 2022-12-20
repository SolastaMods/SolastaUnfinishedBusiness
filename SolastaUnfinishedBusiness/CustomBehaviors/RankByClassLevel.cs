using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class RankByClassLevel : IRankProvider
{
    private string className;

    public RankByClassLevel(string className)
    {
        this.className = className;
    }

    public RankByClassLevel(CharacterClassDefinition classDefinition) : this(classDefinition.Name)
    {
    }

    public int GetRank(RulesetCharacter chracter)
    {
        return chracter.GetClassLevel(className);
    }
}
