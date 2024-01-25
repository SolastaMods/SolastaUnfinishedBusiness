using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomGenericBehaviors;

public class ModifyProviderRankByClassLevel(CharacterClassDefinition klass) : IModifyProviderRank
{
    public int GetRank(RulesetCharacter character)
    {
        return character.GetClassLevel(klass);
    }
}
