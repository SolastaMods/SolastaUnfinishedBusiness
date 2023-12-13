using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class ModifyProviderRankByClassLevel : IModifyProviderRank
{
    private readonly CharacterClassDefinition _class;

    public ModifyProviderRankByClassLevel(CharacterClassDefinition klass)
    {
        _class = klass;
    }

    public int GetRank(RulesetCharacter character)
    {
        return character.GetClassLevel(_class);
    }
}
