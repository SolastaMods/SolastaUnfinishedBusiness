using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Interfaces;

namespace SolastaUnfinishedBusiness.BehaviorsGeneric;

public class ModifyProviderRankByClassLevel(CharacterClassDefinition klass) : IModifyProviderRank
{
    public int GetRank(RulesetCharacter character)
    {
        return character.GetClassLevel(klass);
    }
}
